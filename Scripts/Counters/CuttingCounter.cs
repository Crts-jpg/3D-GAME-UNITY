using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {

    public event EventHandler <CuttingProgressChangedEventArgs> OnCuttingProgressChanged;
    public class CuttingProgressChangedEventArgs : EventArgs {
        public float cuttingProgress;
    }
    public event EventHandler OnCut;

    [SerializeField] private CuttingResepSO[] cuttingResepSOsArray;

    private int cuttingProgress;
    public override void Interact(Player player) {
        if (!HasKicthenObject()) {
            // there is no KitchenObject in here

            if (player.HasKicthenObject()) {
                // there is no KitchenObject in here and the player has a KitchenObject
                if (BahanYangBisaDiCut(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingResepSO cuttingResepSO = GetCuttingResepSOWithInput(GetKitchenObject().GetKitchenObjectSO());


                    OnCuttingProgressChanged?.Invoke(this, new CuttingProgressChangedEventArgs {
                        cuttingProgress = (float) cuttingProgress / cuttingResepSO.cuttingProgressMax 
                    });
                }
                
            }
            else {
                // there is no KitchenObject in here and the player has no KitchenObject
            }
        }
        else {
            // there is a KitchenObject in here
            if (player.HasKicthenObject()) {
                // there is a KitchenObject in here and the player has a KitchenObject
            }
            else {
                // there is a KitchenObject in here and the player has no KitchenObject
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }

    public override void InteractAlternate(Player player) {
        if (HasKicthenObject() && BahanYangBisaDiCut(GetKitchenObject().GetKitchenObjectSO())) {
               // there is a KitchenObject in here and the KitchenObject can be cut
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingResepSO cuttingResepSO = GetCuttingResepSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            
            OnCuttingProgressChanged?.Invoke(this, new CuttingProgressChangedEventArgs
            {
                cuttingProgress = (float)cuttingProgress / cuttingResepSO.cuttingProgressMax
            });
            if (cuttingProgress >= cuttingResepSO.cuttingProgressMax) {
                // the KitchenObject is done being cut
                KitchenObjectSO outputKitchenObjectSO = OutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private bool BahanYangBisaDiCut(KitchenObjectSO inputKitchenObjectSO) {
        CuttingResepSO cuttingResepSO = GetCuttingResepSOWithInput(inputKitchenObjectSO);
        return cuttingResepSO != null;
    }

    private KitchenObjectSO OutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingResepSO cuttingResepSO = GetCuttingResepSOWithInput(inputKitchenObjectSO);
        if (cuttingResepSO != null) {
            return cuttingResepSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingResepSO GetCuttingResepSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingResepSO cuttingResepSO in cuttingResepSOsArray) {
            if (cuttingResepSO.input == inputKitchenObjectSO) {
                return cuttingResepSO;
            }
        }
        return null;
    }   

}