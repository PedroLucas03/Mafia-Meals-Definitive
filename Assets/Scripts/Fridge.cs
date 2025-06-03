using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Tecla para interagir
    [SerializeField] private float interactionRadius = 2f; // Distância para poder entrar
    [SerializeField] private Transform hidePosition; // Ponto onde o jogador fica escondido (dentro da geladeira)

    private Player player;
    private bool playerInside = false;

    private void Update() {
        // Verifica se o jogador está perto e pressionou a tecla
        if (Input.GetKeyDown(interactKey) && IsPlayerNear()) {
            if (!playerInside) {
                EnterFridge();
            }
            else {
                ExitFridge();
            }
        }
    }

    private bool IsPlayerNear() {
        player = FindObjectOfType<Player>(); // Pega a referência do jogador
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= interactionRadius;
    }

    private void EnterFridge() {
        playerInside = true;
        player.transform.position = hidePosition.position; // Teleporta o jogador para dentro
        player.IsInsideFridge = true;
        player.SetVisibility(false); // Opcional: esconde o jogador visualmente
        Debug.Log("Jogador entrou na geladeira!");
    }

    private void ExitFridge() {
        playerInside = false;
        player.IsInsideFridge = false;
        player.SetVisibility(true); // Torna o jogador visível novamente
        Debug.Log("Jogador saiu da geladeira!");
    }

    // Debug: Mostra raio de interação no Editor
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        if (hidePosition != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(hidePosition.position, Vector3.one * 0.5f);
        }
    }
}
