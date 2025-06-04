using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Adicione esta linha para usar TextMeshPro

public class ScoreDisplayOnGameOver : MonoBehaviour {
    [Header("Refer�ncias")]
    [SerializeField] private GameObject starsDisplayPanel;
    [SerializeField] private Image[] starImages;
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private KitchenGameManager kitchenGameManager;
    [SerializeField] private GameOverUI gameOverUI; // Embora n�o esteja sendo usado diretamente, manter por consist�ncia
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI scoreTextDisplay; // NOVO: Campo para exibir a pontua��o como texto

    [Header("Configura��es")]
    [SerializeField] private float delayBeforeShowing = 2f; // Delay para as estrelas aparecerem
    [SerializeField] private float delayBeforeHotkeyActive = 3f; // NOVO: Delay para a hotkey ser ativada
    [SerializeField] private int oneStarThreshold = 1;
    [SerializeField] private int twoStarsThreshold = 4;
    [SerializeField] private int threeStarsThreshold = 7;
    [SerializeField] private KeyCode menuHotkey = KeyCode.E;

    private bool canPressHotkey = false;

    private void Start() {
        // Configura��o inicial
        starsDisplayPanel.SetActive(false);
        foreach (var star in starImages) {
            star.gameObject.SetActive(false);
        }
        menuButton.gameObject.SetActive(false); // Esconde o bot�o inicialmente

        // Garante que o texto da pontua��o esteja escondido no in�cio
        if (scoreTextDisplay != null) {
            scoreTextDisplay.gameObject.SetActive(false);
        }

        // Configura o listener do bot�o
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
            // Desativa a hotkey imediatamente para evitar cliques indesejados antes do delay
            canPressHotkey = false;
            StopAllCoroutines(); // Para qualquer coroutine de delay anterior que possa estar rodando
            StartCoroutine(ShowScoreAfterDelay());
        }
        else {
            StopAllCoroutines();
            starsDisplayPanel.SetActive(false);
            menuButton.gameObject.SetActive(false);
            canPressHotkey = false; // Desativa o flag quando n�o est� em Game Over

            // Esconde o texto da pontua��o quando n�o � Game Over
            if (scoreTextDisplay != null) {
                scoreTextDisplay.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ShowScoreAfterDelay() {
        // Espera o tempo definido para as estrelas aparecerem
        yield return new WaitForSeconds(delayBeforeShowing);

        // Mostra as estrelas
        starsDisplayPanel.SetActive(true);

        // Atualiza e mostra o texto da pontua��o AQUI
        if (scoreTextDisplay != null) {
            scoreTextDisplay.text = "TOTAL:<mspace=23em>" + DeliveryManager.Instance.GetPlayerScore().ToString();
            scoreTextDisplay.gameObject.SetActive(true);
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

        // Ativa as imagens corretas
        for (int i = 0; i < starImages.Length; i++) {
            starImages[i].gameObject.SetActive(i < starsToShow);
        }

        // Mostra o bot�o de menu ap�s um pequeno delay adicional
        yield return new WaitForSeconds(0.5f);
        menuButton.gameObject.SetActive(true);

        // NOVO: Espera o tempo adicional para ativar a hotkey
        // Este c�lculo garante que a hotkey seja ativada no 'delayBeforeHotkeyActive' total desde o Game Over.
        // Por exemplo: se delayBeforeShowing=2s, 0.5s para bot�o, e delayBeforeHotkeyActive=3s (total),
        // ent�o (3 - (2 + 0.5)) = 0.5s de espera adicional.
        float totalTimeElapsedBeforeHotkeyCheck = delayBeforeShowing + 0.5f;
        if (delayBeforeHotkeyActive > totalTimeElapsedBeforeHotkeyCheck) {
            yield return new WaitForSeconds(delayBeforeHotkeyActive - totalTimeElapsedBeforeHotkeyCheck);
        }

        canPressHotkey = true; // Ativa a hotkey
    }

    private void GoToMainMenu() {
        // Desativa a hotkey e o bot�o para evitar m�ltiplos cliques/pressionamentos ap�s a navega��o
        canPressHotkey = false;
        menuButton.gameObject.SetActive(false);
        // Esconde o texto da pontua��o ao sair
        if (scoreTextDisplay != null) {
            scoreTextDisplay.gameObject.SetActive(false);
        }
        Loader.Load(Loader.Scene.Interface);
    }
}