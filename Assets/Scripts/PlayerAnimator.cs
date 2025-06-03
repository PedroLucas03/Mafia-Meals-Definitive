using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";
    private const string IS_PICKING_UP = "IsPickingUp";
    private const string IS_DROPPING = "IsDropping"; // Adiciona o parâmetro para a animação de soltar

    [SerializeField] private Player player;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        player.OnPickedSomething += Player_OnPickedSomething;
        player.OnDroppedSomething += Player_OnDroppedSomething;
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_PICKING_UP, player.IsHoldingAndAnimated() && player.HasKitchenObject()); // Ativa a animação de pegar somente se tiver um objeto
        animator.SetBool(IS_DROPPING, player.IsHoldingAndAnimated() && !player.HasKitchenObject()); // Ativa a animação de soltar se não tiver um objeto
    }

    // Funções a serem chamadas pelos Eventos de Animação no Animator

    private void Player_OnPickedSomething(object sender, System.EventArgs e) {
        // Este evento é chamado quando o Player pega um item.
        // A animação de pegar é controlada pela variável IS_PICKING_UP no Update.
    }

    private void Player_OnDroppedSomething(object sender, System.EventArgs e) {
        // Este evento é chamado quando o Player está prestes a soltar um item.
        // A animação de soltar é controlada pela variável IS_DROPPING no Update.
    }

    public void OnPickingUpAnimationComplete() {
        Debug.Log("OnPickingUpAnimationComplete called");
        player.SetHoldingAndAnimated(false); // Libera o estado de travado do jogador após a animação de pegar
        animator.SetBool(IS_PICKING_UP, false); // Desativa a animação de pegar
    }

    public void OnDroppingAnimationComplete() {
        player.SetHoldingAndAnimated(false); // Libera o estado de travado do jogador após a animação de soltar
        animator.SetBool(IS_DROPPING, false); // Desativa a animação de soltar
    }
}