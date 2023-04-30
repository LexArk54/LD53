using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActorComponent {

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


    public void Awake() {
        cameraTransformZ = Camera.main.transform;
        cameraTransformV = cameraTransformZ.parent;
        cameraTransformH = cameraTransformV.parent;
        Init(GetComponent<Actor>());
        actor.Init();
    }

    public void ResetObject() {
        actor.Init();
        actor.model.animator.SetBool("crabInHands", false);
        enabled = true;
    }

    public void Kill() {
        actor.movement.Stop();
        enabled = false;
        UIManager.main.ScreenFade(() => {
            ResetObject();
        });
    }

    public void FallDeath() {
        actor.movement.Stop();
        enabled = false;
        UIManager.main.ScreenFade(() => {
            ResetObject();
        });
    }

    private void OnEnable() {
        InputManager.Game.Jump.performed += Jump_performed;
        InputManager.Game.Crouch.performed += Crouch_performed;
        InputManager.Game.Sprint.performed += Sprint_performed;
        InputManager.Game.Use.performed += Use_performed;
        InputManager.Game.Drop.performed += Drop_performed;
        InputManager.Game.ESC.performed += ESC_performed;
        InputManager.UI.ESC.performed += ESC_performed;
    }

    private void OnDisable() {
        InputManager.Game.Jump.performed -= Jump_performed;
        InputManager.Game.Crouch.performed -= Crouch_performed;
        InputManager.Game.Sprint.performed -= Sprint_performed;
        InputManager.Game.Use.performed -= Use_performed;
        InputManager.Game.Drop.performed -= Drop_performed;
        InputManager.Game.ESC.performed -= ESC_performed;
        InputManager.UI.ESC.performed -= ESC_performed;
    }

    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        actor.movement.InputCrouch(!actor.movement.isCrouching);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        actor.movement.InputJump();
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        actor.movement.InputSprint();
    }

    private void ESC_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        UIManager.main.TogglePause();
        if (UIManager.main.hasActiveElement()) {
            InputManager.ActivateUIMode();
        } else {
            InputManager.ActivateGameMode();
        }
    }

    private void Use_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) => actor.interact.InputUse();
    private void Drop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) => actor.interact.InputDrop();

    private void Update() {
        CameraControl();
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
            actor.model.SetRenderType(RenderType.OnlyShadow);
        } else {
            actor.model.SetRenderType(RenderType.All);
        }
    }

    void SetTempFPV(bool value) {
        isTempFPV = value;
        if (value) {
            cameraTransformZ.localPosition = Vector3.zero;
            actor.model.SetRenderType(RenderType.OnlyShadow);
        } else {
            actor.model.SetRenderType(RenderType.All);
        }
    }

    void CameraMove() {
        if (isFPV) {
            cameraTransformH.position = actor.model.headPoint.position;
        } else {
            float tempZoom = zoom;
            RaycastHit hit;
            var castDir = cameraTransformV.TransformDirection(zoomDirection).normalized;
            if (Physics.Raycast(cameraTransformV.position, castDir, out hit, zoom, collisionMask)) {
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
                cameraTransformZ.position = actor.model.headPoint.position;
            } else {
                cameraTransformZ.localPosition = zoomDirection * tempZoom;
            }
        }
        cameraTransformH.position = actor.model.headPoint.position;
    }


    private void CharacterControl() {
        var input = InputManager.Game.Move.ReadValue<Vector2>();
        input = Vector2.ClampMagnitude(input, 1f);
        if (input.magnitude > 0) {
            float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            var dir = Quaternion.Euler(0, angle, 0) * cameraTransformH.forward;
            actor.movement.InputDirection(dir);
        } else {
            actor.movement.InputDirection(transform.forward);
        }
        if (actor.movement.isSprinting) {
            actor.movement.InputMove(transform.forward * input.magnitude);
        } else {
            actor.movement.InputMove(cameraTransformH.forward * input.y + cameraTransformH.right * input.x);
        }
    }

}
