using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorRadar : ActorComponent {

    [Header("Автоматические списки")]
    public List<Actor> actors = new List<Actor>();
    public List<Item> items = new List<Item>();

    public Item GetNearestItem(string name = null) => GetNearest(items, name);
    public Actor GetNearestActor(string name = null) => GetNearest(actors, name);
    public ActorBase GetNearest() {
        ActorBase result = null;
        float distance = float.MaxValue;
        float temp;
        foreach (var item in actors) {
            temp = Vector3.Distance(transform.position, item.transform.position);
            if (temp < distance) {
                result = item;
                distance = temp;
            }
        }
        foreach (var item in items) {
            temp = Vector3.Distance(transform.position, item.transform.position);
            if (temp < distance) {
                result = item;
                distance = temp;
            }
        }
        return result;
    }

    public float GetRadius() {
        return GetComponent<SphereCollider>().radius;
    }

    public T GetNearest<T>(List<T> list, string name = null) where T : ActorBase {
        if (list.Count == 0) return null;
        T result = null;
        float distance = float.MaxValue;
        float temp;
        foreach (var item in list) {
            temp = Vector3.Distance(transform.position, item.transform.position);
            if (temp < distance) {
                if (name != null && item.data.name != name) {
                    continue;
                } 
                result = item;
                distance = temp;
            }
        }
        return result;
    }

    private void Awake() {
        enabled = false;
    }

    public override void Init(Actor actor) {
        base.Init(actor);
        gameObject.SetActive(false);
        actors.Clear();
        items.Clear();
        gameObject.SetActive(true);
        enabled = true;
    }


    public event Action<Actor> OnEnter;
    protected virtual void ActorEnter(Actor actor) {
        var index = actors.IndexOf(actor);
        if (index == -1) {
            OnEnter?.Invoke(actor);
            actors.Add(actor);
            actor.OnTriggeringDisable += ActorExit;
            //Debug.Log(Time.time + " actor " + actor.name + " enter");
        }
    }

    public event Action<Actor> OnExit;
    protected virtual void ActorExit(Actor actor) {
        var index = actors.IndexOf(actor);
        if (index > -1) {
            actors.RemoveAt(index);
            actor.OnTriggeringDisable -= ActorExit;
            OnExit?.Invoke(actor);
            //Debug.Log(Time.time + " actor " + actor.name + " exit");
        }
    }


    public event Action<Item> OnItemEnter;
    protected virtual void ItemEnter(Item item) {
        var index = items.IndexOf(item);
        if (index == -1) {
            OnItemEnter?.Invoke(item);
            items.Add(item);
            item.OnTriggeringDisable += ItemExit;
            item.OnRadarIn(actor);
            //Debug.Log(Time.time + " actor " + actor.name + " enter");
        }
    }

    public event Action<Item> OnItemExit;
    protected virtual void ItemExit(Item item) {
        var index = items.IndexOf(item);
        if (index > -1) {
            items.RemoveAt(index);
            item.OnTriggeringDisable -= ItemExit;
            OnItemExit?.Invoke(item);
            item.OnRadarOut(actor);
            //Debug.Log(Time.time + " actor " + actor.name + " exit");
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag == Tag.Actor) {
            ActorEnter(collision.GetComponentInParent<Actor>());
            return;
        }
        if (collision.tag == Tag.Item) {
            ItemEnter(collision.GetComponentInParent<Item>());
            return;
        }
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.tag == Tag.Actor) {
            ActorExit(collision.GetComponentInParent<Actor>());
            return;
        }
        if (collision.tag == Tag.Item) {
            ItemExit(collision.GetComponentInParent<Item>());
            return;
        }
    }

}
