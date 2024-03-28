using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour, IkitchenObjectParent {

    public static Player Instance { get; private set; }

    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class SelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float Speed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform KitchenObjectHoldPoint;
    [SerializeField] private LayerMask counterLayermask;


    private KitchenObject kitchenObject;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance in the scene");
        }

        Instance = this;
    }
    


    private void Start() {
        gameInput.OnInteract += GameInput_OnInteract;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteract(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 movePlayer = gameInput.GetMovementVectorNormalized();

        Vector3 movdir = new Vector3(movePlayer.x, 0f, movePlayer.y);

        if(movdir != Vector3.zero) {
            lastInteractDir = movdir;
        }

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit hit, interactionDistance, counterLayermask)) {
            if (hit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (selectedCounter != baseCounter) {
                    SetSelectetCounter(baseCounter);
                }
            }
            else {
                SetSelectetCounter(null);
            }
        } else {
            SetSelectetCounter(null);
        }

        //Debug.Log(selectedCounter);
    }

    private void HandleMovement() {


        Vector2 movePlayer = gameInput.GetMovementVectorNormalized();

        Vector3 movdir = new Vector3(movePlayer.x, 0f, movePlayer.y);

        float moveDistance = Speed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movdir, moveDistance);

        if (!canMove) {
            Vector3 movDirX = new Vector3(movdir.x, 0f, 0f).normalized;
            canMove = movdir.x !=0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDirX, moveDistance);

            if (canMove) {
                movdir = movDirX;
            }
            else {
                Vector3 movDirZ = new Vector3(0f, 0f, movdir.z).normalized;
                canMove = movdir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDirZ, moveDistance);

                if (canMove) {
                    movdir = movDirZ;

                }
            }
        }

        if (canMove) {
            transform.position += movdir * Speed * Time.deltaTime;
        }

        isWalking = movdir != Vector3.zero;

        float rotation = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movdir, Time.deltaTime * rotation);
    }

    private void SetSelectetCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKicthenObject() {
        return kitchenObject != null;
    }
}
