using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Para usar .Any()

public class GangsterManager : MonoBehaviour {
    [Header("Mafioso Settings")]
    [SerializeField] private GameObject gangsterPrefab; // O prefab do seu GameObject Mafioso com o script GangsterAI
    [SerializeField] private float initialSpawnDelay = 3f; // Atraso para o primeiro mafioso aparecer
    [SerializeField] private float minRespawnDelay = 10f; // Tempo m�nimo antes de um novo mafioso aparecer
    [SerializeField] private float maxRespawnDelay = 20f; // Tempo m�ximo antes de um novo mafioso aparecer

    [Header("Points Configuration")]
    [SerializeField] private List<Transform> spawnPoints; // Lista de poss�veis pontos de spawn
    [SerializeField] private List<WindowSmokeController> windowControllers; // NOVO: Lista de scripts de controle de janela

    private GangsterAI currentActiveGangster = null;
    private bool _isSpawning = false;

    private void Start() {
        if (gangsterPrefab == null) {
            Debug.LogError("Gangster Prefab n�o atribu�do ao GangsterManager! Desativando script.");
            enabled = false;
            return;
        }
        if (spawnPoints.Count == 0 || windowControllers.Count == 0) // Verifica��o alterada
        {
            Debug.LogError("� necess�rio ter pelo menos um Spawn Point e um Window Controller atribu�dos ao GangsterManager! Desativando script.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnGangsterAfterDelay(initialSpawnDelay));
    }

    private IEnumerator SpawnGangsterAfterDelay(float delay) {
        _isSpawning = true;
        Debug.Log($"Gerenciador: Pr�ximo mafioso em {delay:F2} segundos...");
        yield return new WaitForSeconds(delay);
        SpawnNewGangster();
        _isSpawning = false;
    }

    private void SpawnNewGangster() {
        if (currentActiveGangster != null) {
            Debug.LogWarning("Gerenciador: Tentando spawnar um novo mafioso enquanto um j� est� ativo. Abortando spawn.");
            return;
        }

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        // Escolhe um WindowSmokeController aleat�rio
        WindowSmokeController selectedWindowController = windowControllers[Random.Range(0, windowControllers.Count)];

        Debug.Log($"Gerenciador: Criando novo mafioso em '{selectedSpawnPoint.name}' indo para janela '{selectedWindowController.name}'.");

        GameObject newGangsterGO = Instantiate(gangsterPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
        currentActiveGangster = newGangsterGO.GetComponent<GangsterAI>();

        if (currentActiveGangster != null) {
            currentActiveGangster.OnRoutineComplete += HandleGangsterRoutineComplete; // Assina o evento
            // Passa a refer�ncia para o script da janela
            currentActiveGangster.StartGangsterRoutine(selectedSpawnPoint.position, selectedWindowController);
        }
        else {
            Debug.LogError("O Prefab do Mafioso n�o tem o componente GangsterAI! Destruindo GameObject.");
            Destroy(newGangsterGO);
        }
    }

    private void HandleGangsterRoutineComplete(GangsterAI gangster) {
        if (gangster != null) {
            gangster.OnRoutineComplete -= HandleGangsterRoutineComplete; // Desinscreve
        }

        if (gangster == currentActiveGangster) {
            Debug.Log($"Gerenciador: Rotina do mafioso '{gangster.name}' conclu�da. Destruindo e preparando pr�ximo spawn.");

            Destroy(gangster.gameObject);
            currentActiveGangster = null;

            if (!_isSpawning) {
                float nextSpawnDelay = Random.Range(minRespawnDelay, maxRespawnDelay);
                StartCoroutine(SpawnGangsterAfterDelay(nextSpawnDelay));
            }
        }
    }

    // Desenha os pontos no editor para facilitar a visualiza��o
    private void OnDrawGizmos() {
        if (spawnPoints != null) {
            Gizmos.color = Color.cyan;
            foreach (Transform point in spawnPoints) {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.3f);
            }
        }
        if (windowControllers != null) // Alterado para desenhar os pontos das janelas controladoras
        {
            Gizmos.color = Color.magenta;
            foreach (WindowSmokeController controller in windowControllers) {
                if (controller != null)
                    Gizmos.DrawSphere(controller.transform.position, 0.3f);
            }
        }
    }
}