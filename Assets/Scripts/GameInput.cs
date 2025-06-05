// GameInput.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

// Remova o Singleton Instance, pois cada jogador terá seu próprio GameInput.
// public static GameInput Instance { get; private set;}

public class GameInput : MonoBehaviour {

    // A constante PLAYER_PREFS_BINDINGS não é mais usada aqui, pois o BindingManager cuida disso.
    // private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    // Eventos (agora serão disparados pela instância específica de GameInput do jogador)
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    // O evento OnBindingRebind será disparado pelo BindingManager.
    // public event EventHandler OnBindingRebind;

    private PlayerInputActions playerInputActions;
    private PlayerInput playerInput; // Referência ao PlayerInput que gerencia este GameInput.

    // Novo método para inicializar este GameInput, chamado pelo PlayerSpawner.
    public void Initialize(PlayerInput input) {
        playerInput = input;
        playerInputActions = new PlayerInputActions();

        // Carregar os overrides de binding do BindingManager global.
        // Isso garante que cada jogador use os bindings que foram definidos globalmente.
        if (BindingManager.Instance != null) {
            playerInputActions.LoadBindingOverridesFromJson(BindingManager.Instance.globalPlayerInputActions.SaveBindingOverridesAsJson());
        }

        playerInputActions.Player.Enable(); // Habilitar as ações para este jogador.

        // Assinar os eventos de input para esta instância de GameInput.
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;

        // Se o BindingManager existe, subscreva ao seu evento de rebind
        // para que esta instância de GameInput possa atualizar seus próprios bindings.
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind += BindingManager_OnBindingRebind;
        }
    }

    private void BindingManager_OnBindingRebind(object sender, EventArgs e) {
        // Quando os bindings globais são remapeados, recarregue-os nesta instância.
        if (BindingManager.Instance != null) {
            playerInputActions.LoadBindingOverridesFromJson(BindingManager.Instance.globalPlayerInputActions.SaveBindingOverridesAsJson());
        }
    }

    private void OnDestroy() {
        // Desassinar os eventos quando o GameInput for destruído.
        if (playerInputActions != null) {
            playerInputActions.Player.Interact.performed -= Interact_performed;
            playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
            playerInputActions.Player.Pause.performed -= Pause_performed;
            playerInputActions.Dispose();
        }
        // Desassinar o evento do BindingManager.
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind -= BindingManager_OnBindingRebind;
        }
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        // Lê o valor do input diretamente das ações.
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    // Este método não é mais usado para obter o texto de exibição, pois o BindingManager faz isso.
    // Ele pode ser removido ou adaptado se você precisar de uma versão específica por jogador.
    // No entanto, para consistência, a UI deve chamar o BindingManager.
    public string GetBindingText(Binding binding) {
        // Você pode retornar o texto do BindingManager aqui ou deixar a UI chamar o BindingManager diretamente.
        // Para simplificar e evitar redundância, é melhor que a UI chame o BindingManager.
        // Se você precisar, pode retornar string.Empty ou lançar uma exceção.
        Debug.LogWarning("GameInput.GetBindingText é obsoleto. Use BindingManager.Instance.GetBindingText.");
        return string.Empty; // Ou você pode redirecionar para o BindingManager.Instance.GetBindingText(binding);
    }

    // O método RebindBinding foi movido para BindingManager.
    // public void RebindBinding(Binding binding, Action onActionRebound) { ... }
}