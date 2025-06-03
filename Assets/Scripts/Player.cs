using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
    public static Player Instance { get; private set; }

    // Eventos
    public event EventHandler OnPickedSomething;
    public event EventHandler OnDroppedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    // Propriedades de estado
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    public bool IsInsideFridge { get; set; }
    public void EnterFridge() => IsInsideFridge = true;
    public void ExitFridge() => IsInsideFridge = false;


    private bool isBeingDragged = false;
    private Transform dragDestination;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private bool isHoldingAndAnimated = false;
    private bool isHostage = false;

    private void Awake() {
        if (Instance != null) Debug.LogError("There is more than one Player instance");
        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    // --- Métodos para controle de estado ---
    public bool IsWalking() {
        return isWalking && !isHoldingAndAnimated && !isHostage;
    }

    public bool IsHoldingAndAnimated() {
        return isHoldingAndAnimated;
    }

    public void SetHoldingAndAnimated(bool state) {
        isHoldingAndAnimated = state;
    }

    public void SetAsHostage(bool hostageState) {
        isHostage = hostageState;
        if (isHostage) {
            Debug.Log("Jogador feito refém!");
        }
    }

    // --- Lógica principal ---
    public void StartDragging(Transform destination) {
        isBeingDragged = true;
        isHostage = true;
        dragDestination = destination;
    }

    public void StopDragging() {
        isBeingDragged = false;
        isHostage = false;
    }

    private void Update() {
        if (isBeingDragged) {
            // Suaviza o movimento do jogador sendo arrastado
            transform.position = Vector3.Lerp(
                transform.position,
                dragDestination.position,
                Time.deltaTime * 3f
            );
        }

        if (isHostage) {
            isWalking = false;
            return;
        }

        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement() {

        if (isHoldingAndAnimated || isHostage) {
            isWalking = false;
            return;
        }

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            // Tentativa de movimento na horizontal
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove) {
                moveDir = moveDirX;
            }
            else {
                // Tentativa de movimento na vertical
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove) {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        if (isWalking) {
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
    }

    private void HandleInteractions() {
        if (isHoldingAndAnimated || isHostage) return;

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            }
            else {
                SetSelectedCounter(null);
            }
        }
        else {
            SetSelectedCounter(null);
        }
    }

    // --- Métodos da interface IKitchenObjectParent ---
    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
        OnDroppedSomething?.Invoke(this, EventArgs.Empty);
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public void SetVisibility(bool isVisible) {
        GetComponent<Renderer>().enabled = isVisible; // Desliga/renderização
        // Ou: usar um efeito de transparência (ex: shader de "invisível")
    }


    // --- Interação com counters ---
    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying() || isHoldingAndAnimated || isHostage) return;

        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying() || isHoldingAndAnimated || isHostage) return;

        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    public bool IsInteractingWithCounterOfType<T>() where T : BaseCounter {
        return selectedCounter != null && selectedCounter is T;
    }
}