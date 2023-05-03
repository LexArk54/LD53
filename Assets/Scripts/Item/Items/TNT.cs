using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : Item {

    [SerializeField] GameObject explodePrefab;
    [SerializeField] float timerMax = 5f;
    float timer = -1f;

    [SerializeField] Transform destroyable;

    public override void ResetItem() {
        base.ResetItem();
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
        if (Vector3.Distance(destroyable.position, transform.position) < 5f) {
            destroyable.gameObject.SetActive(false);
        }
        ResetItem();
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
