using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Extensions;
using CodeBase.Services;
using UnityEngine;
using Zenject;

public class HeroMove : MonoBehaviour
{
    private const float CoyoteDelay = 0.1f;
    private const float StickyTime = 0.05f;
    private const float StickyForce = 9.6f;

    public CharacterController CharacterController;
    [Range(1, 200)] public float MaxForwardSpeed;
    [Range(1, 100)] public float Acceleration = 20.0f;
    [Min(0)] public int MaxJumpsInAir;
    [Min(0)] public float JumpPower;
    public float Gravity;
    public float UseHardLandedOnPower = 5.0f;

    public event Action Jump;
    public event Action AirJump;
    public event Action<float> SpeedChange;
    public event Action Landed;
    public event Action HardLanded;

    private IInputService _inputService;
    private Camera _camera;

    private Vector3 _move;

    private int _jumpsInAir;
    private float _airborneTime;
    private Vector3 _movementInput;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    private void Awake()
    {
        _camera = Camera.main;
        
        if (CharacterController.enabled)
        {
            CharacterController.Move(Vector3.down * 0.01f);
        }
    }

    private void Update()
    {
        if (!IsAirborne())
            ResetAirJumps();
        
        HandleInput();

        HandleMotion();
    }

    private void HandleMotion()
    {
        bool wasGrounded = CharacterController.isGrounded;
        if (!CharacterController.isGrounded)
        {
            ApplyGravity();
            _airborneTime += Time.deltaTime;
        }
        
        Vector3 stickyMove = CalculateStickyMove();
        CharacterController.Move(_move * Time.deltaTime + stickyMove);

        if (CharacterController.isGrounded && !wasGrounded)
        {
            if (_move.y < -UseHardLandedOnPower)
            {
                HardLanded?.Invoke();
            }
            
            Landed?.Invoke();

            _move.y = 0.0f;
            _airborneTime = 0.0f;
        }
        
        if(_movementInput != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementInput);
    }

    /// <summary>
    /// Use a sticky move to make the minifig stay with moving platforms.
    /// </summary>
    private Vector3 CalculateStickyMove()
    {
        return _airborneTime < StickyTime ? Vector3.down * StickyForce * Time.deltaTime : Vector3.zero;
    }

    private bool IsAirborne()
    {
        return _airborneTime >= CoyoteDelay;
    }

    private void ApplyGravity()
    {
        _move.y -= Gravity * Time.deltaTime;
    }

    private void HandleInput()
    {
        _movementInput = CalculatedMovementInput(_inputService.Axis);
        Vector3 directSpeed = CalculateDirectSpeed(_movementInput);

        ApplySpeed(directSpeed);
        
        if (_inputService.IsJumpButtonDown && CanJump())
        {
            ApplyJump();
        }
    }
    
    private void ApplySpeed(Vector3 directSpeed)
    {
        _move.x = directSpeed.x;
        _move.z = directSpeed.z;
        
        SpeedChange?.Invoke(directSpeed.magnitude);
    }

    private bool CanJump()
    {
        return !IsAirborne() || _jumpsInAir > 0;
    }

    private void ApplyJump()
    {
        if (IsAirborne())
        {
            _jumpsInAir--;
            AirJump?.Invoke();
        }
        else
        {
            Jump?.Invoke();
        }
        
        _move.y = JumpPower;
        
        _airborneTime = CoyoteDelay;
    }

    private void ResetAirJumps()
    {
        _jumpsInAir = MaxJumpsInAir;
    }

    private Vector3 CalculateDirectSpeed(Vector3 input)
    {
        Vector3 targetSpeed = input * MaxForwardSpeed;
        return targetSpeed;
        Vector3 directSpeed = new Vector3(_move.x, 0, _move.z);

        Vector3 speedDiff = targetSpeed - directSpeed;

        if (IsAccelerated(speedDiff))
        {
            return targetSpeed;
        }

        if (speedDiff.sqrMagnitude > 0.0f)
        {
            speedDiff.Normalize();

            return directSpeed + speedDiff * Acceleration * Time.deltaTime;
        }

        return directSpeed;
    }

    private bool IsAccelerated(Vector3 speedDiff)
    {
        return speedDiff.sqrMagnitude < Acceleration * Acceleration * Time.deltaTime * Time.deltaTime;
    }

    private Vector3 CalculatedMovementInput(Vector3 axis)
    {
        var right = Vector3.right;
        var forward = Vector3.forward;
        if (_camera)
        {
            right = _camera.transform.right;
            right.y = 0.0f;
            right.Normalize();
            forward = _camera.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();
        }

        var targetSpeed = right * axis.x;
        targetSpeed += forward * axis.y;
        if (targetSpeed.sqrMagnitude > 0.0f)
        {
            targetSpeed.Normalize();
        }

        return targetSpeed;
    }
}