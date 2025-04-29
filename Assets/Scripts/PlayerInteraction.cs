using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuração")]
    public float interactionRange = 2.0f;
    public LayerMask interactionLayer;
    public LayerMask foodLayer;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode pickupKey = KeyCode.F;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public Transform handTransform;

    [Header("Debug")]
    [SerializeField] private Interactable currentInteractable;
    [SerializeField] private Animatable currentAnimatable;
    [SerializeField] private GameObject heldItem; // Referência ao GameObject segurado
    private Vector3 originalWorldPosition; // Posição original do item pego
    private Quaternion originalWorldRotation; // Rotação original do item pego

    [Header("Configuração de Segurar Item")]
    public Vector3 holdPositionOffset = new Vector3(0.1f, 0.1f, 0.2f);
    public Vector3 holdRotationOffset = new Vector3(0, 0, 0);
    public float dropForwardForce = 1.5f;
    public float dropUpwardForce = 2f;

    void Update()
    {
        CheckInteractables();
        HandlePickup();
    }

    void CheckInteractables()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, interactionRange, interactionLayer);

        if (interactables.Length > 0)
        {
            currentInteractable = interactables[0].GetComponent<Interactable>();
            currentAnimatable = interactables[0].GetComponent<Animatable>();

            if (currentInteractable != null)
            {
                ShowInteractionUI(true, currentInteractable.GetInteractionMessage());
                return;
            }

            if (currentAnimatable != null)
            {
                ShowInteractionUI(true, currentAnimatable.GetInteractionMessage());
                return;
            }
        }
        else
        {
            currentInteractable = null;
            currentAnimatable = null;
            ShowInteractionUI(false);
        }
    }

    void HandlePickup()
    {
        // Tecla E para interações
        if (Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Tecla E pressionada");
            if (currentInteractable != null) currentInteractable.Interact();
            if (currentAnimatable != null) currentAnimatable.Interact();
        }

        // Tecla F para pegar/soltar
        if (Input.GetKeyDown(pickupKey))
        {
            Debug.Log("Tecla F pressionada");
            if (heldItem == null)
            {
                Debug.Log("Tentando pegar item...");
                TryPickUpFood();
            }
            else
            {
                Debug.Log("Tentando soltar item...");
                DropItem();
            }
        }
    }

    void TryPickUpFood()
    {
        Collider[] foods = Physics.OverlapSphere(transform.position, interactionRange, foodLayer);
        if (foods.Length > 0 && heldItem == null) // Só pega se não estiver segurando nada
        {
            heldItem = foods[0].gameObject;
            FoodItem foodItem = heldItem.GetComponent<FoodItem>();

            if (foodItem != null)
            {
                Debug.Log($"[PICKUP] Item encontrado: {heldItem.name}");

                // Guarda a posição e rotação original
                originalWorldPosition = heldItem.transform.position;
                originalWorldRotation = heldItem.transform.rotation;

                // Move para a mão
                heldItem.transform.SetParent(handTransform);
                heldItem.transform.localPosition = holdPositionOffset;
                heldItem.transform.localEulerAngles = holdRotationOffset;

                // Desativa a física para segurar
                Rigidbody rb = heldItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                }
            }
            else
            {
                Debug.Log("[PICKUP] Objeto na camada de comida não tem o componente FoodItem.");
                heldItem = null; // Limpa a referência se não for um FoodItem válido
            }
        }
        else if (heldItem != null)
        {
            Debug.Log("[PICKUP] Já estou segurando um item.");
        }
        else
        {
            Debug.Log("[PICKUP] Nenhum objeto na camada de comida dentro do alcance.");
        }
    }

    void DropItem()
    {
        if (heldItem != null)
        {
            Debug.Log($"[DROP] Soltando item: {heldItem.name}");

            // Restaura a física
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.AddForce(transform.forward * dropForwardForce, ForceMode.Impulse);
                rb.AddForce(transform.up * dropUpwardForce, ForceMode.Impulse);
            }

            // Remove da mão e restaura a posição/rotação mundial (ou você pode usar a posição de drop)
            heldItem.transform.SetParent(null);
            heldItem.transform.position = handTransform.position + transform.forward * 0.5f; // Solta à frente do jogador
            heldItem.transform.rotation = Quaternion.identity; // Ou mantenha a rotação original, se preferir

            heldItem = null;
        }
        else
        {
            Debug.Log("[DROP] Não há nenhum item para soltar.");
        }
    }

    void ShowInteractionUI(bool state, string message = "")
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(state);
            if (state && interactionText != null)
            {
                interactionText.text = message;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}