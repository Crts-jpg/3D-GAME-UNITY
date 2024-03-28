using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;    


    public override void Interact(Player player) {
        if (!HasKicthenObject()) {
            // there is no KitchenObject in here

            if (player.HasKicthenObject()) {
                // there is no KitchenObject in here and the player has a KitchenObject
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                // there is no KitchenObject in here and the player has no KitchenObject
            }
        } else {
            // there is a KitchenObject in here
            if (player.HasKicthenObject()) {
                // there is a KitchenObject in here and the player has a KitchenObject
            } else {
                // there is a KitchenObject in here and the player has no KitchenObject
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}