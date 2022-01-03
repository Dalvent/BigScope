using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;

public class CharacterMovement : MonoBehaviour
{
    private const float StickyTime = 0.05f;
    private const float StickyForce = 9.6f;
    private const float CoyoteDelay = 0.1f;

    public CharacterController CharacterController;
    
    public float MaxForwardSpeed = 5f;
    [Range(1, 60)] public float Acceleration = 20.0f;
    [Range(0, 500)] public float MaxRotateSpeed = 150f;
    public float JumpSpeed = 20f;
    public float Gravity = 40f;
    public bool InputEnabled = true;
    public int MaxJumpsInAir = 1;


    private bool _airborne;
    private float _airborneTime;
    private int _jumpsInAir;
    private Vector3 _directSpeed;

    private float _rotateSpeed;
    private Vector3 _moveDelta = Vector3.zero;
    private float _waitedTime = 0.0f;

    private float _externalRotation;
    private Vector3 _externalMotion;

    private Transform _groundedTransform;
    private Vector3 _groundedLocalPosition;
    private Vector3 _oldGroundedPosition;
    private Quaternion _oldGroundedRotation;

    protected virtual void OnValidate()
    {
        MaxForwardSpeed = Mathf.Clamp(MaxForwardSpeed, 5, 30);
    }

    protected virtual void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        if (CharacterController.enabled)
        {
            CharacterController.Move(Vector3.down * 0.01f);
        }
    }

    protected virtual void Update()
    {
        // Handle input.
        if (InputEnabled)
        {
            // Calculate direct speed and speed.
            var right = Vector3.right;
            var forward = Vector3.forward;
            if (Camera.main)
            {
                right = Camera.main.transform.right;
                right.y = 0.0f;
                right.Normalize();
                forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward.Normalize();
            }

            var targetSpeed = right * Input.GetAxisRaw("Horizontal");
            targetSpeed += forward * Input.GetAxisRaw("Vertical");
            if (targetSpeed.sqrMagnitude > 0.0f)
            {
                targetSpeed.Normalize();
            }

            targetSpeed *= MaxForwardSpeed;

            var speedDiff = targetSpeed - _directSpeed;
            if (speedDiff.sqrMagnitude < Acceleration * Acceleration * Time.deltaTime * Time.deltaTime)
            {
                _directSpeed = targetSpeed;
            }
            else if (speedDiff.sqrMagnitude > 0.0f)
            {
                speedDiff.Normalize();

                _directSpeed += speedDiff * Acceleration * Time.deltaTime;
            }

            // Calculate rotation speed - ignore rotate acceleration.
            _rotateSpeed = 0.0f;
            if (targetSpeed.sqrMagnitude > 0.0f)
            {
                var localTargetSpeed = transform.InverseTransformDirection(targetSpeed);
                var angleDiff = Vector3.SignedAngle(Vector3.forward, localTargetSpeed.normalized, Vector3.up);

                if (angleDiff > 0.0f)
                {
                    _rotateSpeed = MaxRotateSpeed;
                }
                else if (angleDiff < 0.0f)
                {
                    _rotateSpeed = -MaxRotateSpeed;
                }

                // Assumes that x > NaN is false - otherwise we need to guard against Time.deltaTime being zero.
                if (Mathf.Abs(_rotateSpeed) > Mathf.Abs(angleDiff) / Time.deltaTime)
                {
                    _rotateSpeed = angleDiff / Time.deltaTime;
                }
            }

            // Calculate move delta.
            _moveDelta = new Vector3(_directSpeed.x, _moveDelta.y, _directSpeed.z);

            // Check if player is grounded.
            if (!_airborne)
            {
                _jumpsInAir = MaxJumpsInAir;
            }

            // Check if player is jumping.
            if (Input.GetButtonDown("Jump"))
            {
                if (!_airborne || _jumpsInAir > 0)
                {
                    if (_airborne)
                    {
                        _jumpsInAir--;
                    }

                    _moveDelta.y = JumpSpeed;

                    _airborne = true;
                    _airborneTime = CoyoteDelay;
                }
            }
        }

        HandleMotion();
    }

    protected void HandleMotion()
    {
        // Handle external motion.
        _externalMotion = Vector3.zero;
        _externalRotation = 0.0f;

        var wasGrounded = CharacterController.isGrounded;

        if (!CharacterController.isGrounded)
        {
            // Apply gravity.
            _moveDelta.y -= Gravity * Time.deltaTime;

            _groundedTransform = null;

            _airborneTime += Time.deltaTime;
        }
        else
        {
            // Apply external motion and rotation.
            if (_groundedTransform && Time.deltaTime > 0.0f)
            {
                var newGroundedPosition = _groundedTransform.TransformPoint(_groundedLocalPosition);
                _externalMotion = (newGroundedPosition - _oldGroundedPosition) / Time.deltaTime;
                _oldGroundedPosition = newGroundedPosition;

                var newGroundedRotation = _groundedTransform.rotation;
                // FIXME Breaks down if rotating more than 180 degrees per frame.
                var diffRotation = newGroundedRotation * Quaternion.Inverse(_oldGroundedRotation);
                var rotatedRight = diffRotation * Vector3.right;
                rotatedRight.y = 0.0f;
                if (rotatedRight.magnitude > 0.0f)
                {
                    rotatedRight.Normalize();
                    _externalRotation = Vector3.SignedAngle(Vector3.right, rotatedRight, Vector3.up) / Time.deltaTime;
                }

                _oldGroundedRotation = newGroundedRotation;
            }
        }

        // Move minifig - check if game object was made inactive in some callback to avoid warnings from CharacterController.Move.
        if (gameObject.activeInHierarchy)
        {
            // Use a sticky move to make the minifig stay with moving platforms.
            var stickyMove = _airborneTime < StickyTime ? Vector3.down * StickyForce * Time.deltaTime : Vector3.zero;
            CharacterController.Move((_moveDelta + _externalMotion) * Time.deltaTime + stickyMove);
        }

        // If becoming grounded by this Move, reset y movement and airborne time.
        if (!wasGrounded && CharacterController.isGrounded)
        {
            // Play landing sound if landing sufficiently hard.
            if (_moveDelta.y < -5.0f)
            {
            }

            _moveDelta.y = 0.0f;
            _airborneTime = 0.0f;
        }

        // Update airborne state.
        _airborne = _airborneTime >= CoyoteDelay;

        // Rotate minifig.
        transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0);
        transform.RotateAround(_oldGroundedPosition, Vector3.up, _externalRotation * Time.deltaTime);
    }

    public void TeleportTo(Vector3 position)
    {
        CharacterController.enabled = false;
        transform.position = position;
        CharacterController.enabled = true;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (CharacterController.isGrounded)
        {
            RaycastHit raycastHit;
            if (Physics.SphereCast(transform.position + Vector3.up * 0.8f, 0.8f, Vector3.down, out raycastHit, 0.1f, -1, QueryTriggerInteraction.Ignore))
            {
                _groundedTransform = raycastHit.collider.transform;
                _oldGroundedPosition = raycastHit.point;
                _groundedLocalPosition = _groundedTransform.InverseTransformPoint(_oldGroundedPosition);
                _oldGroundedRotation = _groundedTransform.rotation;
            }
        }
    }
}