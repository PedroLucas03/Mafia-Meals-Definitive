using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GangsterAI : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float detectionRange = 10f; // Alcance para detectar jogadores

    [Header("References")]
    [SerializeField] private Transform kitchenDoor;
    [SerializeField] private Transform targetPointAfterFridge; // Novo ponto para ir ap�s o jogador estar na geladeira
    [SerializeField] private Transform spawnPoint;

    private NavMeshAgent agent;
    private Animator animator;
    private Player targetPlayer;
    private bool isWaiting = false;
    private Vector3 initialPosition;
    private bool playerWasInFridge = false; // Flag para evitar persegui��o repetida

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.stoppingDistance = stoppingDistance;
        initialPosition = spawnPoint.position;
    }

    private void Start() {
        MoveToKitchenDoor();
    }

    private void Update() {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        if (isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            if (agent.destination == kitchenDoor.position) {
                StartCoroutine(WaitAtKitchenDoor());
            }
            else if (agent.destination == initialPosition) {
                Destroy(gameObject);
            }
            else if (agent.destination == targetPointAfterFridge.position) {
                Debug.Log("Chegou ao ponto ap�s o jogador estar na geladeira.");
                // Aqui voc� pode adicionar o comportamento desejado ap�s chegar a esse ponto.
                ReturnToSpawnPoint(); // Por exemplo, voltar ao spawn
            }
        }
    }

    private void MoveToKitchenDoor() {
        agent.SetDestination(kitchenDoor.position);
    }

    private IEnumerator WaitAtKitchenDoor() {
        isWaiting = true;
        animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(waitTime);

        // Verifica jogadores dentro do range
        targetPlayer = FindClosestPlayerInRange();

        if (targetPlayer != null && targetPlayer.IsInsideFridge && !playerWasInFridge) {
            Debug.Log("Jogador est� dentro da geladeira! Movendo para o ponto de destino.");
            agent.SetDestination(targetPointAfterFridge.position);
            playerWasInFridge = true; // Garante que a persegui��o s� aconte�a uma vez por entrada na geladeira
        }
        else if (targetPlayer != null && !targetPlayer.IsInsideFridge) {
            Debug.Log("Jogador fora da geladeira. Esperando...");
            // Se o jogador sair da geladeira antes do tempo de espera acabar,
            // voc� pode querer redefinir a flag para que ele possa ser detectado novamente
            playerWasInFridge = false;
            // Se voc� quiser que ele fa�a algo mesmo com o jogador fora, adicione l�gica aqui.
        }
        else {
            Debug.Log("Nenhum jogador encontrado no range.");
        }

        isWaiting = false;
        // Se o jogador n�o estiver na geladeira, o gangster simplesmente para de esperar
        // e o Update continuar� verificando a dist�ncia. Se voc� quiser que ele espere novamente,
        // voc� precisaria chamar MoveToKitchenDoor() aqui ou em outro lugar.
    }

    private Player FindClosestPlayerInRange() {
        Player[] players = FindObjectsOfType<Player>();
        Player closestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (Player player in players) {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance && distance <= detectionRange) {
                minDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    private void ReturnToSpawnPoint() {
        agent.SetDestination(initialPosition);
    }

    // Debug: Mostra o range de detec��o
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}