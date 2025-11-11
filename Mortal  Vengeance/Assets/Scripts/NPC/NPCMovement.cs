using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed;
    public float changeDirectionTime;
    private Vector3 direction;
    private float timer;
    
    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        //move NPC in the current direction
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        //decrese timer
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ChooseNewDirection();
        }
    }

    //choose a new random direction for the NPC to move in & reset the timer
    void ChooseNewDirection()
    {
        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized; 
        timer = changeDirectionTime;
    }
}
