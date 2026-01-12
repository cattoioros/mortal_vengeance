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

    private readonly Dictionary<string, Button> buttonsBySkillId = new Dictionary<string, Button>();
    private Skill currentlySelectedSkill;

    private SkillTreeManager skillTreeManager;
    private SkillData skillData;

    private void Awake()
    {
        skillTreeManager = SkillTreeManager.instance;
        skillData = SkillData.instance;
    }

    private void OnEnable()
    {
        // When the UI is opened (SetActive(true)), refresh immediately.
        if (skillTreeManager == null) skillTreeManager = SkillTreeManager.instance;
        Refresh();
    }

    private void OnDisable()
    {
        // Ensure we don't keep stale subscriptions if the UI object is disabled/enabled.
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

        // Prevent UI text from blocking clicks on buttons behind it.
        if (skillPointsText != null) skillPointsText.raycastTarget = false;
        if (skillNameText != null) skillNameText.raycastTarget = false;
        if (skillDescriptionText != null) skillDescriptionText.raycastTarget = false;

        BuildButtons();
        SelectSkill(null);
        
        if (unlockButton != null)
        {
            unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        }
        
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
        Refresh();
    }

    private void OnSkillTreeChanged()
    {
        Refresh();
    }

    private void BuildButtons()
    {
        buttonsBySkillId.Clear();

        // Clear existing children (except prefab if user kept it inside container)
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

                var label = button.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = skill.skillName;
                    label.raycastTarget = false;
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
        // Keep a predictable order.
        var categoryOrder = new List<(string id, string display)>
        {
            ("str", "Strength"),
            ("int", "Intelligence"),
            ("dex", "Dexterity"),
            ("other", "Other")
        };

        var byCategory = new Dictionary<string, List<Skill>>
        {
            ["str"] = new List<Skill>(),
            ["int"] = new List<Skill>(),
            ["dex"] = new List<Skill>(),
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
            // Stable ordering within a category: keeps it scalable and predictable.
            list.Sort((a, b) => string.CompareOrdinal(a.skillId, b.skillId));
            yield return (id, display, list);
        }
    }

    private string GetCategoryId(string skillId)
    {
        if (string.IsNullOrWhiteSpace(skillId)) return "other";
        if (skillId.StartsWith("str_")) return "str";
        if (skillId.StartsWith("int_")) return "int";
        if (skillId.StartsWith("dex_")) return "dex";
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

        // Make layout behave nicely under VerticalLayoutGroup.
        var layout = headerObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 28;
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

            // Interactable: allow click always so info shows; but visually indicate state.
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
