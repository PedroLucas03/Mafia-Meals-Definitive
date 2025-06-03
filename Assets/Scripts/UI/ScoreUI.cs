using TMPro;
using UnityEngine;
using static LeanTween;

public class ScoreUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start() {
        UpdateScoreDisplay(); // Exibe a pontua��o inicial
    }

    private void Update() {
        UpdateScoreDisplay(); // Atualiza a pontua��o a cada frame
    }

    private void UpdateScoreDisplay() {
        if (DeliveryManager.Instance != null && scoreText != null) {
            scoreText.text = DeliveryManager.Instance.GetPlayerScore().ToString();
        }
    }
}