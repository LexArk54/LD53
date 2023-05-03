using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponent : MonoBehaviour {

    [HideInInspector] public Character character;

    public virtual void Init(Character character) {
        this.character = character;
    }

}
