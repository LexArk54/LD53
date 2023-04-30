using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ItemNames {
    public const string Fish = "Fish";
}

[CreateAssetMenu(menuName = "_Game/ItemData")]
public class ItemData : ScriptableObject {

    public new string name;
    public Texture icon;
    public Item prefab;

}
