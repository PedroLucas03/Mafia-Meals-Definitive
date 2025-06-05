using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject bonusDisplayToggle;
    [SerializeField] private TextMeshProUGUI bonusMultiplierText; // NOVO: Campo para o texto do multiplicador
    [SerializeField] private Image bonusFillImage;

    private void Awake() {
        if (bonusDisplayToggle != null) {
            bonusDisplayToggle.SetActive(false);
        }
    }

    private void Start() {
        UpdateScoreDisplay();
    }

    private void OnEnable() {
        if (DeliveryManager.Instance != null) {
            DeliveryManager.Instance.OnBonusMultiplierChanged += DeliveryManager_OnBonusMultiplierChanged;
        }
    }

    private void OnDisable() {
        if (DeliveryManager.Instance != null) {
            DeliveryManager.Instance.OnBonusMultiplierChanged -= DeliveryManager_OnBonusMultiplierChanged;
        }
    }

    private void Update() {
        UpdateScoreDisplay();
        UpdateBonusFillImage(); // MUDANÇA: Separamos a atualização da imagem de preenchimento
    }

    private void UpdateScoreDisplay() {
        if (DeliveryManager.Instance != null && scoreText != null) {
            scoreText.text = DeliveryManager.Instance.GetPlayerScore().ToString();
        }
    }

    private void DeliveryManager_OnBonusMultiplierChanged(object sender, EventArgs e) {
        UpdateBonusDisplayVisibility();
        UpdateBonusMultiplierText(); // NOVO: Atualiza o texto do multiplicador
        UpdateBonusFillImage();     // Garante que a imagem de preenchimento seja atualizada na mudança do bônus
    }

    private void UpdateBonusDisplayVisibility() {
        if (bonusDisplayToggle == null || DeliveryManager.Instance == null) return;

        bool isActive = DeliveryManager.Instance.IsBonusActive;
        bonusDisplayToggle.SetActive(isActive);
    }

    private void UpdateBonusMultiplierText() {
        if (bonusMultiplierText == null || DeliveryManager.Instance == null) return;

        // Mostra o multiplicador atual (ex: 1.2x, 1.4x)
        bonusMultiplierText.text = DeliveryManager.Instance.GetCurrentBonusMultiplier().ToString("F1") + "x";
    }

    private void UpdateBonusFillImage() { // MUDANÇA: Função separada para a imagem
        if (bonusFillImage == null || DeliveryManager.Instance == null) return;

        if (DeliveryManager.Instance.IsBonusActive) {
            float timeLeft = DeliveryManager.Instance.GetCurrentBonusResetTimer();
            // Atualiza a barra de progresso (Image Fill Amount)
            bonusFillImage.fillAmount = timeLeft / DeliveryManager.Instance.bonusMultiplierResetTime;
        }
        else {
            // Se o bônus não está ativo, a barra de preenchimento deve estar vazia (0)
            bonusFillImage.fillAmount = 0f;
        }
    }
}