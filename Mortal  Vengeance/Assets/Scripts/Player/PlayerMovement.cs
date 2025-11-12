using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputAction moveAction;

    Vector2 moveRead;

    [SerializeField]
    public float speed = 5;
    public float rotationSpeed = 0.1f;


    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {

        ReadInput();
        Movement();

    }

    private void ReadInput()
    {
        moveRead = new Vector2(moveAction.ReadValue<Vector2>().x, moveAction.ReadValue<Vector2>().y);
    }
    

    private void Movement()
    {
        
    Vector3 camForward = Camera.main.transform.forward;
    Vector3 camRight = Camera.main.transform.right;

    camForward.y = 0;
    camRight.y = 0;

    camForward.Normalize();
    camRight.Normalize();

    Vector3 move = camRight * moveRead.x + camForward * moveRead.y;

    transform.position += move * speed * Time.deltaTime;


    if (move.magnitude > 0.1f)
    {
        Vector3 direction = move.normalized;
        transform.forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
    }
    


    

    }
}
