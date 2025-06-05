// BindingManager.cs
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class BindingManager : MonoBehaviour {
    public static BindingManager Instance { get; private set; }

    private const string PLAYER_PREFS_BINDINGS_GLOBAL = "GlobalInputBindings";
    // globalPlayerInputActions � a inst�ncia "mestra" que gerencia os bindings que s�o salvos/carregados.
    public PlayerInputActions globalPlayerInputActions;

    // Evento para notificar a UI quando um rebind � conclu�do.
    public event EventHandler OnBindingRebind;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); // Garante que s� h� uma inst�ncia
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Permite que persista entre cenas
        }

        // Crie e habilite a inst�ncia das a��es de input para gerenciar os bindings.
        globalPlayerInputActions = new PlayerInputActions();
        globalPlayerInputActions.Enable();

        // Carregar os bindings salvos se existirem.
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS_GLOBAL)) {
            globalPlayerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS_GLOBAL));
        }
    }

    private void OnDestroy() {
        // Descarte as a��es de input quando o manager for destru�do.
        if (globalPlayerInputActions != null) {
            globalPlayerInputActions.Dispose();
        }
    }

    /// <summary>
    /// Retorna a string de exibi��o para um binding espec�fico.
    /// </summary>
    public string GetBindingText(Binding binding) {
        switch (binding) {
            default:
            case Binding.Move_Up:
                return globalPlayerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return globalPlayerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return globalPlayerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Move_Right:
                return globalPlayerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Interact:
                return globalPlayerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return globalPlayerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return globalPlayerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return globalPlayerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return globalPlayerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return globalPlayerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    /// <summary>
    /// Inicia o processo de remapeamento para um binding espec�fico.
    /// </summary>
    /// <param name="binding">O binding a ser remapeado.</param>
    /// <param name="onActionRebound">Callback a ser executado ap�s o remapeamento.</param>
    public void RebindBinding(Binding binding, Action onActionRebound) {
        // Desabilitar todas as a��es temporariamente para evitar inputs indesejados durante o remapeamento.
        globalPlayerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        // Selecionar a a��o e o �ndice de binding corretos.
        switch (binding) {
            default:
            case Binding.Move_Up:
                inputAction = globalPlayerInputActions.Player.Move;
                bindingIndex = 1; // W key
                break;
            case Binding.Move_Down:
                inputAction = globalPlayerInputActions.Player.Move;
                bindingIndex = 2; // S key
                break;
            case Binding.Move_Left:
                inputAction = globalPlayerInputActions.Player.Move;
                bindingIndex = 4; // A key
                break;
            case Binding.Move_Right:
                inputAction = globalPlayerInputActions.Player.Move;
                bindingIndex = 3; // D key
                break;
            case Binding.Interact:
                inputAction = globalPlayerInputActions.Player.Interact;
                bindingIndex = 0; // Keyboard Interact (E)
                break;
            case Binding.InteractAlternate:
                inputAction = globalPlayerInputActions.Player.InteractAlternate;
                bindingIndex = 0; // Keyboard Interact Alternate (F)
                break;
            case Binding.Pause:
                inputAction = globalPlayerInputActions.Player.Pause;
                bindingIndex = 0; // Keyboard Pause (Escape)
                break;
            case Binding.Gamepad_Interact:
                inputAction = globalPlayerInputActions.Player.Interact;
                bindingIndex = 1; // Gamepad Interact (South Button)
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = globalPlayerInputActions.Player.InteractAlternate;
                bindingIndex = 1; // Gamepad Interact Alternate (West Button)
                break;
            case Binding.Gamepad_Pause:
                inputAction = globalPlayerInputActions.Player.Pause;
                bindingIndex = 1; // Gamepad Pause (Start Button)
                break;
        }

        // Inicia o remapeamento interativo.
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose(); // Libera o callback.

                // Habilita as a��es de input novamente ap�s o remapeamento.
                globalPlayerInputActions.Player.Enable();

                // Salva os bindings atualizados.
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS_GLOBAL, globalPlayerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                // Notifica a UI e qualquer outro ouvinte que um binding foi remapeado.
                OnBindingRebind?.Invoke(this, EventArgs.Empty);
                onActionRebound?.Invoke(); // Chama o callback espec�fico da UI.
            })
            .Start(); // Inicia o processo.
    }
}