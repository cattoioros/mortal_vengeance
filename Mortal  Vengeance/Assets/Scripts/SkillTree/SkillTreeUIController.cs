using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillTreeUIController : MonoBehaviour
{
    public static SkillTreeUIController Instance { get; private set; }

    [Header("UI")]
    [Tooltip("The root GameObject of the skill tree UI to show/hide. If null, uses this GameObject.")]
    [SerializeField] private GameObject skillTreeRoot;

    [Tooltip("Optional Back button in the skill tree UI. If assigned, it will close the skill tree.")]
    [SerializeField] private Button backButton;

    [Header("Input")]
    [Tooltip("Primary close key while the skill tree is open.")]
    [SerializeField] private Key closeKey = Key.Escape;

    [Tooltip("Also allow closing with the same interact key used to open the skill tree.")]
    [SerializeField] private Key interactCloseKey = Key.E;

    [Header("Behavior")]
    [SerializeField] private bool pauseGameWhileOpen = true;
    [SerializeField] private bool lockCursorWhenClosed = true;

    private bool isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (skillTreeRoot == null) skillTreeRoot = gameObject;

        if (backButton == null)
        {
            // Optional convenience: auto-find a Button named "Back" or "BackButton".
            var buttons = GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                var b = buttons[i];
                if (b == null) continue;
                if (b.gameObject.name == "Back" || b.gameObject.name == "BackButton")
                {
                    backButton = b;
                    break;
                }
            }
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(Close);
        }

        // Start hidden so it doesn't cover the screen.
        SetOpen(false);
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Keyboard.current != null &&
            (Keyboard.current[closeKey].wasPressedThisFrame || Keyboard.current[interactCloseKey].wasPressedThisFrame))
        {
            Close();
        }
    }

    public void Open()
    {
        if (SystemMenuController.IsUIBlockingInput && !isOpen)
            return;

        SetOpen(true);
    }

    public void Close()
    {
        SetOpen(false);
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    private void SetOpen(bool open)
    {
        isOpen = open;

        if (skillTreeRoot != null)
        {
            skillTreeRoot.SetActive(open);
        }

        if (open)
        {
            if (pauseGameWhileOpen) Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SystemMenuController.SetUIBlockingInput(true);
        }
        else
        {
            if (pauseGameWhileOpen) Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = lockCursorWhenClosed ? CursorLockMode.Locked : CursorLockMode.None;
            SystemMenuController.SetUIBlockingInput(false);
        }
    }
}
