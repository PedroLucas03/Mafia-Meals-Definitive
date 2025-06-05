using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour {

    public GameObject playerPrefab; // Referência ao seu prefab de Player
    public GameObject gameInputPrefab; // Crie um prefab para o GameInput também, ou instancie o componente

    // Este método é chamado pelo PlayerInputManager quando um novo jogador é adicionado
    // (se Notification Behavior estiver configurado para Send Messages)
    public void OnPlayerJoined(PlayerInput playerInput) {
        Debug.Log($"Novo jogador conectado: {playerInput.playerIndex}");

        // Instancie o GameInput como um componente no GameObject do playerInput.
        // Isso garante que cada PlayerInput tenha seu próprio GameInput.
        GameInput gameInputInstance = playerInput.gameObject.AddComponent<GameInput>();
        gameInputInstance.Initialize(playerInput); // Inicializa o GameInput com a instância de PlayerInput

        // Encontre o componente Player no GameObject do playerInput
        // O PlayerInputManager irá instanciar o Player Prefab que você configurou.
        Player playerComponent = playerInput.GetComponent<Player>();
        if (playerComponent != null) {
            playerComponent.Initialize(gameInputInstance); // Inicializa o Player com seu GameInput
        }
        else {
            Debug.LogError("Player component not found on the spawned player prefab!");
        }

        // Opcional: posicione os jogadores em locais diferentes
        playerInput.gameObject.transform.position = new Vector3(playerInput.playerIndex * 3f, 0, 0);
    }
}