using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplayOnGameOver : MonoBehaviour {
    [Header("Referências")]
    [SerializeField] private GameObject starsDisplayPanel; // Painel que contém as imagens de estrelas
    [SerializeField] private Image[] starImages; // Array das imagens de estrelas (0-3 estrelas)
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private KitchenGameManager kitchenGameManager;
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Configurações")]
    [SerializeField] private float delayBeforeShowing = 2f;
    [SerializeField] private int oneStarThreshold = 1;
    [SerializeField] private int twoStarsThreshold = 4;
    [SerializeField] private int threeStarsThreshold = 7;

    private void Start() {
        // Desativa tudo inicialmente
        starsDisplayPanel.SetActive(false);
        foreach (var star in starImages) {
            star.gameObject.SetActive(false);
        }

        // Se inscreve no evento de Game Over
        kitchenGameManager.OnStateChanged += KitchenGameManager_OnStateChanged;
    }

    private void OnDestroy() {
        // Desinscreve do evento para evitar vazamentos de memória
        kitchenGameManager.OnStateChanged -= KitchenGameManager_OnStateChanged;
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (kitchenGameManager.IsGameOver()) {
            StartCoroutine(ShowScoreAfterDelay());
        }
        else {
            // Se não for Game Over, esconde tudo
            StopAllCoroutines();
            starsDisplayPanel.SetActive(false);
        }
    }

    private IEnumerator ShowScoreAfterDelay() {
        // Espera o GameOverUI aparecer e mais 2 segundos
        yield return new WaitForSeconds(delayBeforeShowing);

        // Ativa o painel de estrelas
        starsDisplayPanel.SetActive(true);

        // Determina quantas estrelas mostrar
        int playerScore = deliveryManager.GetPlayerScore();
        int starsToShow = 0;

        if (playerScore >= threeStarsThreshold) {
            starsToShow = 3;
        }
        else if (playerScore >= twoStarsThreshold) {
            starsToShow = 2;
        }
        else if (playerScore >= oneStarThreshold) {
            starsToShow = 1;
        }

        // Ativa apenas as imagens de estrelas necessárias
        for (int i = 0; i < starImages.Length; i++) {
            starImages[i].gameObject.SetActive(i < starsToShow);
        }
    }
}