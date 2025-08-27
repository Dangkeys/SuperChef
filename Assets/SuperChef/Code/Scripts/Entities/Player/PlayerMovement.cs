using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AutoInjectOnAwake), typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Components")]
    CharacterController _characterController;

    [Header("Player Movement")]
    public float moveSpeed = 7.5f;
    public float jumpHeight = 2.0f;

    [Header("Camera Look")]
    public float lookSpeed = 0.12f;
    public float lookXLimit = 60.0f;

    [Header("Physics")]
    public float gravity = -15.0f;

    private Transform cameraTransform;

    private GameInputReader _inputReader;
    [field: SerializeField] public Transform PlayerEyesTransform;


    [Inject]
    private void Init(GameInputReader inputReader)
    {
        _inputReader = inputReader;
    }

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.MoveEvent += OnMove;
        _inputReader.JumpEvent += OnJump;
        _inputReader.LookEvent += OnLook;

        Camera mainCamera = Camera.main;
        mainCamera.transform.SetParent(PlayerEyesTransform);
        mainCamera.transform.localPosition = Vector3.zero;
        cameraTransform = mainCamera.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private Vector3 playerVelocity;
    private bool isGrounded;
    private float rotationX = 0;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void OnMove(Vector2 vector)
    {
        moveInput = vector;
    }
    private void OnJump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void OnLook(Vector2 look)
    {
        lookInput = look;
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleGrounded();
        HandleMovement();
        HandleGravity();
        HandleCameraLook();
    }

    private void HandleGrounded()
    {
        isGrounded = _characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        _characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        _characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleCameraLook()
    {
        float mouseX = lookInput.x * lookSpeed;
        float mouseY = lookInput.y * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    public override void OnDestroy()
    {
        if (!IsOwner) return;
        _inputReader.MoveEvent -= OnMove;
        _inputReader.JumpEvent -= OnJump;
        _inputReader.LookEvent -= OnLook;
    }
}