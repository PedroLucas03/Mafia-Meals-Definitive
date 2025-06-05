using UnityEngine;

public class WindowSmokeController : MonoBehaviour {
    [SerializeField] private ParticleSystem smokeParticleSystem; // O ParticleSystem de fumaça desta janela

    void Awake() {
        if (smokeParticleSystem == null) {
            Debug.LogWarning($"WindowSmokeController em {gameObject.name}: Particle System de fumaça não atribuído.", this);
        }
        else {
            smokeParticleSystem.Stop(); // Garante que a fumaça esteja parada ao iniciar
        }
    }

    // Método para ativar a fumaça desta janela
    public void StartSmoke() {
        if (smokeParticleSystem != null) {
            smokeParticleSystem.Stop(); // Para qualquer resquício de toque
            smokeParticleSystem.Clear(); // Limpa partículas existentes
            smokeParticleSystem.Play();
            Debug.Log($"Fumaça ativada na janela: {gameObject.name}");
        }
    }
}