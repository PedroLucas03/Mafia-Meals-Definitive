using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Required for Action

public class OptionsUI : MonoBehaviour {

    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseCallback; // To store the callback when OptionsUI is closed

    private void Awake() {
        Instance = this;

        moveUpButton.onClick.AddListener(() => RebindBinding(Binding.Move_Up));
        moveDownButton.onClick.AddListener(() => RebindBinding(Binding.Move_Down));
        moveLeftButton.onClick.AddListener(() => RebindBinding(Binding.Move_Left));
        moveRightButton.onClick.AddListener(() => RebindBinding(Binding.Move_Right));
        interactButton.onClick.AddListener(() => RebindBinding(Binding.Interact));
        interactAlternateButton.onClick.AddListener(() => RebindBinding(Binding.InteractAlternate));
        pauseButton.onClick.AddListener(() => RebindBinding(Binding.Pause));
        gamepadInteractButton.onClick.AddListener(() => RebindBinding(Binding.Gamepad_Interact));
        gamepadInteractAlternateButton.onClick.AddListener(() => RebindBinding(Binding.Gamepad_InteractAlternate));
        gamepadPauseButton.onClick.AddListener(() => RebindBinding(Binding.Gamepad_Pause));

        // When the close button is clicked, hide and invoke the callback
        closeButton.onClick.AddListener(() => {
            Hide();
            onCloseCallback?.Invoke(); // Invoke the stored callback
        });
    }

    private void Start() {
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind += BindingManager_OnBindingRebind;
        }

        UpdateVisual();
        Hide(); // Hide initially
    }

    private void OnDestroy() {
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind -= BindingManager_OnBindingRebind;
        }
    }

    private void BindingManager_OnBindingRebind(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        if (BindingManager.Instance != null) {
            moveUpButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Move_Up);
            moveDownButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Move_Down);
            moveLeftButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Move_Left);
            moveRightButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Move_Right);
            interactButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Interact);
            interactAlternateButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.InteractAlternate);
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Pause);
            gamepadInteractButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Gamepad_Interact);
            gamepadInteractAlternateButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Gamepad_InteractAlternate);
            gamepadPauseButton.GetComponentInChildren<TextMeshProUGUI>().text = BindingManager.Instance.GetBindingText(Binding.Gamepad_Pause);
        }
    }

    // Make Show public and accept an Action callback
    public void Show(Action callback = null) {
        this.onCloseCallback = callback; // Store the callback
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void ShowPressToRebindKey() {
        // This method also needs to be public if called from external UIs,
        // but typically it's only called internally by RebindBinding.
        // If it's only called internally, you don't need to change its access.
        // Assuming it's only called by RebindBinding internally, keeping it private is fine.
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void RebindBinding(Binding binding) {
        ShowPressToRebindKey();
        if (BindingManager.Instance != null) {
            BindingManager.Instance.RebindBinding(binding, () => {
                HidePressToRebindKey();
                UpdateVisual();
            });
        }
    }

    private void HidePressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }
}