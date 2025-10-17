using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speed")]
    [Tooltip("The base speed of the player before any carrots are collected.")]
    [SerializeField] private float baseSpeed = 7.0f;

    [Header("Power-up Settings")]
    [Tooltip("The player's speed will be multiplied by this value for each carrot collected.")]
    [SerializeField] private float speedMultiplierPerCarrot = 1.1f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Animator animator;

    private Vector3 currentMovement;
    private float verticalRotation;
    
    public float CurrentSpeed
    {
        get
        {
            // Speed = baseSpeed * (multiplier ^ carrotCount)
            return baseSpeed * Mathf.Pow(speedMultiplierPerCarrot, GameInfo.carrotCount);
        }
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Player Animator component not found. Animations will be disabled.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            if (animator != null)
            {
                 animator.SetBool("isWalking", false);
            }
            return;
        }
        HandleMovement();
        HandleRotation();
        HandleAnimation();
    }
    
    private void HandleAnimation()
    {
        if (animator == null) return;

        Vector2 moveInput = playerInputHandler.MovementInput;
        Vector2 clampedInput = new Vector2(moveInput.x, Mathf.Clamp(moveInput.y, 0f, 1f));

        bool isMoving = characterController.isGrounded && clampedInput.magnitude > 0.1f;

        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            animator.SetFloat("InputX", clampedInput.x);
            animator.SetFloat("InputY", clampedInput.y);
        }
    }

    private Vector3 CalculateWorldDirection()
    {
        Vector2 rawInput = playerInputHandler.MovementInput;

        float forwardInput = Mathf.Clamp(rawInput.y, 0f, 1f);
        
        Vector3 inputDirection = new Vector3(rawInput.x, 0f, forwardInput);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }
}

