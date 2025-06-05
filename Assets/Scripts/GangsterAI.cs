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

    [Header("Interaction Durations")]
    [SerializeField] private float windowInteractionDelay = 1f; // Tempo na janela ANTES de começar a fumar
    [SerializeField] private float smokingVisualEffectDuration = 5f; // Duração que o efeito visual da fumaça dura no ParticleSystem
    [SerializeField] private float idleAfterSmokingDuration = 1f; // Tempo parado na janela DEPOIS que a fumaça parou de ser visível, ANTES de voltar ao spawn

    private WindowSmokeController currentTargetWindowController;
    private Vector3 currentSpawnPosition;

    private NavMeshAgent agent;
    private Animator animator;

    private bool _isRoutineActive = false;
    private bool _isReturning = false;
    private bool _hasStartedWindowInteraction = false; // Flag para controlar o início da interação na janela

    public event System.Action<GangsterAI> OnRoutineComplete;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.stoppingDistance = stoppingDistance;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsFumando", false);
    }

    private void Update() {
        if (!_isRoutineActive) return;

        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        // Lógica para iniciar a interação na janela
        if (!_isReturning && !_hasStartedWindowInteraction) {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f && agent.velocity.sqrMagnitude < 0.01f) {
                if (currentTargetWindowController != null && Vector3.Distance(agent.destination, currentTargetWindowController.transform.position) < 0.1f) {
                    StartCoroutine(PerformWindowInteraction());
                    _hasStartedWindowInteraction = true;
                }
            }
        }

        // Lógica de retorno
        if (_isReturning && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f) {
            if (Vector3.Distance(agent.destination, currentSpawnPosition) < 0.1f) {
                Debug.Log($"Mafioso ({gameObject.name}) retornou ao spawn point e completou a rotina.");
                _isReturning = false;
                _isRoutineActive = false;
                animator.SetBool("IsWalking", false);
                OnRoutineComplete?.Invoke(this);
            }
        }
    }

    public void StartGangsterRoutine(Vector3 spawnPos, WindowSmokeController windowController) {
        currentSpawnPosition = spawnPos;
        currentTargetWindowController = windowController;
        _isRoutineActive = true;
        _isReturning = false;
        _hasStartedWindowInteraction = false; // Resetar para cada nova rotina!

        transform.position = spawnPos;
        agent.Warp(spawnPos);
        agent.isStopped = false;

        // REMOVIDO: A chamada currentTargetWindowController.StopSmoke() aqui.
        // O WindowSmokeController já faz Stop/Clear no Awake e StartSmoke() faz isso antes de Play().

        MoveToWindow();
    }

    private void MoveToWindow() {
        if (currentTargetWindowController != null) {
            agent.SetDestination(currentTargetWindowController.transform.position);
            Debug.Log($"Mafioso ({gameObject.name}) indo para a janela: {currentTargetWindowController.name}");
        }
        else {
            Debug.LogError($"Mafioso ({gameObject.name}): WindowSmokeController não atribuída pelo Manager.");
            _isRoutineActive = false;
            OnRoutineComplete?.Invoke(this);
        }
    }

    private IEnumerator PerformWindowInteraction() {
        agent.isStopped = true;
        Debug.Log($"Mafioso ({gameObject.name}) chegou à janela, esperando para fumar...");
        animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(windowInteractionDelay);

        Debug.Log($"Mafioso ({gameObject.name}) começou a fumar. Tempo: {Time.time}");
        animator.SetBool("IsFumando", true);

        if (currentTargetWindowController != null) {
            currentTargetWindowController.StartSmoke();
        }

        // Espera a duração do efeito visual da fumaça para que ela tenha tempo de sumir
        yield return new WaitForSeconds(smokingVisualEffectDuration);

        // A fumaça já deve ter parado de ser visível por si só. Não precisamos de StopSmoke() aqui.
        Debug.Log($"Mafioso ({gameObject.name}) parou de fumar (animação). Tempo: {Time.time}");
        animator.SetBool("IsFumando", false);

        yield return new WaitForSeconds(idleAfterSmokingDuration);

        Debug.Log($"Mafioso ({gameObject.name}) está retornando ao spawn.");
        _isReturning = true;
        agent.isStopped = false;
        ReturnToSpawnPoint();
    }

    private void ReturnToSpawnPoint() {
        agent.SetDestination(currentSpawnPosition);
    }

    private void OnDrawGizmosSelected() {
        if (currentTargetWindowController != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currentTargetWindowController.transform.position, stoppingDistance + 0.1f);
        }
        if (_isRoutineActive && currentSpawnPosition != Vector3.zero) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentSpawnPosition, 0.5f);
        }
    }
}