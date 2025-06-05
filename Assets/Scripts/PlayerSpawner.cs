using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour {

    public GameObject playerPrefab; // Refer�ncia ao seu prefab de Player
    public GameObject gameInputPrefab; // Crie um prefab para o GameInput tamb�m, ou instancie o componente

    // Este m�todo � chamado pelo PlayerInputManager quando um novo jogador � adicionado
    // (se Notification Behavior estiver configurado para Send Messages)
    public void OnPlayerJoined(PlayerInput playerInput) {
        Debug.Log($"Novo jogador conectado: {playerInput.playerIndex}");

        // Instancie o GameInput como um componente no GameObject do playerInput.
        // Isso garante que cada PlayerInput tenha seu pr�prio GameInput.
        GameInput gameInputInstance = playerInput.gameObject.AddComponent<GameInput>();
        gameInputInstance.Initialize(playerInput); // Inicializa o GameInput com a inst�ncia de PlayerInput

        // Encontre o componente Player no GameObject do playerInput
        // O PlayerInputManager ir� instanciar o Player Prefab que voc� configurou.
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