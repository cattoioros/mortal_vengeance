using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private Slider brightnessSlider;

    private const string PREF_KEY = "Brightness";

    void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(PREF_KEY, 0.3f);
        brightnessSlider.value = savedValue;
        ApplyBrightness(savedValue);

        brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
    }

    void ApplyBrightness(float value)
    {
        float alpha = Mathf.Clamp01(value);

        Color color = brightnessOverlay.color;
        color.a = alpha;
        brightnessOverlay.color = color;

        PlayerPrefs.SetFloat(PREF_KEY, value);
    }
}
