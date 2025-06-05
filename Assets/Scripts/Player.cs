using System;
using UnityEngine;
using UnityEngine.InputSystem; // Adicione este namespace

// Remova o Singleton Instance, pois cada jogador terá seu próprio Player.
// public static Player Instance { get; private set; } // Esta linha deve estar comentada ou removida

public class Player : MonoBehaviour, IKitchenObjectParent {
    // Eventos (permanecem específicos para esta instância de Player)
    public event EventHandler OnPickedSomething;
    public event EventHandler OnDroppedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    // Propriedades de estado
    [SerializeField] private float moveSpeed = 7f;
    // Remova a serialização direta do gameInput, ele será injetado.
    // [SerializeField] private GameInput gameInput; // Esta linha deve estar comentada ou removida
    private GameInput gameInput; // Agora é privado e injetado.

    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    // Novo: Campo para os SelectedCounterVisuals específicos deste jogador
    // Arraste e solte os objetos de visualização de contador que pertencem a este jogador aqui no Inspector.
    [SerializeField] private SelectedCounterVisual[] playerSpecificSelectedCounterVisuals;


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

    // Novo método para inicializar o jogador com seu GameInput.
    public void Initialize(GameInput playerGameInput) {
        gameInput = playerGameInput;
        // Subscrever aos eventos do GameInput específico deste jogador.
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        gameInput.OnPauseAction += GameInput_OnPauseAction; // Adicione o evento de pausa aqui também
        Debug.Log($"Player {gameObject.name} initialized with PlayerInput ID: {gameInput.GetComponent<PlayerInput>().playerIndex}");

        // Inicializar os SelectedCounterVisuals associados a este jogador
        if (playerSpecificSelectedCounterVisuals != null) {
            foreach (SelectedCounterVisual visual in playerSpecificSelectedCounterVisuals) {
                if (visual != null) {
                    visual.Initialize(this); // Passa esta instância do Player para o visual
                }
            }
        }
    }

    private void Awake() {
        // Remova a verificação de Singleton. Esta parte do código deve estar comentada/removida.
        // if (Instance != null) Debug.LogError("There is more than one Player instance");
        // Instance = this;
    }

    // O Start() não é mais necessário aqui para subscrever o gameInput,
    // pois a inicialização será feita via Initialize()
    // private void Start() { } // Comentado ou Removido

    private void OnDestroy() {
        // Garanta que você desinscreva os eventos quando o Player for destruído.
        if (gameInput != null) {
            gameInput.OnInteractAction -= GameInput_OnInteractAction;
            gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
            gameInput.OnPauseAction -= GameInput_OnPauseAction; // Desassinar o evento de pausa
        }

        // Não é necessário desinscrever os SelectedCounterVisuals daqui.
        // O próprio SelectedCounterVisual deve lidar com sua própria destruição
        // e o Player que ele escuta será destruído ao mesmo tempo ou antes,
        // limpando automaticamente o evento.
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
        // Verifique se gameInput foi inicializado antes de usá-lo
        if (gameInput == null) return;

        if (isBeingDragged) {
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
        // AQUI ESTÁ A MUDANÇA: Inverter o sinal do inputVector.y
        Vector3 moveDir = new Vector3(inputVector.x, 0f, -inputVector.y); // <-- INVERTER O SINAL AQUI!

        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove) {
                moveDir = moveDirX;
            }
            else {
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
        Vector3 moveDir = new Vector3(inputVector.x, 0f, -inputVector.y); // <-- Considerar inverter aqui também para a direção de interação

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

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        KitchenGameManager.Instance.TogglePauseGame(); // Assumindo que você tem um método para pausar/despausar no GameManager
    }

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
        // Este evento agora dispara para a instância DESTE jogador do contador selecionado.
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public void SetVisibility(bool isVisible) {
        // Isso pode precisar de um GetComponentInChildren<Renderer>() se o Player tiver sub-objetos com renderers
        Renderer playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null) {
            playerRenderer.enabled = isVisible;
        }
        else {
            foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
                r.enabled = isVisible;
            }
        }
    }

    public bool IsInteractingWithCounterOfType<T>() where T : BaseCounter {
        return selectedCounter != null && selectedCounter is T;
    }
}