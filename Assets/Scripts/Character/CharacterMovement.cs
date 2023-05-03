using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : CharacterComponent {

    public Rigidbody rigidBody { get; private set; }
    private CapsuleCollider _collider;

    public Vector3 groundNormal { get; private set; }
    public Vector3 groundNormalForward { get; private set; }
    public float groundAngle { get; private set; }

    private float _slidingPrevPos = 0;
    [HideInInspector] public Vector3 lastGroundPos;
    private float lastGroundTime;



    private Vector3 _targetVelocity;

    private Vector3 _inputMove;
    private Vector3 _inputDirection;
    private float _inputSpeedMod = 1f;
    private float _jumpGroundingDelay = 0;
    public float speedFactor { get; private set; }

    [Header("Показатели")]
    public bool isGrounded = false;
    public bool isCrouching = false;
    public bool isSprinting = false;
    public bool isAirSprinting = false;
    public bool isSliding = false;
    public float sprintValue;
    public float sprintAir;
    public bool isMoving => _targetVelocity != Vector3.zero;

    [Header("Характеристики")]
    public float speedAcceleration = 15f;
    public float maxSpeedValue = .3f;
    public float crouchSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 12f;
    public float jumpForce = 300f;
    public float airSprintTime = 1.5f;

    public float angularAxel = .2f;
    public float angular = 10f;

    [Header("Моды")]
    public float speedModWithCrab = .7f;
    public float angularModWithSprint = .2f;

    [Header("System")]
    public LayerMask groundLayerMask;
    public float groundCheckDistance = .08f;
    public float slideLimit = 45f;
    [SerializeField] private float cauotTime = 0.2f;

    NavMeshPath path;

    public float GetActualSpeedMax() {
        float speed;
        if (isSprinting) {
            speed = sprintSpeed;
        } else if (isCrouching) {
            speed = crouchSpeed;
        } else {
            speed = runSpeed;
        }
        return speed * _inputSpeedMod;
    }

    public override void Init(Character actor) {
        base.Init(actor);
        rigidBody = GetComponent<Rigidbody>();
        _collider = (CapsuleCollider)actor.model.collider;
        Stop();
        if (isCrouching) {
            InputCrouch(false);
        }
        if (isSprinting) {
            StopSprint();
        }
        actor.model.animator.SetBool("isFalling", false);
        actor.model.animator.Play("Idle");
    }

    public void Stop() {
        _inputMove = Vector3.zero;
        _inputDirection = transform.forward;
        rigidBody.velocity = Vector3.zero;
        character.model.animator.SetFloat("speedFactor", 0);
        character.model.animator.SetBool("isSprinting", false);
    }

    public void InputSpeedMod(float value) {
        if (_inputSpeedMod == value) return;
        _inputSpeedMod = value;
    }

    public void InputMove(Vector3 value) {
        if (_inputMove == value) return;
        _inputMove = value;
    }

    public void InputDirection(Vector3 value) {
        if (_inputDirection == value) return;
        _inputDirection = value;
    }

    public void InputCrouch(bool isCrouching) {
        if (!isGrounded || isSprinting) return;
        this.isCrouching = isCrouching;
        character.model.animator.SetBool("isCrouching", isCrouching);
    }

    public void InputJump() {
        if (isSprinting) return;
        if (isGrounded || Time.time - lastGroundTime < cauotTime) {
            if (isCrouching) InputCrouch(false);
            isGrounded = false;
            isSliding = false;
            _SetFriction(0f);
            _jumpGroundingDelay = .2f;
            rigidBody.AddForce(Vector3.up * jumpForce * rigidBody.mass);
            character.model.animator.Play("Jump");
            lastGroundTime = Time.time;
            lastGroundPos = transform.position;
        }
    }

    protected void AirSprintStart() {
        sprintAir = 0;
        rigidBody.useGravity = false;
        rigidBody.velocity = rigidBody.velocity.SetY();
        _targetVelocity = _targetVelocity.SetY();
        transform.position = transform.position.SetY(lastGroundPos.y);
        isAirSprinting = true;
    }
    protected void StopAirSprint() {
        sprintAir = 0;
        rigidBody.useGravity = true;
        isAirSprinting = false;
        //if (!isGrounded && !isSliding) {
        //    Stop();
        //}
    }
    public void MoveTo(Vector3 pos, bool smooth) {
        path = new NavMeshPath();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, Layer.NavMesh.PathFind)) {
            NavMesh.CalculatePath(hit.position, pos, Layer.NavMesh.PathFind, path);
        }
        Vector3 moveDirection;
        if (path.corners.Length < 2) {
            moveDirection = (pos - transform.position).SetY();
        } else {
            moveDirection = (path.corners[1] - transform.position).SetY();
        }
        character.movement.InputDirection(moveDirection.normalized);
        if (smooth) {
            var destinationDirection = (pos - transform.position).SetY();
            moveDirection = transform.forward * (destinationDirection.magnitude / character.movement.runSpeed);
            character.movement.InputMove(Vector3.ClampMagnitude(moveDirection, 1f));
        } else {
            character.movement.InputMove(transform.forward);
        }
        character.movement.InputSprint();
    }

    public void InputSprint() {
        if (!isGrounded && !isAirSprinting) return;
        if (isCrouching) InputCrouch(false);
        sprintValue = maxSpeedValue;
        if (!isSprinting) {
            isSprinting = true;
            character.model.animator.SetBool("isSprinting", true);
            character.model.animator.Play("Sprint");
        }
    }

    protected void SprintToggler() {
        if (isSprinting) {
            sprintValue -= Time.fixedDeltaTime;
            if (isAirSprinting) {
                sprintAir += Time.fixedDeltaTime;
            }
            if (sprintValue <= 0 || _inputMove == Vector3.zero || sprintAir > airSprintTime) {
                StopSprint();
            }
        }
    }

    protected void StopSprint() {
        isSprinting = false;
        character.model.animator.SetBool("isSprinting", false);
        sprintValue = 0;
        if (isAirSprinting) {
            StopAirSprint();
        }
    }

    private void LateUpdate() {
        Rotation();
    }

    void FixedUpdate() {
        Moving();
        GroundCheck();
        SprintToggler();
        if (_inputMove != Vector3.zero) {
            character.model.animator.SetFloat("speedFactor", speedFactor);
        } else {
            character.model.animator.SetFloat("speedFactor", 0);
        }
    }



    void Rotation() {
        float angle = transform.forward.AngleByAxis(_inputDirection, Vector3.up);
        float angularSpeed = Mathf.Clamp(angle * angularAxel, -angular, angular);
        if (isSprinting) angularSpeed *= angularModWithSprint;
        rigidBody.angularVelocity = Vector3.up * angularSpeed;
    }

    void Moving() {
        //if (Vector3.Dot(_inputMove, rigidBody.velocity) < 0) {
        //    speedFactor = Mathf.MoveTowards(speedFactor, 0, 5f * Time.fixedDeltaTime);
        //} else {
        //}
        speedFactor = Mathf.Clamp(rigidBody.velocity.SetY().magnitude, 0, sprintSpeed) / runSpeed;
        float maxSpeed = GetActualSpeedMax();
        if (isAirSprinting) {
            _targetVelocity = _inputMove * maxSpeed;
            rigidBody.AddForce((_targetVelocity - rigidBody.velocity) * speedAcceleration);
        } else if (isGrounded) {
            groundNormalForward = Vector3.ProjectOnPlane(groundNormal, Vector3.Cross(_inputMove, Vector3.up));
            _targetVelocity = Vector3.ProjectOnPlane(_inputMove, groundNormalForward).normalized * _inputMove.magnitude * maxSpeed;
            rigidBody.AddForce((_targetVelocity - rigidBody.velocity) * speedAcceleration);
        } else {
            _targetVelocity = _inputMove * maxSpeed;
            _targetVelocity.y = rigidBody.velocity.y;
            rigidBody.AddForce(_targetVelocity - rigidBody.velocity);
        }
    }

    public bool GroundCast(out RaycastHit hit, float distanceMod = 1) {
        var startPos = transform.position + Vector3.up * _collider.radius * 2f;
        float distance = groundCheckDistance + _collider.radius;
        return Physics.SphereCast(startPos, _collider.radius - .01f, Vector3.down, out hit, distance * distanceMod, groundLayerMask);
    }

    void GroundCheck() {
        if (_jumpGroundingDelay > 0) {
            _jumpGroundingDelay -= Time.fixedDeltaTime;
            return;
        }
        if (GroundCast(out RaycastHit hit, (isSprinting ? 2f : 1f))) {
            groundNormal = hit.normal;
            groundAngle = Vector3.Angle(Vector3.up, groundNormal);
            if (groundAngle > slideLimit) {
                if (Mathf.Abs(_slidingPrevPos - transform.position.y) < Mathf.Epsilon) {
                    _TryGround();
                } else if (!isSliding || isGrounded) {
                    isSliding = true;
                    isGrounded = false;
                    _SetFriction(0f);
                }
                _slidingPrevPos = transform.position.y;
            } else {
                _SetFriction(Mathf.Clamp01(groundAngle / slideLimit - _inputMove.magnitude) * 5f);
                _TryGround();
                if (!isAirSprinting) {
                    lastGroundPos = transform.position;
                    lastGroundTime = Time.time;
                }
            }
        } else {
            if (isGrounded || isSliding) {
                isGrounded = false;
                isSliding = false;
                _SetFriction(0f);
                if (isSprinting && !isAirSprinting) {
                    AirSprintStart();
                }
            }
            character.model.animator.SetBool("isFalling", true);
        }
    }

    void _TryGround() {
        if (isSliding || !isGrounded) {
            isSliding = false;
            isGrounded = true;
            character.model.animator.SetBool("isFalling", false);
            if (isAirSprinting) {
                StopAirSprint();
            }
        }
    }

    void _SetFriction(float friciton) {
        _collider.material.dynamicFriction = friciton;
        _collider.material.staticFriction = friciton;
    }

}