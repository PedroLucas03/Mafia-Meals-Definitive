using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string customMessage; // Mensagem personalizada para a interação
    public Color newColor = Color.red; // Cor que o objeto assumirá ao interagir

    private MeshRenderer meshRenderer; // Referência ao componente MeshRenderer do objeto
    private Color originalColor; // Cor original do objeto

    private void Start()
    {
        // Obtém o componente MeshRenderer do objeto
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            // Salva a cor original do objeto
            originalColor = meshRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("MeshRenderer não encontrado no objeto!");
        }
    }

    public string GetInteractionMessage()
    {
        return string.IsNullOrEmpty(customMessage) ? "Interagir com " + gameObject.name : customMessage;
    }

    public void Interact()
    {
        if (meshRenderer != null)
        {
            // Alterna entre a cor original e a nova cor
            if (meshRenderer.material.color == originalColor)
            {
                meshRenderer.material.color = newColor;
            }
            else
            {
                meshRenderer.material.color = originalColor;
            }

            Debug.Log($"Interagiu com {gameObject.name} e mudou a cor para {meshRenderer.material.color}");
        }
        else
        {
            Debug.LogWarning("MeshRenderer não encontrado no objeto!");
        }
    }
}