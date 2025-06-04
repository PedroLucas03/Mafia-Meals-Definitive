using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GangsterAI : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Window Interaction Settings")]
    [SerializeField] private Transform targetWindow; // Referência para a janela alvo
    [SerializeField] private ParticleSystem smokeParticleSystem; // Partículas de fumaça na cozinha
    [SerializeField] private float activationTime = 5f; // Tempo de jogo para iniciar a ida à janela
    [SerializeField] private float windowInteractionDelay = 1f; // Tempo na janela antes de fumar
    [SerializeField] private float returnToSpawnDelay = 5f; // Tempo fumando antes de voltar ao spawn

    [Header("References")]
    [SerializeField] private Transform spawnPoint; // Ponto de spawn inicial do NPC

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 initialPosition;
    private bool hasInteractedAtWindow = false; // Flag para garantir interação única
    private bool isReturning = false;
    private bool hasStartedMovingToWindow = false; // NOVO: Flag para controlar o movimento inicial para a janela

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.stoppingDistance = stoppingDistance;

        initialPosition = spawnPoint.position; // Garante que a posição inicial seja do spawn point

        // Garante que o Particle System esteja parado no início
        if (smokeParticleSystem != null) {
            smokeParticleSystem.Stop();
        }
        else {
            Debug.LogWarning("Smoke Particle System não atribuído ao NPC Mafioso (GangsterAI).");
        }
    }

    private void Start() {
        // NPC começa parado no spawn
        agent.SetDestination(transform.position);
    }

    private void Update() {
        // Define o parâmetro de animação "IsWalking" com base na velocidade do NavMeshAgent
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        // Lógica de ativação e movimento para a janela
        if (Time.time >= activationTime && !hasInteractedAtWindow && !isReturning && !hasStartedMovingToWindow) {
            MoveToWindow();
            hasStartedMovingToWindow = true; // Garante que MoveToWindow seja chamada apenas uma vez
        }

        // Se o NPC está indo para a janela e chegou perto o suficiente
        // (usamos a distância, que é mais robusta que comparar o agent.destination == targetWindow.position)
        if (hasStartedMovingToWindow && !hasInteractedAtWindow && !isReturning &&
            agent.remainingDistance <= agent.stoppingDistance + 0.1f) // Adiciona uma pequena margem de erro
        {
            // Verifica se o destino atual é a janela para evitar que ele interaja no spawn se o stoppingDistance for grande
            if (Vector3.Distance(agent.destination, targetWindow.position) < 0.1f) {
                StartCoroutine(PerformWindowInteraction());
            }
        }


        // Se o NPC chegou ao spawn point após a interação
        if (isReturning && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.destination == initialPosition) {
            Debug.Log("Mafioso retornou ao spawn point.");
            isReturning = false;
            // Opcional: desativar o script se ele não for fazer mais nada
            // enabled = false; 
        }
    }

    private void MoveToWindow() {
        if (targetWindow != null) {
            agent.SetDestination(targetWindow.position);
            Debug.Log("Mafioso indo para a janela...");
        }
        else {
            Debug.LogError("Target Window não atribuída ao Mafioso (GangsterAI).");
            enabled = false; // Desativa o script se não houver janela
        }
    }

    private IEnumerator PerformWindowInteraction() {
        hasInteractedAtWindow = true; // Impede que essa coroutine seja chamada novamente

        // Garante que o agente realmente pare na janela, mesmo que o NavMesh tenha um pequeno problema
        agent.isStopped = true;

        Debug.Log("Mafioso chegou à janela, esperando para fumar...");
        animator.SetBool("IsWalking", false); // Parar animação de andar

        yield return new WaitForSeconds(windowInteractionDelay); // Espera um pouco antes de fumar

        Debug.Log("Mafioso começou a fumar.");
        // Ativa a animação de fumar
        if (animator != null) {
            animator.SetBool("IsFumando", true);
        }

        // Ativa a fumaça
        if (smokeParticleSystem != null) {
            smokeParticleSystem.Play();
        }

        yield return new WaitForSeconds(returnToSpawnDelay); // Espera o tempo fumando

        Debug.Log("Mafioso parou de fumar e está retornando ao spawn.");
        // Desativa a animação de fumar
        if (animator != null) {
            animator.SetBool("IsFumando", false);
        }

        // Para a fumaça
        if (smokeParticleSystem != null) {
            smokeParticleSystem.Stop();
        }

        isReturning = true;
        agent.isStopped = false; // Permite que o agente se mova novamente
        ReturnToSpawnPoint();
    }

    private void ReturnToSpawnPoint() {
        agent.SetDestination(initialPosition);
    }

    // Debug: Mostra o ponto alvo da janela no Editor
    private void OnDrawGizmosSelected() {
        if (targetWindow != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetWindow.position, stoppingDistance + 0.1f); // Mostra a margem extra
        }
        if (spawnPoint != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }
    }
}