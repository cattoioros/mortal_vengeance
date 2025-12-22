using NUnit.Framework.Interfaces;
using UnityEngine;

public class BossLightHalfHealth : MonoBehaviour
{


    private Light lumina;

    [SerializeField] private float intensitateNormala = 100f;

    [SerializeField] private float intensitateMaxima = 1000f;

    [SerializeField] private float timpCrestereIntensitate = 5f;

    private float startTime;

    void Start()
    {
        lumina = GetComponent<Light>();

        lumina.intensity = 0;

        if (lumina == null)
        {
            Debug.LogError("Nu exista componenta luminca");
            return;
        }

    }

    public void StopDescent()
    {
        startTime = 0f;

        lumina.intensity = 0f; 
    }

    public void StartDescent()
    {
        startTime = Time.time;

        lumina.intensity = intensitateNormala;
    }

    // Update is called once per frame
    void Update()
    {

        if (startTime > 0)
        {
            float timeElapsed = Time.time - startTime;
            float t = timeElapsed / timpCrestereIntensitate;

            

            float intensitateCurenta = Mathf.Lerp(intensitateNormala, intensitateMaxima, t);
            lumina.intensity = intensitateCurenta;

            if (t >= 1f)
            {
                lumina.intensity = intensitateMaxima;
                startTime = 0;
            }
        }
    }
}
