using UnityEngine;

public class Animatable : MonoBehaviour
{
    public Animator objectAnimator;
    public string animationBoolParam; // Nome do parâmetro booleano no Animator
    public string customMessage; // Mensagem personalizada para a interação
    public AudioClip interactionSound; // Som ao interagir
    public GameObject interactionUI; // UI para exibir a mensagem
    public float closeDelay = 2f; // Tempo para fechar automaticamente após abrir

    private bool isOpen = false; // Estado do balcão (aberto/fechado)
    private float closeTimer = 0f; // Temporizador para fechar automaticamente

    public string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Interagir com " + gameObject.name : customMessage;
    }

    public virtual void Interact()
    {
        if (objectAnimator != null && !isOpen)
        {
            // Abre o balcão
            objectAnimator.SetBool(animationBoolParam, true);
            isOpen = true;
            closeTimer = closeDelay; // Inicia o temporizador para fechar

            // Toca um som ao interagir
            if (interactionSound != null)
            {
                AudioSource.PlayClipAtPoint(interactionSound, transform.position);
            }

            // Exibe a mensagem na UI
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                Invoke("HideUI", 2f); // Esconde a UI após 2 segundos
            }

            Debug.Log($"Balcão aberto: {gameObject.name}");
        }
    }

    private void Update()
    {
        if (isOpen)
        {
            // Conta o tempo para fechar automaticamente
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
            {
                // Fecha o balcão
                objectAnimator.SetBool(animationBoolParam, false);
                isOpen = false;

                Debug.Log($"Balcão fechado: {gameObject.name}");
            }
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