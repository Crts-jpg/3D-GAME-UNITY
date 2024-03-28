using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter {

    public event EventHandler <OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public FryingState state;
        public OnStateChangedEventArgs(FryingState state) {
            this.state = state;
        }
    }

    public enum FryingState {
        Idle,
        Memasak,
        Masak,
        Gosong,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private GosongRecipeSO[] gosongRecipeSOArray;

    private FryingState state;
    private float fryingTimer;
    private float gosongTimer;
    private FryingRecipeSO fryingRecipeSO;
    private GosongRecipeSO gosongRecipeSO;

    private void Start() {
        state = FryingState.Idle;
    }
    private void Update() {
        if (HasKicthenObject()) {
            switch (state) {
                case FryingState.Idle:
                    //Idle();
                    break;
                case FryingState.Memasak:
                    //Memasak();
                    fryingTimer += Time.deltaTime;
                    if (fryingTimer > fryingRecipeSO.fryingTimeMax) {
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = FryingState.Masak;

                        gosongTimer = 0f;
                        gosongRecipeSO = GetGosongResepSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(state));
                    }
                    break;
                case FryingState.Masak:
                    //Masak();
                    gosongTimer += Time.deltaTime;
                    if (gosongTimer > gosongRecipeSO.GosongTimeMax) {
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(gosongRecipeSO.output, this);

                        state = FryingState.Gosong;

                        gosongTimer = 0f;
                    }
                    break;
                case FryingState.Gosong:
                    //Gosong();
                    break;
            }
        }
    }
    public override void Interact(Player player) {
        if (!HasKicthenObject()) {
            // there is no KitchenObject in here

            if (player.HasKicthenObject()) {
                // there is no KitchenObject in here and the player has a KitchenObject
                if (BahanYangBisaDiCut(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingResepSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = FryingState.Memasak;
                    fryingTimer = 0f;
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

                state = FryingState.Idle;
            }
        }

    }

    private bool BahanYangBisaDiCut(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingResepSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO OutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingResepSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null) {
            return fryingRecipeSO.output;
        }
        else {
            return null;
        }
    }

    private FryingRecipeSO GetFryingResepSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private GosongRecipeSO GetGosongResepSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (GosongRecipeSO gosongRecipeSO in gosongRecipeSOArray) {
            if (gosongRecipeSO.input == inputKitchenObjectSO) {
                return gosongRecipeSO;
            }
        }
        return null;
    }

}

