using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem; // <-- ADD THIS LINE

public class KitchenGameManager : MonoBehaviour {

    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 180f;
    private bool isGamePaused = false;

    // Lista para manter o controle de todos os GameInputs dos jogadores
    // Isso é importante para desinscrever eventos quando os jogadores são destruídos.
    private List<GameInput> registeredGameInputs = new List<GameInput>();


    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start() {
        // NÃO SUBSCREVA GameInput.Instance aqui, pois ele não existe mais globalmente.
        // O registro será feito pelo PlayerSpawner.
    }

    private void OnDestroy() {
        // Desinscreve todos os GameInputs registrados para evitar vazamentos de memória.
        foreach (GameInput gameInput in registeredGameInputs) {
            if (gameInput != null) {
                gameInput.OnPauseAction -= HandlePlayerPauseAction;
                gameInput.OnInteractAction -= HandlePlayerInteractAction;
            }
        }
        registeredGameInputs.Clear(); // Limpa a lista
    }

    // Método para que o PlayerSpawner registre o GameInput de cada jogador
    public void RegisterPlayerGameInput(GameInput gameInput) {
        if (!registeredGameInputs.Contains(gameInput)) { // Evita duplicatas
            registeredGameInputs.Add(gameInput);
            gameInput.OnPauseAction += HandlePlayerPauseAction;
            gameInput.OnInteractAction += HandlePlayerInteractAction;
            // The PlayerInput component will be on the same GameObject as GameInput
            Debug.Log($"KitchenGameManager: Registered GameInput for player {gameInput.GetComponent<PlayerInput>().playerIndex}");
        }
    }

    // Método para desregistrar GameInput (útil se jogadores puderem sair do jogo)
    public void UnregisterPlayerGameInput(GameInput gameInput) {
        if (registeredGameInputs.Contains(gameInput)) {
            registeredGameInputs.Remove(gameInput);
            gameInput.OnPauseAction -= HandlePlayerPauseAction;
            gameInput.OnInteractAction -= HandlePlayerInteractAction;
            Debug.Log($"KitchenGameManager: Unregistered GameInput for player {gameInput.GetComponent<PlayerInput>().playerIndex}");
        }
    }

    // O evento de interação deve ser global para iniciar o jogo
    // Qualquer jogador interagindo inicia o CountdownToStart
    private void HandlePlayerInteractAction(object sender, EventArgs e) {
        if (state == State.WaitingToStart) {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    // O evento de pausa também deve ser global
    // Qualquer jogador pausando o jogo o pausa globalmente
    private void HandlePlayerPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f) {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
        // Debug.Log(state); // You might want to disable this Debug.Log in a final game
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }
    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
}