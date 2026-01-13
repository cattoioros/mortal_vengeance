using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform skillButtonContainer;
    [SerializeField] private Button skillButtonPrefab;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private Button unlockButton;

    [Header("Layout")]
    [Tooltip("Preferred height for each skill button (used by VerticalLayoutGroup if present).")]
    [SerializeField] private float skillButtonPreferredHeight = 44f;
    [Tooltip("Enable TMP auto-sizing so long names fit without clipping.")]
    [SerializeField] private bool autoSizeSkillButtonText = true;
    [SerializeField] private float skillButtonTextMaxSize = 20f;
    [SerializeField] private float skillButtonTextMinSize = 12f;

    private readonly Dictionary<string, Button> buttonsBySkillId = new Dictionary<string, Button>();
    private Skill currentlySelectedSkill;

    private SkillTreeManager skillTreeManager;
    private SkillData skillData;

    private bool isInitialized;

    private void Awake()
    {
        // Grab singletons early; we still re-check in Start for safety.
        skillTreeManager = SkillTreeManager.instance;
        skillData = SkillData.instance;
    }

    private void OnEnable()
    {
        // OnEnable can run before Start (first activation), so avoid Refresh until initialized.
        if (skillTreeManager == null) skillTreeManager = SkillTreeManager.instance;
        if (skillData == null) skillData = SkillData.instance;

        Subscribe();

        if (isInitialized)
        {
            Refresh();
        }
    }

    private void OnDisable()
    {
        // Avoid stale subscriptions when the UI is toggled on/off.
        Unsubscribe();
    }

    private void Start()
    {
        if (skillTreeManager == null)
        {
            skillTreeManager = FindAnyObjectByType<SkillTreeManager>();
        }

        if (skillData == null)
        {
            skillData = FindAnyObjectByType<SkillData>();
        }

        if (skillTreeManager == null || skillData == null)
        {
            Debug.LogError("SkillTreeUI missing SkillTreeManager or SkillData in scene.");
            enabled = false;
            return;
        }

        Subscribe();

        if (skillButtonContainer == null || skillButtonPrefab == null)
        {
            Debug.LogError("SkillTreeUI missing skillButtonContainer or skillButtonPrefab.");
            enabled = false;
            return;
        }

        // Prevent TMP labels from blocking clicks.
        if (skillPointsText != null) skillPointsText.raycastTarget = false;
        if (skillNameText != null) skillNameText.raycastTarget = false;
        if (skillDescriptionText != null) skillDescriptionText.raycastTarget = false;

        // Build the list once; later Refresh just updates colors/text.
        BuildButtons();
        SelectSkill(null);
        
        if (unlockButton != null)
        {
            unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        }
        
        isInitialized = true;
        Refresh();
    }

    private void Subscribe()
    {
        if (skillTreeManager == null) return;
        skillTreeManager.SkillPointsChanged -= OnSkillTreeChanged;
        skillTreeManager.SkillsChanged -= OnSkillTreeChanged;
        skillTreeManager.SkillPointsChanged += OnSkillTreeChanged;
        skillTreeManager.SkillsChanged += OnSkillTreeChanged;
    }

    private void Unsubscribe()
    {
        if (skillTreeManager == null) return;
        skillTreeManager.SkillPointsChanged -= OnSkillTreeChanged;
        skillTreeManager.SkillsChanged -= OnSkillTreeChanged;
    }

    private void OnSkillTreeChanged(int _)
    {
        if (isInitialized) Refresh();
    }

    private void OnSkillTreeChanged()
    {
        if (isInitialized) Refresh();
    }

    private void BuildButtons()
    {
        buttonsBySkillId.Clear();

        // Clear existing children (keep prefab if it's stored under the container).
        var toDestroy = new List<GameObject>();
        for (int i = 0; i < skillButtonContainer.childCount; i++)
        {
            var child = skillButtonContainer.GetChild(i).gameObject;
            if (child == skillButtonPrefab.gameObject) continue;
            toDestroy.Add(child);
        }

        foreach (var go in toDestroy)
        {
            Destroy(go);
        }

        foreach (var group in GroupSkills(skillData.allSkills))
        {
            AddHeader(group.categoryDisplayName);

            foreach (Skill skill in group.skills)
            {
                if (skill == null || string.IsNullOrWhiteSpace(skill.skillId))
                    continue;

                Button button = Instantiate(skillButtonPrefab, skillButtonContainer);
                button.gameObject.name = $"SkillButton_{skill.skillId}";
                button.gameObject.SetActive(true);

                // Make sure the button has enough vertical space for the label.
                // This plays nicely with a VerticalLayoutGroup on the container.
                var layoutElement = button.GetComponent<LayoutElement>();
                if (layoutElement == null) layoutElement = button.gameObject.AddComponent<LayoutElement>();
                layoutElement.preferredHeight = Mathf.Max(24f, skillButtonPreferredHeight);

                var label = button.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = skill.skillName;
                    label.raycastTarget = false;

                    // Prevent clipping on longer names.
                    label.enableWordWrapping = true;
                    label.overflowMode = TextOverflowModes.Ellipsis;
                    if (autoSizeSkillButtonText)
                    {
                        label.enableAutoSizing = true;
                        label.fontSizeMax = Mathf.Max(skillButtonTextMinSize, skillButtonTextMaxSize);
                        label.fontSizeMin = Mathf.Max(8f, Mathf.Min(skillButtonTextMinSize, label.fontSizeMax));
                    }
                }

                string capturedId = skill.skillId;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnSkillButtonClicked(capturedId));

                buttonsBySkillId[capturedId] = button;
            }
        }
    }

    private IEnumerable<(string categoryId, string categoryDisplayName, List<Skill> skills)> GroupSkills(List<Skill> skills)
    {
        // Stable ordering keeps the UI predictable.
        var categoryOrder = new List<(string id, string display)>
        {
            ("hlt", "Health"),
            ("str", "Strength"),
            ("other", "Other")
        };

        var byCategory = new Dictionary<string, List<Skill>>
        {
            ["hlt"] = new List<Skill>(),
            ["str"] = new List<Skill>(),
            ["other"] = new List<Skill>()
        };

        foreach (Skill skill in skills)
        {
            if (skill == null) continue;
            byCategory[GetCategoryId(skill.skillId)].Add(skill);
        }

        foreach (var (id, display) in categoryOrder)
        {
            var list = byCategory[id];
            if (list.Count == 0) continue;
            // Stable ordering within a category.
            list.Sort((a, b) => string.CompareOrdinal(a.skillId, b.skillId));
            yield return (id, display, list);
        }
    }

    private string GetCategoryId(string skillId)
    {
        // Categories are inferred from skillId prefixes (e.g., "hlt_", "str_").
        if (string.IsNullOrWhiteSpace(skillId)) return "other";
        if (skillId.StartsWith("hlt_")) return "hlt";
        if (skillId.StartsWith("str_")) return "str";
        return "other";
    }

    private void AddHeader(string text)
    {
        var headerObj = new GameObject($"Header_{text}", typeof(RectTransform));
        headerObj.transform.SetParent(skillButtonContainer, false);

        var headerText = headerObj.AddComponent<TextMeshProUGUI>();
        headerText.text = text;
        headerText.raycastTarget = false;

        // Reuse styling from an existing TMP text if available.
        if (skillPointsText != null)
        {
            headerText.font = skillPointsText.font;
            headerText.fontSharedMaterial = skillPointsText.fontSharedMaterial;
        }

        headerText.fontSize = 22;
        headerText.alignment = TextAlignmentOptions.Left;

        // Make layout behave nicely under a VerticalLayoutGroup.
        var layout = headerObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 32;
    }

    private void OnSkillButtonClicked(string skillId)
    {
        Skill skill = skillData.GetSkill(skillId);
        SelectSkill(skill);
        Refresh();
    }

    private void OnUnlockButtonClicked()
    {
        if (currentlySelectedSkill == null)
            return;

        skillTreeManager.TryUnlockSkill(currentlySelectedSkill.skillId);
        SelectSkill(currentlySelectedSkill);
        Refresh();
    }

    private void SelectSkill(Skill skill)
    {
        currentlySelectedSkill = skill;

        if (skillNameText == null || skillDescriptionText == null)
            return;

        if (skill == null)
        {
            skillNameText.text = "Skill: (none)";
            skillDescriptionText.text = "Click a skill to view details";
            if (unlockButton != null)
            {
                unlockButton.interactable = false;
                var buttonText = unlockButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = "Unlock";
            }
            return;
        }

        skillNameText.text = skill.skillName;

        string status;
        bool canUnlock = false;
        if (skillTreeManager.IsSkillUnlocked(skill.skillId))
        {
            status = "[UNLOCKED]";
        }
        else if (!PrereqsMet(skill))
        {
            status = "[LOCKED: prerequisites]";
        }
        else if (skillTreeManager.GetAvailablePoints() < skill.skillPointCost)
        {
            status = "[LOCKED: points]";
        }
        else
        {
            status = $"[Cost: {skill.skillPointCost}]";
            canUnlock = true;
        }

        skillDescriptionText.text = $"{skill.description}\n{status}";

        if (unlockButton != null)
        {
            unlockButton.gameObject.SetActive(true);
            unlockButton.interactable = canUnlock && !skillTreeManager.IsSkillUnlocked(skill.skillId);
            
            var buttonText = unlockButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (skillTreeManager.IsSkillUnlocked(skill.skillId))
                    buttonText.text = "Unlocked";
                else
                    buttonText.text = "Unlock";
            }
        }
    }

    private bool PrereqsMet(Skill skill)
    {
        foreach (string prereqId in skill.prerequisiteSkillIds)
        {
            if (!skillTreeManager.IsSkillUnlocked(prereqId))
                return false;
        }

        return true;
    }

    private void Refresh()
    {
        if (skillTreeManager == null || skillData == null)
            return;

        if (skillPointsText != null)
        {
            skillPointsText.text = $"Skill Points: {skillTreeManager.GetAvailablePoints()}";
        }

        foreach (var pair in buttonsBySkillId)
        {
            string skillId = pair.Key;
            Button button = pair.Value;

            Skill skill = skillData.GetSkill(skillId);
            if (skill == null) continue;

            bool unlocked = skillTreeManager.IsSkillUnlocked(skillId);
            bool canUnlock = !unlocked && PrereqsMet(skill) && skillTreeManager.GetAvailablePoints() >= skill.skillPointCost;

            // Always clickable so the player can read details; color indicates state.
            button.interactable = true;

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                if (unlocked)
                    image.color = new Color(0.35f, 0.75f, 0.35f, 1f);
                else if (canUnlock)
                    image.color = new Color(0.85f, 0.85f, 0.85f, 1f);
                else
                    image.color = new Color(0.55f, 0.55f, 0.55f, 1f);
            }
        }
    }
}
