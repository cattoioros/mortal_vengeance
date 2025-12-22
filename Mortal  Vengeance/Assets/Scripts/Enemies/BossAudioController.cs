using UnityEngine;
using UnityEngine.Audio;

public class BossAudioController : MonoBehaviour
{

    private AudioSource sursa;

    public AudioClip explosionSound;

    public AudioClip groundExplodeSound;

    public AudioClip meteorsFlyingSound;

    public AudioClip descentSound;

    public AudioClip descentCrashSound;

    public AudioClip swingSwordSound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        sursa = GetComponent<AudioSource>();

        if(sursa == null)
        {
            sursa = gameObject.AddComponent<AudioSource>();
            sursa.playOnAwake = false;
        }
        
    }

    public void playExplosionCue()
    {
        if (sursa != null && explosionSound != null)
        {
            sursa.PlayOneShot(explosionSound, 0.5f);
        }
    }

    public void GroundExplodeCue()
    {
        if(sursa != null && groundExplodeSound != null)
        {
            sursa.PlayOneShot(groundExplodeSound, 0.5f);
        }
    }

    public void playMeteorsCue()
    {
        if(sursa!=null && meteorsFlyingSound != null)
        {
            sursa.PlayOneShot(meteorsFlyingSound, 0.5f);
        }
    }

    public void playDescentSound()
    {
        if(sursa!=null && descentSound!= null)
        {
            sursa.PlayOneShot(descentSound, 0.5f);
        }
    }

    public void playDescentCrash()
    {
        if(sursa!=null && descentCrashSound!= null)
        {
            sursa.PlayOneShot(descentCrashSound,0.5f);
        }
    }

    public void playSwordSwing()
    {
        if(sursa!=null && swingSwordSound!=null)
        {
            sursa.PlayOneShot(swingSwordSound, 6f);
        }
    }

}
