using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuração")]
    public float interactionRange = 2.0f;
    public LayerMask interactionLayer;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public Animator playerAnimator;

    [Header("Debug")]
    [SerializeField] private Interactable currentInteractable;
    [SerializeField] private Animatable currentAnimatable;

    void Update()
    {
        Collider[] interactablesInRange = Physics.OverlapSphere(transform.position, interactionRange, interactionLayer);

        if (interactablesInRange.Length > 0)
        {
            // Verifica Interactable primeiro
            currentInteractable = interactablesInRange[0].GetComponent<Interactable>();
            if (currentInteractable != null)
            {
                currentInteractable.SetPlayerAnimator(playerAnimator);
                ShowInteractionUI(true, currentInteractable.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey))
                {
                    currentInteractable.Interact();
                }
                return;
            }

            // Se não encontrou Interactable, verifica Animatable
            currentAnimatable = interactablesInRange[0].GetComponent<Animatable>();
            if (currentAnimatable != null)
            {
                ShowInteractionUI(true, currentAnimatable.GetInteractionMessage());

                if (Input.GetKeyDown(interactionKey))
                {
                    currentAnimatable.Interact();
                }
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

    private void ShowInteractionUI(bool state, string message = "")
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