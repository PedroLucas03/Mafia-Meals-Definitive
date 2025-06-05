using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

    [SerializeField] private BaseCounter clearCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    // This method will be called by the Player or PlayerSpawner
    public void Initialize(Player player) {
        if (player == null) {
            Debug.LogError("SelectedCounterVisual: Player reference is null during initialization.", this);
            return;
        }

        player.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        // Ensure initial state is correct
        Hide();
    }

    private void OnDestroy() {
        // Important: If the player is destroyed before this visual, you might get a null reference.
        // It's safer to have the player unregister this listener if possible,
        // or check for null on 'sender' in the event handler if the player could be gone.
        // For now, assuming the visual might be destroyed before the player, or they are paired.
        // You would ideally want the Player script to manage this event unsubscription upon its destruction.
        // As a fallback, if you have a reference to the player, you can try to unsubscribe:
        // (This might require storing the 'player' reference as a member variable in SelectedCounterVisual)

        // Example (if 'player' was a member variable):
        // if (player != null) {
        //     player.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
        // }
        // For simplicity, we'll rely on the player destroying the visual or not having multiple subscriptions.
    }


    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
        // 'sender' here refers to the Player instance that triggered the event.
        // You can use 'e.selectedCounter' to check if it's the counter this visual cares about.
        if (e.selectedCounter == clearCounter) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }
}