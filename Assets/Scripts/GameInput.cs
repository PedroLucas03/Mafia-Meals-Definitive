// GameInput.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

// Remova o Singleton Instance, pois cada jogador ter� seu pr�prio GameInput.
// public static GameInput Instance { get; private set;}

public class GameInput : MonoBehaviour {

    // A constante PLAYER_PREFS_BINDINGS n�o � mais usada aqui, pois o BindingManager cuida disso.
    // private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    // Eventos (agora ser�o disparados pela inst�ncia espec�fica de GameInput do jogador)
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    // O evento OnBindingRebind ser� disparado pelo BindingManager.
    // public event EventHandler OnBindingRebind;

    private PlayerInputActions playerInputActions;
    private PlayerInput playerInput; // Refer�ncia ao PlayerInput que gerencia este GameInput.

    // Novo m�todo para inicializar este GameInput, chamado pelo PlayerSpawner.
    public void Initialize(PlayerInput input) {
        playerInput = input;
        playerInputActions = new PlayerInputActions();

        // Carregar os overrides de binding do BindingManager global.
        // Isso garante que cada jogador use os bindings que foram definidos globalmente.
        if (BindingManager.Instance != null) {
            playerInputActions.LoadBindingOverridesFromJson(BindingManager.Instance.globalPlayerInputActions.SaveBindingOverridesAsJson());
        }

        playerInputActions.Player.Enable(); // Habilitar as a��es para este jogador.

        // Assinar os eventos de input para esta inst�ncia de GameInput.
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;

        // Se o BindingManager existe, subscreva ao seu evento de rebind
        // para que esta inst�ncia de GameInput possa atualizar seus pr�prios bindings.
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind += BindingManager_OnBindingRebind;
        }
    }

    private void BindingManager_OnBindingRebind(object sender, EventArgs e) {
        // Quando os bindings globais s�o remapeados, recarregue-os nesta inst�ncia.
        if (BindingManager.Instance != null) {
            playerInputActions.LoadBindingOverridesFromJson(BindingManager.Instance.globalPlayerInputActions.SaveBindingOverridesAsJson());
        }
    }

    private void OnDestroy() {
        // Desassinar os eventos quando o GameInput for destru�do.
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
        // L� o valor do input diretamente das a��es.
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    // Este m�todo n�o � mais usado para obter o texto de exibi��o, pois o BindingManager faz isso.
    // Ele pode ser removido ou adaptado se voc� precisar de uma vers�o espec�fica por jogador.
    // No entanto, para consist�ncia, a UI deve chamar o BindingManager.
    public string GetBindingText(Binding binding) {
        // Voc� pode retornar o texto do BindingManager aqui ou deixar a UI chamar o BindingManager diretamente.
        // Para simplificar e evitar redund�ncia, � melhor que a UI chame o BindingManager.
        // Se voc� precisar, pode retornar string.Empty ou lan�ar uma exce��o.
        Debug.LogWarning("GameInput.GetBindingText � obsoleto. Use BindingManager.Instance.GetBindingText.");
        return string.Empty; // Ou voc� pode redirecionar para o BindingManager.Instance.GetBindingText(binding);
    }

    // O m�todo RebindBinding foi movido para BindingManager.
    // public void RebindBinding(Binding binding, Action onActionRebound) { ... }
}