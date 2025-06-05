using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Para EventArgs

public class SoundManager : MonoBehaviour {

    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private float volume = 1f;

    // Lista para manter o controle de todos os Players registrados
    private List<Player> registeredPlayers = new List<Player>();

    private void Awake() {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start() {
        // SUBSCREVA APENAS A EVENTOS DE SINGLETONS AQUI, COMO DeliveryManager.Instance
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed; // Corrigido de Instance_OnRecipeFailed
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;

        // Player.Instance.OnPickedSomething += Player_OnPickedSomething; // REMOVIDO: Não use Player.Instance
    }

    private void OnDestroy() {
        // Desinscreva eventos de singletons (se eles não forem destruídos primeiro)
        if (DeliveryManager.Instance != null) {
            DeliveryManager.Instance.OnRecipeSuccess -= DeliveryManager_OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
        }
        // Para eventos estáticos como OnAnyCut, eles não precisam de OnDestroy aqui,
        // mas é bom ter uma prática consistente se você reconfigurar mais tarde.
        // CuttingCounter.OnAnyCut -= CuttingCounter_OnAnyCut; // (Opcional, se CuttingCounter é um singleton)
        // BaseCounter.OnAnyObjectPlacedHere -= BaseCounter_OnAnyObjectPlacedHere; // (Opcional)
        // TrashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed; // (Opcional)


        // Desinscreva todos os Players registrados
        foreach (Player player in registeredPlayers) {
            if (player != null) {
                player.OnPickedSomething -= HandlePlayerPickedSomething;
                // ... desinscreva outros eventos de jogador, se houver
            }
        }
        registeredPlayers.Clear();
    }


    // NOVO: Método para que o PlayerSpawner registre cada Player
    public void RegisterPlayer(Player player) {
        if (!registeredPlayers.Contains(player)) { // Evita duplicatas
            registeredPlayers.Add(player);
            player.OnPickedSomething += HandlePlayerPickedSomething;
            // Adicione mais eventos do Player que você precisa ouvir aqui
            Debug.Log($"SoundManager: Registered events for Player {player.name}");
        }
    }

    // NOVO: Método para desregistrar Player (útil se jogadores puderem sair do jogo)
    public void UnregisterPlayer(Player player) {
        if (registeredPlayers.Contains(player)) {
            registeredPlayers.Remove(player);
            player.OnPickedSomething -= HandlePlayerPickedSomething;
            Debug.Log($"SoundManager: Unregistered events for Player {player.name}");
        }
    }


    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    }

    // RENOMEADO para ser mais genérico para qualquer jogador
    private void HandlePlayerPickedSomething(object sender, System.EventArgs e) {
        // 'sender' é a instância do Player que pegou algo.
        // Você pode usar sender para obter a posição do jogador se precisar de sons 3D.
        Player player = sender as Player;
        if (player != null) {
            PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
        }
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    // RENOMEADO e verificado para DeliveryManager.Instance.OnRecipeFailed
    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance; // Assumindo que DeliveryCounter é Singleton
        PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance; // Assumindo que DeliveryCounter é Singleton
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume); // Use UnityEngine.Random
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    public void PlayFootstepsSound(Vector3 position, float volume) {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }

    public void PlayWarningSound(Vector3 position) {
        PlaySound(audioClipRefsSO.warning, position);
    }

    public void ChangeVolume() {
        volume += .1f;
        if (volume > 1f) {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return volume;
    }
}