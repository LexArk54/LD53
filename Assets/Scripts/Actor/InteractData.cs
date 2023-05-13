using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ActorNames {
    public const string Player = "Player";
    public const string Crabherd = "Crabherd";
    public const string Crab = "Crab";
    public const string Fish = "Fish";
    public const string TNT = "TNT";
    public const string PostalBix = "Mail Box";
    public const string HintTable = "Hint";
}

[CreateAssetMenu(menuName = "_Game/ActorData")]
public class InteractData : ScriptableObject {

    public enum Type {
        None,
        All,
        Character,
        Item,
    }

    public new string name;
    public Type type;

    [Header("Hints")]
    [TextArea(3, 3)] public string hintUse;
    [TextArea(3, 3)] public string hintUseInhand;

}
