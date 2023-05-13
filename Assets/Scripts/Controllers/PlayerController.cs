using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActorController {

    [Header("Camera Settings")]
    [SerializeField] private float sensivityScroll = 0.05f;
    [SerializeField] private float zoom = 4f;
    [SerializeField] private float zoomMin = .5f;
    [SerializeField] private float zoomMax = 6f;
    [SerializeField] private float zoomSens = 10f;
    [SerializeField] private Vector3 zoomDirection = new Vector3(0, .1f, -1f);
    [SerializeField] private bool allowFPV = true;

    [SerializeField] private LayerMask collisionMask;

    Transform cameraTransformZ;
    Transform cameraTransformV;
    Transform cameraTransformH;

    float cameraEulerV = 25f;
    float cameraEulerH;

    public bool isFPV { get; private set; } = false;
    public bool isTempFPV { get; private set; } = false;


    public override void Awake() {
        cameraTransformZ = Camera.main.transform;
        cameraTransformV = cameraTransformZ.parent;
        cameraTransformH = cameraTransformV.parent;
        cameraEulerH = transform.eulerAngles.y;
        base.Awake();
    }

    public override void OnActorReseted() {
        base.OnActorReseted();
        UIManager.main.SetEYE(IndicatorColor.None);
        UIManager.main.SetFishCount(0);
        UIManager.main.SetItemHint(string.Empty);
        character.model.animator.ResetTrigger("FallDeath");
        var triggers = FindObjectsByType<DeathTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers) {
            trigger.ResetObject();
        }
        Play();
        character.SetIsDead(false);
    }

    public void Kill(Character enemy, Action callback = null) {
        if (enemy) {
            transform.forward = (enemy.transform.position - transform.position).SetY().normalized;
        }
        Pause();
        character.SetIsDead(true);
        character.model.animator.Play("Death");
        UIManager.main.ScreenFade(() => {
            character.ResetObject();
            foreach (var crab in GameManager.main.crabs) {
                crab.OnActorReseted();
            }
            callback?.Invoke();
            UIManager.main.ScreenShow();
        });
    }

    public void FallDeath() {
        Pause();
        character.SetIsDead(true);
        character.model.animator.SetTrigger("FallDeath");
        UIManager.main.ScreenFade(() => {
            character.ResetObject();
            UIManager.main.ScreenShow();
        });
    }

    private void OnEnable() {
        InputManager.Game.Jump.performed += Jump_performed;
        InputManager.Game.Crouch.performed += Crouch_performed;
        InputManager.Game.Sprint.performed += Sprint_performed;
        InputManager.Game.Drop.performed += Drop_performed;
        InputManager.Game.ESC.performed += ESC_performed;
        InputManager.Game.Use.performed += Use_performed;
        InputManager.UI.ESC.performed += ESC_performed;
    }

    private void OnDisable() {
        InputManager.Game.Jump.performed -= Jump_performed;
        InputManager.Game.Crouch.performed -= Crouch_performed;
        InputManager.Game.Sprint.performed -= Sprint_performed;
        InputManager.Game.Drop.performed -= Drop_performed;
        InputManager.Game.ESC.performed -= ESC_performed;
        InputManager.Game.Use.performed -= Use_performed;
        InputManager.UI.ESC.performed -= ESC_performed;
    }

    private void Use_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isPaused || !character.movement.isGrounded || !character.isUnderControl) return;
        character.interact.InputInteract();
    }

    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isPaused || !character.isUnderControl) return;
        character.movement.InputCrouch(!character.movement.isCrouching);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isPaused || !character.isUnderControl) return;
        character.movement.InputJump();
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isPaused || character.interact.crabIsInHands() || !character.isUnderControl) return;
        character.movement.InputSprint();
    }

    private void ESC_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        UIManager.main.TogglePause();
    }

    private void Drop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isPaused || !character.movement.isGrounded || !character.isUnderControl) return;
        character.interact.InputDrop();
    }

    private void Update() {
        CameraControl();
        if (isPaused || !character.isUnderControl || !Application.isFocused) return;
        CharacterControl();
    }

    void LateUpdate() {
        CameraZoom();
        CameraMove();
    }

    void CameraControl() {
        var input = InputManager.Game.Look.ReadValue<Vector2>() * InputManager.mouseSens;
        cameraEulerH += input.x;
        cameraEulerH = Mathf.Repeat(cameraEulerH, 360f);
        cameraEulerV -= input.y;
        cameraEulerV = Mathf.Clamp(cameraEulerV, -80f, 89f);
        cameraTransformV.localEulerAngles = new Vector3(cameraEulerV, 0, 0);
        cameraTransformH.eulerAngles = new Vector3(0, cameraEulerH, 0);
    }

    void CameraZoom() {
        float mouseWheel = InputManager.Game.Scroll.ReadValue<Vector2>().y * sensivityScroll * Time.deltaTime;
        if (mouseWheel != 0) {
            if (allowFPV && zoom == zoomMin && mouseWheel < 0) SetFPV(false);
            zoom -= zoomSens * mouseWheel;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
            if (allowFPV && zoom == zoomMin) SetFPV(true);
        }
    }


    void SetFPV(bool value) {
        isFPV = value;
        if (value) {
            cameraTransformZ.localPosition = Vector3.zero;
            character.model.SetRenderType(RenderType.OnlyShadow);
        } else {
            character.model.SetRenderType(RenderType.All);
        }
    }

    void SetTempFPV(bool value) {
        isTempFPV = value;
        if (value) {
            cameraTransformZ.localPosition = Vector3.zero;
            character.model.SetRenderType(RenderType.OnlyShadow);
        } else {
            character.model.SetRenderType(RenderType.All);
        }
    }

    void CameraMove() {
        if (isFPV) {
            cameraTransformH.position = character.model.headPoint.position;
        } else {
            float tempZoom = zoom;
            RaycastHit hit;
            var castDir = cameraTransformV.TransformDirection(zoomDirection).normalized;
            if (Physics.SphereCast(cameraTransformV.position, .1f, castDir, out hit, zoom, collisionMask)) {
                tempZoom = hit.distance;
            }
            if (allowFPV) {
                if (isTempFPV) {
                    if (tempZoom > zoomMin) {
                        SetTempFPV(false);
                    }
                } else if (tempZoom <= zoomMin) {
                    SetTempFPV(true);
                }
            }
            if (isTempFPV) {
                cameraTransformZ.position = character.model.headPoint.position;
            } else {
                cameraTransformZ.localPosition = zoomDirection * tempZoom;
            }
        }
        cameraTransformH.position = character.model.headPoint.position;
    }


    private void CharacterControl() {
        var input = InputManager.Game.Move.ReadValue<Vector2>();
        input = Vector2.ClampMagnitude(input, 1f);
        if (input.magnitude > 0) {
            float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            var dir = Quaternion.Euler(0, angle, 0) * cameraTransformH.forward;
            character.movement.InputDirection(dir);
        }
        if (character.movement.isSprinting) {
            if (input.magnitude > 0) {
                input.Normalize();
            }
            float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            var dir = Quaternion.Euler(0, angle, 0) * cameraTransformH.forward;
            character.movement.InputDirection(dir);
            character.movement.InputMove(transform.forward);
        } else {
            character.movement.InputMove(cameraTransformH.forward * input.y + cameraTransformH.right * input.x);
        }
    }

}
