using UnityEngine;

public class WindowSmokeController : MonoBehaviour {
    [SerializeField] private ParticleSystem smokeParticleSystem; // O ParticleSystem de fuma�a desta janela

    void Awake() {
        if (smokeParticleSystem == null) {
            Debug.LogWarning($"WindowSmokeController em {gameObject.name}: Particle System de fuma�a n�o atribu�do.", this);
        }
        else {
            smokeParticleSystem.Stop(); // Garante que a fuma�a esteja parada ao iniciar
        }
    }

    // M�todo para ativar a fuma�a desta janela
    public void StartSmoke() {
        if (smokeParticleSystem != null) {
            smokeParticleSystem.Stop(); // Para qualquer resqu�cio de toque
            smokeParticleSystem.Clear(); // Limpa part�culas existentes
            smokeParticleSystem.Play();
            Debug.Log($"Fuma�a ativada na janela: {gameObject.name}");
        }
    }
}