using UnityEngine;
using UnityEngine.Audio;

public class MeleeAudioController : MonoBehaviour
{

    private AudioSource sursa;

    public AudioClip swordAttack;



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

    public void playSwordAttack()
    {
        if (sursa != null && swordAttack != null)
        {
            sursa.PlayOneShot(swordAttack, 0.5f);
        }
    }

  



}
