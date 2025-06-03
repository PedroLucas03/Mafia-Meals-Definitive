using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LeanTween;

public class DeliveryManagerSingleUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private Image expiryTimerImage;
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color startColor = Color.green; // Cor inicial do timer
    [SerializeField] private Color endColor = Color.red;    // Cor final do timer (quando o tempo está acabando)
    [SerializeField] private float warningThreshold = 0.3f;

    private RecipeSO recipeSO;
    private Vector3 originalPosition;

    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        originalPosition = transform.localPosition;
    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        this.recipeSO = recipeSO;
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }

        // Resetar a cor ao definir um novo pedido
        if (expiryTimerImage != null) {
            expiryTimerImage.color = startColor;
        }
        transform.localPosition = originalPosition;
        LeanTween.cancel(gameObject); // Cancela qualquer tween anterior (para garantir que não haja fade-out pendente)
        canvasGroup.alpha = 1f; // Garante que o pedido seja totalmente visível
    }

    private void Update() {
        if (recipeSO != null && expiryTimerImage != null) {
            float timerNormalized = DeliveryManager.Instance.GetRecipeExpiryTimerNormalized(recipeSO);
            expiryTimerImage.fillAmount = timerNormalized;
            expiryTimerImage.color = Color.Lerp(endColor, startColor, timerNormalized);

            if (timerNormalized <= 0f) {
                LeanTween.alpha(gameObject, 0f, fadeOutTime).setOnComplete(() => {
                    Destroy(gameObject);
                });
            }
        }
    }
}