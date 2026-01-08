using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
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

        // Dezactivăm Root Motion pentru a folosi controlul prin script
        animator.applyRootMotion = false;

        // Mapare acțiuni din Input System
        moveAction = InputSystem.actions.FindAction("Move");
        runAction = InputSystem.actions.FindAction("Sprint"); 
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
    }

    private void ReadInput()
    {
        moveRead = moveAction.ReadValue<Vector2>();

        // Logică Săritură (fizică + animație)
        if (jumpAction.WasPressedThisFrame() && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            animator.SetTrigger("Jump");
            Invoke(nameof(ExecuteJumpForce), 0f);
        }
    }

    private void ExecuteJumpForce()
{
    verticalVelocity = jumpForce;
}

    private void HandleMovement()
    {
        // 1. Calculăm direcția față de camera principală
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 move = (camRight * moveRead.x + camForward * moveRead.y);

        // 2. Calculăm viteza pentru Animator (0 = Idle, 1 = Walk, 2 = Run)
        bool isRunning = runAction.IsPressed() && move.magnitude > 0.1f;
        float targetSpeed = move.magnitude * (isRunning ? 2f : 1f);

        // Interpolare pentru tranziții fine în Blend Tree
        smoothSpeed = Mathf.Lerp(smoothSpeed, targetSpeed, Time.deltaTime * smoothFactor);
        animator.SetFloat("Speed", smoothSpeed);

        // 3. Rotație lină către direcția de mers
        if (move.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. Gravitație și mișcare fizică
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Mică forță în jos pentru stabilitate pe sol
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        float actualMoveSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 finalVelocity = move * actualMoveSpeed;
        finalVelocity.y = verticalVelocity;

        characterController.Move(finalVelocity * Time.deltaTime);
    }
}