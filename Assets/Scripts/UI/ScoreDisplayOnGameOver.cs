using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Adicione esta linha para usar TextMeshPro

public class ScoreDisplayOnGameOver : MonoBehaviour {
    [Header("Referências")]
    [SerializeField] private GameObject starsDisplayPanel;
    [SerializeField] private Image[] starImages;
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private KitchenGameManager kitchenGameManager;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI scoreTextDisplay; // Exibe a pontuação
    [SerializeField] private TextMeshProUGUI recipesCompletedText; // NOVO: Para receitas realizadas
    [SerializeField] private TextMeshProUGUI recipesFailedText;    // NOVO: Para receitas perdidas

    [Header("Configurações")]
    [SerializeField] private float delayBeforeShowing = 2f;
    [SerializeField] private float delayBeforeHotkeyActive = 3f;
    [SerializeField] private int oneStarThreshold = 1;
    [SerializeField] private int twoStarsThreshold = 4;
    [SerializeField] private int threeStarsThreshold = 7;
    [SerializeField] private KeyCode menuHotkey = KeyCode.E;

    private bool canPressHotkey = false;

    private void Start() {
        // Configuração inicial
        starsDisplayPanel.SetActive(false);
        foreach (var star in starImages) {
            star.gameObject.SetActive(false);
        }
        menuButton.gameObject.SetActive(false); // Esconde o botão inicialmente

        // Garante que o texto da pontuação e dos novos contadores estejam escondidos no início
        if (scoreTextDisplay != null) {
            scoreTextDisplay.gameObject.SetActive(false);
        }
        if (recipesCompletedText != null) {
            recipesCompletedText.gameObject.SetActive(false);
        }
        if (recipesFailedText != null) {
            recipesFailedText.gameObject.SetActive(false);
        }


        // Configura o listener do botão
        menuButton.onClick.AddListener(() => {
            GoToMainMenu();
        });

        kitchenGameManager.OnStateChanged += KitchenGameManager_OnStateChanged;
    }

    private void OnDestroy() {
        if (kitchenGameManager != null) {
            kitchenGameManager.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
    }

    private void Update() {
        if (canPressHotkey && Input.GetKeyDown(menuHotkey)) {
            GoToMainMenu();
        }
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (kitchenGameManager.IsGameOver()) {
            canPressHotkey = false;
            StopAllCoroutines();
            StartCoroutine(ShowScoreAfterDelay());
        }
        else {
            StopAllCoroutines();
            starsDisplayPanel.SetActive(false);
            menuButton.gameObject.SetActive(false);
            canPressHotkey = false;

            // Esconde todos os textos de pontuação e contadores quando não é Game Over
            if (scoreTextDisplay != null) {
                scoreTextDisplay.gameObject.SetActive(false);
            }
            if (recipesCompletedText != null) {
                recipesCompletedText.gameObject.SetActive(false);
            }
            if (recipesFailedText != null) {
                recipesFailedText.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ShowScoreAfterDelay() {
        yield return new WaitForSeconds(delayBeforeShowing);

        starsDisplayPanel.SetActive(true);

        // Atualiza e mostra o texto da pontuação
        if (scoreTextDisplay != null) {
            scoreTextDisplay.text = "TOTAL: " + DeliveryManager.Instance.GetPlayerScore().ToString();
            scoreTextDisplay.gameObject.SetActive(true);
        }

        // NOVO: Atualiza e mostra o texto das receitas realizadas
        if (recipesCompletedText != null) {
            recipesCompletedText.text = "REALIZADAS: " + DeliveryManager.Instance.GetRecipesCompletedSuccessfully().ToString();
            recipesCompletedText.gameObject.SetActive(true);
        }

        // NOVO: Atualiza e mostra o texto das receitas perdidas
        if (recipesFailedText != null) {
            recipesFailedText.text = "PERDIDAS: " + DeliveryManager.Instance.GetRecipesFailed().ToString();
            recipesFailedText.gameObject.SetActive(true);
        }

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

        for (int i = 0; i < starImages.Length; i++) {
            starImages[i].gameObject.SetActive(i < starsToShow);
        }

        yield return new WaitForSeconds(0.5f);
        menuButton.gameObject.SetActive(true);

        float totalTimeElapsedBeforeHotkeyCheck = delayBeforeShowing + 0.5f;
        if (delayBeforeHotkeyActive > totalTimeElapsedBeforeHotkeyCheck) {
            yield return new WaitForSeconds(delayBeforeHotkeyActive - totalTimeElapsedBeforeHotkeyCheck);
        }

        canPressHotkey = true;
    }

    private void GoToMainMenu() {
        canPressHotkey = false;
        menuButton.gameObject.SetActive(false);

        // Esconde os textos ao sair
        if (scoreTextDisplay != null) {
            scoreTextDisplay.gameObject.SetActive(false);
        }
        if (recipesCompletedText != null) {
            recipesCompletedText.gameObject.SetActive(false);
        }
        if (recipesFailedText != null) {
            recipesFailedText.gameObject.SetActive(false);
        }
        Loader.Load(Loader.Scene.Interface);
    }
}