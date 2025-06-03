using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";
    private const string IS_PICKING_UP = "IsPickingUp";
    private const string IS_DROPPING = "IsDropping"; // Adiciona o par�metro para a anima��o de soltar

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
        animator.SetBool(IS_PICKING_UP, player.IsHoldingAndAnimated() && player.HasKitchenObject()); // Ativa a anima��o de pegar somente se tiver um objeto
        animator.SetBool(IS_DROPPING, player.IsHoldingAndAnimated() && !player.HasKitchenObject()); // Ativa a anima��o de soltar se n�o tiver um objeto
    }

    // Fun��es a serem chamadas pelos Eventos de Anima��o no Animator

    private void Player_OnPickedSomething(object sender, System.EventArgs e) {
        // Este evento � chamado quando o Player pega um item.
        // A anima��o de pegar � controlada pela vari�vel IS_PICKING_UP no Update.
    }

    private void Player_OnDroppedSomething(object sender, System.EventArgs e) {
        // Este evento � chamado quando o Player est� prestes a soltar um item.
        // A anima��o de soltar � controlada pela vari�vel IS_DROPPING no Update.
    }

    public void OnPickingUpAnimationComplete() {
        Debug.Log("OnPickingUpAnimationComplete called");
        player.SetHoldingAndAnimated(false); // Libera o estado de travado do jogador ap�s a anima��o de pegar
        animator.SetBool(IS_PICKING_UP, false); // Desativa a anima��o de pegar
    }

    public void OnDroppingAnimationComplete() {
        player.SetHoldingAndAnimated(false); // Libera o estado de travado do jogador ap�s a anima��o de soltar
        animator.SetBool(IS_DROPPING, false); // Desativa a anima��o de soltar
    }
}