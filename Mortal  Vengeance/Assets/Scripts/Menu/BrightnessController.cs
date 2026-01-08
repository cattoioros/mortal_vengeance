using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0.5f, 0.95f)]
    private float maxDarkness = 0.85f;

    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private Slider brightnessSlider;

    private const string PREF_KEY = "Brightness";

    private float targetAlpha;
    private float currentAlpha;

    void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(PREF_KEY, 0.5f);
        brightnessSlider.value = savedValue;

        currentAlpha = Mathf.Lerp(maxDarkness, 0f, savedValue);
        targetAlpha = currentAlpha;

        SetAlpha(currentAlpha);

        brightnessSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.unscaledDeltaTime * 8f);
        SetAlpha(currentAlpha);
    }

    void OnSliderChanged(float value)
    {
        targetAlpha = Mathf.Lerp(maxDarkness, 0f, value);
        PlayerPrefs.SetFloat(PREF_KEY, value);
    }

    void SetAlpha(float alpha)
    {
        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }

    void OnDestroy()
    {
        brightnessSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}
