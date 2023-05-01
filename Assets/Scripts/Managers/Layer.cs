using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Layer {

    public static int Default { get; } = LayerMask.NameToLayer("Default");
    public static int TransparentFX { get; } = LayerMask.NameToLayer("TransparentFX");
    public static int IgnoreRaycast { get; } = LayerMask.NameToLayer("Ignore Raycast");
    public static int Player { get; } = LayerMask.NameToLayer("Player");
    public static int Water { get; } = LayerMask.NameToLayer("Water");
    public static int UI { get; } = LayerMask.NameToLayer("UI");
    public static int Actor { get; } = LayerMask.NameToLayer("Actor");
    public static int Radar { get; } = LayerMask.NameToLayer("Radar");
    public static int Item { get; } = LayerMask.NameToLayer("Item");
    public static int Trigger { get; } = LayerMask.NameToLayer("Item");
    public static int Interactive { get; } = LayerMask.NameToLayer("Interactive");




    public static LayerMask GetMask(int[] layers) {
        var mask = new LayerMask();
        foreach (var layer in layers) {
            mask += (1 << layer);
        }
        return mask;
    }

    public static void SetLayerRecursively(this GameObject go, int layer) {
        go.layer = layer;
        foreach (Transform t in go.transform) {
            t.gameObject.SetLayerRecursively(layer);
        }
    }



    public sealed class Mask {

        public static LayerMask playerAttack = GetMask(new int[] {
            Layer.Default,
            Layer.Actor,
        });

    }

    public sealed class NavMesh {

        public const int PathFind = -1;

    }

}