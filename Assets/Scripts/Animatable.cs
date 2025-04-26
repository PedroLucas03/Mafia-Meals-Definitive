using UnityEngine;

public class Animatable : MonoBehaviour
{
    [Header("Animator Settings")]
    public Animator objectAnimator;
    public string animationBoolParam; 
    public Animator playerAnimator;
    public string playerAnimationTrigger; 
    public string holdItemBoolParam = "IsHoldingItem";

    [Header("Interaction Settings")]
    public string customMessage; 
    public AudioClip interactionSound; 
    public GameObject interactionUI;
    public float closeDelay = 2f;

    [Header("Item Settings")]
    public GameObject itemPrefab;
    public Transform handTransform;
    public float dropForce = 3f;
    public LayerMask countertopLayer;
    public Vector3 holdPositionOffset = new Vector3(0.1f, 0.1f, 0.2f);
    public Vector3 holdRotationOffset = new Vector3(0, 0, 0);

    [Header("Drop Settings")]
    public float dropDistance = 0.5f;
    public KeyCode dropKey = KeyCode.Q;

    private GameObject heldItem;
    private bool isOpen = false;
    private float closeTimer = 0f;
    private bool canInteract = true;

    void Update()
    {
        // Sistema para soltar o item com tecla
        if (heldItem != null && Input.GetKeyDown(dropKey))
        {
            DropItem();
        }

        // Temporizador para fechar o objeto
        if (isOpen)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
            {
                objectAnimator.SetBool(animationBoolParam, false);
                isOpen = false;
            }
        }
    }

    public string GetInteractionMessage()
    {
        return canInteract ? 
            (string.IsNullOrEmpty(customMessage) ? "Pegar de " + gameObject.name : customMessage) 
            : "Nada mais aqui";
    }

    public virtual void Interact()
    {
        if (!canInteract) return;

        if (objectAnimator != null && !isOpen)
        {
            StartCoroutine(InteractionRoutine());
        }
    }

    private System.Collections.IEnumerator InteractionRoutine()
    {
        // Dispara animação do player
        if (playerAnimator != null && !string.IsNullOrEmpty(playerAnimationTrigger))
        {
            playerAnimator.SetTrigger(playerAnimationTrigger);
            
            // Espera um frame para a animação começar
            yield return null;
            
            // Espera até que a animação esteja no ponto de pegar o item
            float animLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength * 0.3f);
        }

        // Abre o objeto interagível
        objectAnimator.SetBool(animationBoolParam, true);
        isOpen = true;
        closeTimer = closeDelay;

        // Pega o item
        PickupItem();

        // Feedback de interação
        if (interactionSound != null)
        {
            AudioSource.PlayClipAtPoint(interactionSound, transform.position);
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
            Invoke("HideUI", 2f);
        }
    }

    private void PickupItem()
    {
        if (itemPrefab == null || handTransform == null) return;

        // Instancia o item diretamente na mão
        heldItem = Instantiate(itemPrefab, handTransform);
        heldItem.transform.localPosition = holdPositionOffset;
        heldItem.transform.localRotation = Quaternion.Euler(holdRotationOffset);

        // Remove componentes físicos
        DestroyRigidbodyAndColliders(heldItem);

        canInteract = false;

        // Ativa estado de segurar item
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(holdItemBoolParam, true);
        }
    }

    public void DropItem()
    {
        if (heldItem == null) return;

        // Solta o item da mão
        heldItem.transform.SetParent(null);

        // Restaura física
        AddPhysicsComponents(heldItem);

        // Posiciona o item corretamente
        PlaceDroppedItem();

        // Limpa referência
        heldItem = null;
        canInteract = true;

        // Atualiza animação
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(holdItemBoolParam, false);
        }
    }

    private void DestroyRigidbodyAndColliders(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        foreach (Collider col in obj.GetComponents<Collider>())
        {
            Destroy(col);
        }
    }

    private void AddPhysicsComponents(GameObject obj)
    {
        obj.AddComponent<Rigidbody>();
        
        // Adiciona colliders apropriados baseados no renderer
        if (obj.GetComponent<MeshFilter>() != null)
        {
            MeshCollider meshCol = obj.AddComponent<MeshCollider>();
            meshCol.convex = true;
        }
        else
        {
            obj.AddComponent<BoxCollider>();
        }
    }

    private void PlaceDroppedItem()
    {
        RaycastHit hit;
        Vector3 dropPosition = handTransform.position + handTransform.forward * dropDistance;

        // Verifica se há bancada abaixo
        if (Physics.Raycast(dropPosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, countertopLayer))
        {
            heldItem.transform.position = hit.point + Vector3.up * 0.1f;
            heldItem.transform.rotation = Quaternion.identity;
        }
        else
        {
            // Solta no chão com física
            heldItem.transform.position = dropPosition;
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            rb.AddForce(handTransform.forward * dropForce, ForceMode.Impulse);
        }
    }

    private void HideUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}