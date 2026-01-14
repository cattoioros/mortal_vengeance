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
        // Load saved brightness or set to default
        float savedValue = PlayerPrefs.GetFloat(PREF_KEY, 0.5f);
        brightnessSlider.value = savedValue;

        // Initialize alpha values
        currentAlpha = Mathf.Lerp(maxDarkness, 0f, savedValue);
        targetAlpha = currentAlpha;

        SetAlpha(currentAlpha);

        // Link slider to method
        brightnessSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        // Smoothly interpolate current alpha towards target alpha
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.unscaledDeltaTime * 8f);
        SetAlpha(currentAlpha);
    }

    void OnSliderChanged(float value)
    {
        // Update target alpha based on slider value
        targetAlpha = Mathf.Lerp(maxDarkness, 0f, value);
        PlayerPrefs.SetFloat(PREF_KEY, value);
    }

    void SetAlpha(float alpha)
    {
        // Update the overlay color alpha
        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }

    void OnDestroy()
    {
        // Unlink listener to avoid memory leaks
        brightnessSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}
