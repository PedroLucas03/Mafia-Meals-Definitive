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
    [SerializeField] private float windowInteractionDelay = 1f; // Tempo na janela ANTES de come�ar a fumar
    [SerializeField] private float smokingVisualEffectDuration = 5f; // Dura��o que o efeito visual da fuma�a dura no ParticleSystem
    [SerializeField] private float idleAfterSmokingDuration = 1f; // Tempo parado na janela DEPOIS que a fuma�a parou de ser vis�vel, ANTES de voltar ao spawn

    private WindowSmokeController currentTargetWindowController;
    private Vector3 currentSpawnPosition;

    private NavMeshAgent agent;
    private Animator animator;

    private bool _isRoutineActive = false;
    private bool _isReturning = false;
    private bool _hasStartedWindowInteraction = false; // Flag para controlar o in�cio da intera��o na janela

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

        // L�gica para iniciar a intera��o na janela
        if (!_isReturning && !_hasStartedWindowInteraction) {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f && agent.velocity.sqrMagnitude < 0.01f) {
                if (currentTargetWindowController != null && Vector3.Distance(agent.destination, currentTargetWindowController.transform.position) < 0.1f) {
                    StartCoroutine(PerformWindowInteraction());
                    _hasStartedWindowInteraction = true;
                }
            }
        }

        // L�gica de retorno
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
        // O WindowSmokeController j� faz Stop/Clear no Awake e StartSmoke() faz isso antes de Play().

        MoveToWindow();
    }

    private void MoveToWindow() {
        if (currentTargetWindowController != null) {
            agent.SetDestination(currentTargetWindowController.transform.position);
            Debug.Log($"Mafioso ({gameObject.name}) indo para a janela: {currentTargetWindowController.name}");
        }
        else {
            Debug.LogError($"Mafioso ({gameObject.name}): WindowSmokeController n�o atribu�da pelo Manager.");
            _isRoutineActive = false;
            OnRoutineComplete?.Invoke(this);
        }
    }

    private IEnumerator PerformWindowInteraction() {
        agent.isStopped = true;
        Debug.Log($"Mafioso ({gameObject.name}) chegou � janela, esperando para fumar...");
        animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(windowInteractionDelay);

        Debug.Log($"Mafioso ({gameObject.name}) come�ou a fumar. Tempo: {Time.time}");
        animator.SetBool("IsFumando", true);

        if (currentTargetWindowController != null) {
            currentTargetWindowController.StartSmoke();
        }

        // Espera a dura��o do efeito visual da fuma�a para que ela tenha tempo de sumir
        yield return new WaitForSeconds(smokingVisualEffectDuration);

        // A fuma�a j� deve ter parado de ser vis�vel por si s�. N�o precisamos de StopSmoke() aqui.
        Debug.Log($"Mafioso ({gameObject.name}) parou de fumar (anima��o). Tempo: {Time.time}");
        animator.SetBool("IsFumando", false);

        yield return new WaitForSeconds(idleAfterSmokingDuration);

        Debug.Log($"Mafioso ({gameObject.name}) est� retornando ao spawn.");
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