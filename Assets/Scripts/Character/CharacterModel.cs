using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public enum RenderType {
    None,
    OnlyModel,
    OnlyShadow,
    All,
}

public class CharacterModel : CharacterComponent {

    public Transform headPoint;
    public Transform armPoint;

    public new Collider collider { get; protected set; }
    public Animator animator { get; protected set; }

    private Renderer[] renderers;
    private float prevAnimSpeed;

    public GameObject sprintFootsContainer;


    public override void Init(Character actor) {
        base.Init(actor);
        animator = GetComponentInChildren<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
        collider = GetComponent<Collider>();
        StopSprint();
    }

    public void StartSprint() {
        if (sprintFootsContainer) sprintFootsContainer.SetActive(true);
    }
    public void StopSprint() {
        if (sprintFootsContainer) sprintFootsContainer.SetActive(false);
    }

    public void AnimationPause() {
        if (animator.speed == 0) return;
        prevAnimSpeed = animator.speed;
        animator.speed = 0;
    }

    public void AnimationPlay() {
        if (animator.speed != 0) return;
        animator.speed = prevAnimSpeed;
    }

    public void SetRenderType(RenderType value) {
        if (value == RenderType.None) {
            foreach (var renderer in renderers) {
                renderer.enabled = false;
            }
        } else {
            var shadowMode = ShadowCastingMode.On;
            switch (value) {
                case RenderType.OnlyModel: shadowMode = ShadowCastingMode.Off; break;
                case RenderType.OnlyShadow: shadowMode = ShadowCastingMode.ShadowsOnly; break;
                case RenderType.All: shadowMode = ShadowCastingMode.On; break;
            }
            foreach (var renderer in renderers) {
                renderer.enabled = true;
                renderer.shadowCastingMode = shadowMode;
            }
        }
    }
}
