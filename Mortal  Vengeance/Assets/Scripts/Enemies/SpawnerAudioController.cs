using UnityEngine;
using UnityEngine.Audio;

public class SpawnerAudioController : MonoBehaviour
{

    private AudioSource sursa;

    public AudioClip spawnMinion;

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

    public void playSpawnMinion()
    {
        if (sursa != null && spawnMinion != null)
        {
            sursa.PlayOneShot(spawnMinion, 0.5f);
        }
    }

    public void playDeathSound()
    {
        if (sursa != null && deathSound != null)
        {
            sursa.PlayOneShot(deathSound, 1f);
        }
    }
}
