using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : NetworkBehaviour
{
    public Transform weaponHolderTransform;
    
    private Vector3 _velocity;
    private bool _jumpPressed;
    private bool _canMove;

    public bool isHelpingTeleport = false;
    public Transform targetTransform;

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }
    
    private CharacterController _controller;

    [FormerlySerializedAs("PlayerSpeed")] public float playerSpeed = 2f;

    [FormerlySerializedAs("JumpForce")] public float jumpForce = 5f;
    [FormerlySerializedAs("GravityValue")] public float gravityValue = -9.81f;

    [FormerlySerializedAs("Camera")] public Camera mainCamera;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        
    }
    
    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.GetComponent<FirstPersonCamera>().Target = transform;
            
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isHelpingTeleport)
        {
            transform.position = targetTransform.position;
            transform.rotation = Quaternion.Euler(0, targetTransform.rotation.y, 0);
            if (transform.position == targetTransform.position) isHelpingTeleport = true;
        }
        // FixedUpdateNetwork is only executed on the StateAuthority
        if (!_canMove) return;
        
        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        var cameraRotationY = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0);

        var move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * Runner.DeltaTime * playerSpeed;

        _velocity.y += gravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += jumpForce;
        }
        
        _controller.Move(move + _velocity * Runner.DeltaTime);
        
        transform.rotation = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0);

        Vector3 weaponRotation = new Vector3(mainCamera.transform.eulerAngles.x, transform.eulerAngles.y, 0);
        
        weaponHolderTransform.rotation = Quaternion.Euler(weaponRotation);
        
        _jumpPressed = false;
    }
    
   
}