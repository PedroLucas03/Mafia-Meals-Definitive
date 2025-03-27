using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configurações de Hover")]
    [SerializeField] private float hoverScale = 1.1f; // Escala do botão (10% maior)
    [SerializeField] private float hoverFontSize = 22f; // Tamanho da fonte
    [SerializeField] private Color hoverTextColor = Color.yellow; // Cor do texto
    [SerializeField] private float lerpSpeed = 8f; // Velocidade da animação

    private Vector3 originalScale;
    private Text buttonText;
    private float originalFontSize;
    private Color originalTextColor;
    private bool isHovering;

    void Start()
    {
        originalScale = transform.localScale;
        buttonText = GetComponentInChildren<Text>();
        originalFontSize = buttonText.fontSize;
        originalTextColor = buttonText.color;
    }

    void Update()
    {
        // Interpolação suave para todos os efeitos
        float delta = lerpSpeed * Time.deltaTime;

        // Escala do botão
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            isHovering ? originalScale * hoverScale : originalScale,
            delta
        );

        // Tamanho e cor do texto
        if (buttonText != null)
        {
            buttonText.fontSize = (int)Mathf.Lerp(
                buttonText.fontSize,
                isHovering ? hoverFontSize : originalFontSize,
                delta
            );

            buttonText.color = Color.Lerp(
                buttonText.color,
                isHovering ? hoverTextColor : originalTextColor,
                delta
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => isHovering = true;
    public void OnPointerExit(PointerEventData eventData) => isHovering = false;
}