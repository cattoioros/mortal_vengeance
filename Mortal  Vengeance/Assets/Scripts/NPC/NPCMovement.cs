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
        Vector3 desiredVelocity = direction * moveSpeed;
        desiredVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, 0.2f);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime)
            );
        }
    }

    void Update()
    {
        // decrease timer
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
        Vector3 normal = collision.contacts[0].normal;

        //remove velocity component towards the normal
        Vector3 velocity = rb.linearVelocity;
        velocity = Vector3.ProjectOnPlane(velocity, normal);
        rb.linearVelocity = velocity;

        //reflect direction
        direction = Vector3.Reflect(direction, normal).normalized;

        timer = changeDirectionTime;
    }


}
