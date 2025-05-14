using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Configuração Básica")]
    public string customMessage;
    public Color newColor = Color.red;

    [Header("Configuração de Animação")]
    public Animator objectAnimator; // Animator da bancada/máquina
    public string objectAnimationTrigger = "Cut"; // Nome do trigger da bancada

    private MeshRenderer meshRenderer;
    private Color originalColor;
    private Animator playerAnimator; // Referência ao Animator do jogador

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    public string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Interagir com " + gameObject.name : customMessage;
    }

    public void SetPlayerAnimator(Animator animator)
    {
        playerAnimator = animator;
    }

    public void Interact()
    {
        // Mudança de cor (feedback visual)
        if (meshRenderer != null)
        {
            meshRenderer.material.color = (meshRenderer.material.color == originalColor) ? newColor : originalColor;
        }

        // Dispara animações
        if (objectAnimator != null)
        {
            objectAnimator.SetTrigger(objectAnimationTrigger);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Cutting"); // Trigger da animação do player
        }

        Debug.Log($"Interagiu com {gameObject.name}");
    }
}