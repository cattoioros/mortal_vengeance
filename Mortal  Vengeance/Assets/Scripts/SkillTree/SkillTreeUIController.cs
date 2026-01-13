using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillTreeUIController : MonoBehaviour
{
    public static SkillTreeUIController Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Root GameObject to show/hide. If null, uses this GameObject.")]
    [SerializeField] private GameObject skillTreeRoot;

    [Tooltip("Optional Back button that calls Close().")]
    [SerializeField] private Button backButton;

    [Header("Input")]
    [Tooltip("Close key while open.")]
    [SerializeField] private Key closeKey = Key.Escape;

    [Tooltip("Also allow closing with the interact key.")]
    [SerializeField] private Key interactCloseKey = Key.E;

    [Header("Behavior")]
    [SerializeField] private bool pauseGameWhileOpen = true;
    [SerializeField] private bool lockCursorWhenClosed = true;

    private bool isOpen;

    private void Awake()
    {
        // Singleton so world triggers can open/close one shared skill UI.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (skillTreeRoot == null) skillTreeRoot = gameObject;

        if (backButton == null)
        {
            // Convenience: auto-find a Button named "Back" or "BackButton".
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
            // When the tree is open we pause gameplay + show cursor for UI interaction.
            if (pauseGameWhileOpen) Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SystemMenuController.SetUIBlockingInput(true);
        }
        else
        {
            // Restore gameplay state + cursor state.
            if (pauseGameWhileOpen) Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = lockCursorWhenClosed ? CursorLockMode.Locked : CursorLockMode.None;
            SystemMenuController.SetUIBlockingInput(false);
        }
    }
}
