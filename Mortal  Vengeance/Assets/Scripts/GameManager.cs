using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    public Transform PlayerTransform { get; private set; }
    private void Awake()
    {

        if(instance !=null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;

        }

        //Getting the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");



        if (playerObj != null)
        {
            PlayerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player nu a fost gasit.");
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
