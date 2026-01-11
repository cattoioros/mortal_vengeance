using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed;
    public float changeDirectionTime;
    private Vector3 direction;
    private float timer;
    public float rotationSpeed;
    private Rigidbody rb;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChooseNewDirection();
    }

    void FixedUpdate()
    {
        // move using Rigidbody (collision aware)
        rb.linearVelocity = new Vector3(direction.x * moveSpeed,rb.linearVelocity.y,direction.z * moveSpeed);

        // rotate NPC to face the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation,targetRotation,rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void Update()
    {
        // decrease timer (logic)
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

    void OnCollisionEnter(Collision collision)
    {
        //take the normal of the collision
        Vector3 normal = collision.contacts[0].normal;

        //reflect the current direction using the normal
        direction = Vector3.Reflect(direction, normal).normalized;

        //reset the timer
        timer = changeDirectionTime;
    }

}
