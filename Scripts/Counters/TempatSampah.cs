using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempatSampah : BaseCounter{

    public override void Interact(Player player) {
        if (player.HasKicthenObject()) {
            player.GetKitchenObject().DestroySelf();
        }
    }

}
