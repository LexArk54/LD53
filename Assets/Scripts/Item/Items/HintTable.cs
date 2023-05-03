using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTable : Item {

    [TextArea(3, 10)]
    public string text;
    public override void OnUse(Character actor) {
        base.OnUse(actor);
        UIManager.main.message.Show(text);
    }

    public override void OnActorRadarExit(Character actor) {
        base.OnActorRadarExit(actor);
        if (actor.isPlayer) {
            UIManager.main.message.Show("");
        }
    }

}
