using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRadar : CharacterComponent {

    [Header("Автоматические списки")]
    public List<Interactive> interactives = new List<Interactive>();

    public enum HandState {
        None,
        All,
        NotInHand,
        InHand,
    }

    public Interactive GetNearest(InteractData.Type type = InteractData.Type.All, HandState handState = HandState.All) {
        if (interactives.Count == 0) return null;
        Interactive result = null;
        float distance = float.MaxValue;
        float temp;
        foreach (var iobject in interactives) {
            if (type != InteractData.Type.All && iobject.data.type != type) {
                continue;
            }
            if (handState != HandState.All) {
                if (iobject.hander) {
                    if (handState == HandState.NotInHand) continue;
                } else {
                    if (handState == HandState.InHand) continue;
                }
            }
            temp = Vector3.Distance(transform.position, iobject.transform.position);
            if (temp < distance) {
                result = iobject;
                distance = temp;
            }
        }
        return result;
    }
    public Interactive GetNearest(string name = null, HandState handState = HandState.All) {
        if (interactives.Count == 0) return null;
        Interactive result = null;
        float distance = float.MaxValue;
        float temp;
        foreach (var iobject in interactives) {
            if (handState != HandState.All) {
                if (iobject.hander) {
                    if (handState == HandState.NotInHand) continue;
                } else {
                    if (handState == HandState.InHand) continue;
                }
            }
            temp = Vector3.Distance(transform.position, iobject.transform.position);
            if (temp < distance) {
                if (name != null && iobject.data.name != name) {
                    continue;
                }
                result = iobject;
                distance = temp;
            }
        }
        return result;
    }

    public float GetRadius() {
        return GetComponent<SphereCollider>().radius;
    }

    private void Awake() {
        enabled = false;
    }

    public override void Init(Character character) {
        base.Init(character);
        gameObject.SetActive(false);
        interactives.Clear();
        gameObject.SetActive(true);
        enabled = true;
    }


    public event Action<Interactive> OnEnter;
    protected virtual void Enter(Interactive interactive) {
        var index = interactives.IndexOf(interactive);
        if (index == -1) {
            interactive.OnActorRadarEnter(character);
            OnEnter?.Invoke(interactive);
            interactives.Add(interactive);
            interactive.OnTriggeringDisable += Exit;
        }
    }

    public event Action<Interactive> OnExit;
    protected virtual void Exit(Interactive interactive) {
        var index = interactives.IndexOf(interactive);
        if (index > -1) {
            interactives.RemoveAt(index);
            interactive.OnTriggeringDisable -= Exit;
            OnExit?.Invoke(interactive);
            interactive.OnActorRadarExit(character);
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag == Tag.Interactive) {
            Enter(collision.GetComponentInParent<Interactive>());
            return;
        }
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.tag == Tag.Interactive) {
            Exit(collision.GetComponentInParent<Interactive>());
            return;
        }
    }

}
