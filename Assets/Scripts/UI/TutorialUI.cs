using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Ensure TextMeshPro is being used

public class TutorialUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyMoveInteractText;
    [SerializeField] private TextMeshProUGUI keyMoveInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyMovePauseText;
    [SerializeField] private TextMeshProUGUI keyMoveGamepadInteractText;
    [SerializeField] private TextMeshProUGUI keyMoveGamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyMoveGamepadPauseText;

    private void Start() {
        // Subscribe to the BindingManager's rebind event to update the UI
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind += BindingManager_OnBindingRebind;
        }
        else {
            Debug.LogError("BindingManager.Instance is null. Make sure BindingManager is set up correctly in the scene.");
        }

        // Subscribe to the KitchenGameManager state changes
        if (KitchenGameManager.Instance != null) {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        }
        else {
            Debug.LogError("KitchenGameManager.Instance is null. Make sure KitchenGameManager is set up correctly in the scene.");
        }

        UpdateVisual(); // Update the UI immediately with current bindings
        Show(); // Show the tutorial UI by default at start
    }

    private void OnDestroy() {
        // Unsubscribe from events to prevent memory leaks
        if (BindingManager.Instance != null) {
            BindingManager.Instance.OnBindingRebind -= BindingManager_OnBindingRebind;
        }
        if (KitchenGameManager.Instance != null) {
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        // Hide the tutorial when the game countdown starts
        if (KitchenGameManager.Instance.IsCountdownToStartActive()) {
            Hide();
        }
    }

    private void BindingManager_OnBindingRebind(object sender, System.EventArgs e) {
        UpdateVisual(); // Update the tutorial UI after any rebind
    }

    private void UpdateVisual() {
        // Access the Binding enum directly and get text from BindingManager.Instance
        if (BindingManager.Instance != null) {
            keyMoveUpText.text = BindingManager.Instance.GetBindingText(Binding.Move_Up);
            keyMoveDownText.text = BindingManager.Instance.GetBindingText(Binding.Move_Down);
            keyMoveLeftText.text = BindingManager.Instance.GetBindingText(Binding.Move_Left);
            keyMoveRightText.text = BindingManager.Instance.GetBindingText(Binding.Move_Right);
            keyMoveInteractText.text = BindingManager.Instance.GetBindingText(Binding.Interact);
            keyMoveInteractAlternateText.text = BindingManager.Instance.GetBindingText(Binding.InteractAlternate);
            keyMovePauseText.text = BindingManager.Instance.GetBindingText(Binding.Pause);
            keyMoveGamepadInteractText.text = BindingManager.Instance.GetBindingText(Binding.Gamepad_Interact);
            keyMoveGamepadInteractAlternateText.text = BindingManager.Instance.GetBindingText(Binding.Gamepad_InteractAlternate);
            keyMoveGamepadPauseText.text = BindingManager.Instance.GetBindingText(Binding.Gamepad_Pause);
        }
        else {
            // Optional: Handle the case where BindingManager might not be available yet
            Debug.LogWarning("BindingManager.Instance is not available to update TutorialUI visuals.");
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}