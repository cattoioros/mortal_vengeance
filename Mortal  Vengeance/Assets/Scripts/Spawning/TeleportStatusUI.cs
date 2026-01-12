using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeleportStatusUI : MonoBehaviour
{
    public static TeleportStatusUI Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Optional. If left empty, the script will auto-find a TMP/TextMesh/UGUI Text object named 'TeleportStatus'.")]
    [SerializeField] private TMP_Text messageTMP;

    [Tooltip("Optional fallback for legacy UGUI Text.")]
    [SerializeField] private Text messageUGUI;

    [Tooltip("Optional fallback for 3D TextMesh.")]
    [SerializeField] private TextMesh messageTextMesh;

    [Header("Behavior")]
    [SerializeField] private float messageDurationSeconds = 2f;

    private Coroutine activeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (messageTMP == null && messageUGUI == null && messageTextMesh == null)
        {
            messageTMP = GetComponentInChildren<TMP_Text>(true);
            if (messageTMP == null) messageUGUI = GetComponentInChildren<Text>(true);
            if (messageTMP == null && messageUGUI == null) messageTextMesh = GetComponentInChildren<TextMesh>(true);
        }

        if (messageTMP == null && messageUGUI == null && messageTextMesh == null)
        {
            // Try to find a scene object named exactly "TeleportStatus".
            var byName = GameObject.Find("TeleportStatus");
            if (byName != null)
            {
                messageTMP = byName.GetComponent<TMP_Text>();
                if (messageTMP == null) messageTMP = byName.GetComponentInChildren<TMP_Text>(true);

                if (messageTMP == null)
                {
                    messageUGUI = byName.GetComponent<Text>();
                    if (messageUGUI == null) messageUGUI = byName.GetComponentInChildren<Text>(true);
                }

                if (messageTMP == null && messageUGUI == null)
                {
                    messageTextMesh = byName.GetComponent<TextMesh>();
                    if (messageTextMesh == null) messageTextMesh = byName.GetComponentInChildren<TextMesh>(true);
                }
            }
        }

        if (messageTMP == null && messageUGUI == null && messageTextMesh == null)
        {
            // Last resort: scan for a matching text object name (case-insensitive).
            var allTMP = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < allTMP.Length && messageTMP == null; i++)
            {
                var t = allTMP[i];
                if (t == null) continue;
                if (string.Equals(t.gameObject.name, "TeleportStatus", System.StringComparison.OrdinalIgnoreCase))
                    messageTMP = t;
            }

            if (messageTMP == null)
            {
                var allUGUI = Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                for (int i = 0; i < allUGUI.Length && messageUGUI == null; i++)
                {
                    var t = allUGUI[i];
                    if (t == null) continue;
                    if (string.Equals(t.gameObject.name, "TeleportStatus", System.StringComparison.OrdinalIgnoreCase))
                        messageUGUI = t;
                }
            }

            if (messageTMP == null && messageUGUI == null)
            {
                var allTextMesh = Object.FindObjectsByType<TextMesh>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                for (int i = 0; i < allTextMesh.Length && messageTextMesh == null; i++)
                {
                    var t = allTextMesh[i];
                    if (t == null) continue;
                    if (string.Equals(t.gameObject.name, "TeleportStatus", System.StringComparison.OrdinalIgnoreCase))
                        messageTextMesh = t;
                }
            }
        }

        if (messageTMP == null && messageUGUI == null && messageTextMesh == null)
        {
            CreateDefaultOverlay();
        }

        EnsureRightSideLayout();

        HideImmediate();
    }

    public static void Show(string message, float? durationSeconds = null)
    {
        if (Instance == null)
        {
            // If no UI exists in the scene, do nothing (keeps this optional).
            return;
        }

        Instance.ShowInternal(message, durationSeconds ?? Instance.messageDurationSeconds);
    }

    /// <summary>
    /// Shows a message until you explicitly call Hide().
    /// </summary>
    public static void ShowPersistent(string message)
    {
        if (Instance == null) return;
        Instance.ShowInternal(message, duration: 0f);
    }

    /// <summary>
    /// Shows a message. Pass durationSeconds &lt;= 0 for persistent.
    /// </summary>
    public static void ShowPersistent(string message, float durationSeconds)
    {
        if (Instance == null) return;
        Instance.ShowInternal(message, durationSeconds);
    }

    public static void Hide()
    {
        if (Instance == null) return;
        Instance.HideImmediate();
    }

    private void ShowInternal(string message, float duration)
    {
        if (!HasAnyTextTarget()) return;

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        SetMessageText(message);
        SetVisible(true);

        // duration <= 0 means persistent.
        if (duration > 0f)
        {
            activeRoutine = StartCoroutine(HideAfter(duration));
        }
    }

    private IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideImmediate();
        activeRoutine = null;
    }

    private void HideImmediate()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        SetMessageText(string.Empty);
        SetVisible(false);
    }

    private void CreateDefaultOverlay()
    {
        // Creates a simple Screen Space - Overlay canvas with a right-aligned text.
        var canvasGO = new GameObject("TeleportStatusCanvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var textGO = new GameObject("TeleportStatusText");
        textGO.transform.SetParent(canvasGO.transform, false);

        // Prefer TMP if available.
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.alignment = TextAlignmentOptions.MidlineRight;
        tmp.color = Color.white;
        tmp.fontSize = 28;
        tmp.raycastTarget = false;

        var rt = tmp.rectTransform;
        rt.anchorMin = new Vector2(1f, 0.85f);
        rt.anchorMax = new Vector2(1f, 0.85f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.anchoredPosition = new Vector2(-24f, 0f);
        rt.sizeDelta = new Vector2(700f, 80f);

        messageTMP = tmp;
    }

    private bool HasAnyTextTarget()
    {
        return messageTMP != null || messageUGUI != null || messageTextMesh != null;
    }

    private void SetMessageText(string message)
    {
        if (messageTMP != null) messageTMP.text = message;
        if (messageUGUI != null) messageUGUI.text = message;
        if (messageTextMesh != null) messageTextMesh.text = message;
    }

    private void SetVisible(bool visible)
    {
        if (messageTMP != null) messageTMP.enabled = visible;
        if (messageUGUI != null) messageUGUI.enabled = visible;
        if (messageTextMesh != null)
        {
            var r = messageTextMesh.GetComponent<Renderer>();
            if (r != null) r.enabled = visible;
            else messageTextMesh.gameObject.SetActive(visible);
        }
    }

    private void EnsureRightSideLayout()
    {
        // For on-screen UI, we can only position things that have a RectTransform.

        if (messageTMP != null)
        {
            messageTMP.alignment = TextAlignmentOptions.MidlineRight;

            if (messageTMP is TextMeshProUGUI tmpUGUI)
            {
                var rt = tmpUGUI.rectTransform;
                rt.anchorMin = new Vector2(1f, 0.85f);
                rt.anchorMax = new Vector2(1f, 0.85f);
                rt.pivot = new Vector2(1f, 0.5f);
                rt.anchoredPosition = new Vector2(-24f, 0f);

                if (rt.sizeDelta.x <= 0.01f || rt.sizeDelta.y <= 0.01f)
                    rt.sizeDelta = new Vector2(700f, 80f);
            }
        }

        if (messageUGUI != null)
        {
            messageUGUI.alignment = TextAnchor.MiddleRight;

            var rt = messageUGUI.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(1f, 0.85f);
                rt.anchorMax = new Vector2(1f, 0.85f);
                rt.pivot = new Vector2(1f, 0.5f);
                rt.anchoredPosition = new Vector2(-24f, 0f);

                if (rt.sizeDelta.x <= 0.01f || rt.sizeDelta.y <= 0.01f)
                    rt.sizeDelta = new Vector2(700f, 80f);
            }
        }
    }
}
