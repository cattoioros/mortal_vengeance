using UnityEngine;
using UnityEngine.Audio;

public class RangedAudioController : MonoBehaviour
{

    private AudioSource sursa;

    public AudioClip loadBow;

    public AudioClip shootBow;

    public AudioClip deathSound;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        sursa = GetComponent<AudioSource>();

        if (sursa == null)
        {
            sursa = gameObject.AddComponent<AudioSource>();
            sursa.playOnAwake = false;
        }

    }

    public void playLoadBow()
    {
        if (sursa != null && loadBow != null)
        {
            sursa.PlayOneShot(loadBow, 0.5f);
        }
    }

    public void playShootBow()
    {
        if (sursa != null && shootBow != null)
        {
            sursa.PlayOneShot(shootBow, 0.5f);
        }
    }

    public void playDeathSound()
    {
        if (sursa != null &&deathSound != null)
        {
            sursa.PlayOneShot(deathSound, 1f);
        }
    }

    

}
