using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FishRadar : MonoBehaviour {

    [SerializeField] private Fish fish;

    [Header("Автоматические списки")]
    public int inRadarCount;

    private void Awake() {
        gameObject.SetActive(false);
    }

    protected virtual void Enter(Interactive actor) {
        if (actor is Fish) {
            inRadarCount++;
            actor.OnTriggeringDisable += Exit;
            fish.OnFishRadarEnter(inRadarCount);
        }
    }

    protected virtual void Exit(Interactive actor) {
        if (actor is Fish) {
            inRadarCount--;
            actor.OnTriggeringDisable -= Exit;
            fish.OnFishRadarExit(inRadarCount);
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
