using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : Item {

    [SerializeField] GameObject explodePrefab;
    [SerializeField] float explodeDistance = 5f;
    [SerializeField] float timerMax = 5f;
    float timer = -1f;

    [SerializeField] Transform destroyable;

    public override void ResetObject() {
        base.ResetObject();
        timer = -1f;
    }


    public override Item OnTake(Character actor) {
        base.OnTake(actor);
        timer = -1f;
        return this;
    }

    public override void OnUse(Character actor) {
        base.OnUse(actor);
        timer = timerMax;
        actor.interact.PlaceItem();
    }

    IEnumerator _ExplodeDelay() {
        var explode = Instantiate(explodePrefab, transform.position, transform.rotation);
        Destroy(explode, 3f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(.3f);
        if (Vector3.Distance(transform.position, destroyable.position) < explodeDistance) {
            destroyable.gameObject.SetActive(false);
        }
        ResetObject();
    }

    private void FixedUpdate() {
        if (timer > 0) {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0) {
                GameManager.main.StartCoroutine(_ExplodeDelay());
            }
        }
    }

}
