using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerAttack playerAttack;
    
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("References")]
    [SerializeField] private Animator animator;
    private CharacterController characterController;
    
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    
    private Vector2 moveRead;
    private float verticalVelocity;
    private float smoothSpeed = 0f;
    private float smoothFactor = 8f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        moveAction = InputSystem.actions.FindAction("Move");
        runAction = InputSystem.actions.FindAction("Sprint"); 
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Start()
    {
        if (playerAttack == null) playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {

        if(!playerAttack.isAttacking){
        ReadInput();
        HandleMovement();
        }
    }

    private void ReadInput()
    {
        if (playerAttack != null && playerAttack.isAttacking) 
        {
            moveRead = Vector2.zero;
            return; 
        }

        moveRead = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame() && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            animator.SetTrigger("Jump");
        }
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = Vector3.zero;


        if (playerAttack != null && playerAttack.isAttacking)
        {
            animator.SetFloat("Speed", 0f);
            smoothSpeed = 0f;
            
            finalVelocity.y = verticalVelocity;
            characterController.Move(finalVelocity * Time.deltaTime);
            return; // Ieșim din funcție
        }

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 move = (camRight * moveRead.x + camForward * moveRead.y);

        bool isRunning = runAction.IsPressed() && move.magnitude > 0.1f;
        float targetSpeed = move.magnitude * (isRunning ? 2f : 1f);

        smoothSpeed = Mathf.Lerp(smoothSpeed, targetSpeed, Time.deltaTime * smoothFactor);
        animator.SetFloat("Speed", smoothSpeed);

        if (move.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float actualMoveSpeed = isRunning ? runSpeed : walkSpeed;
        finalVelocity = move * actualMoveSpeed;
        finalVelocity.y = verticalVelocity; 

        characterController.Move(finalVelocity * Time.deltaTime);
    }
}