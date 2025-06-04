using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";
    private const string IS_PICKING_UP = "IsPickingUp";
    private const string IS_HOLDING_OBJECT = "IsHoldingObject"; // Par�metro para o estado de segurar cont�nuo
    private const string IS_DROPPING = "IsDropping";
    private const string IS_CUTTING = "IsCutting"; // Se voc� for usar essa anima��o

    [SerializeField] private Player player;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        player.OnPickedSomething += Player_OnPickedSomething;
        player.OnDroppedSomething += Player_OnDroppedSomething;
        // player.OnStartCutting += Player_OnStartCutting; // Se for usar
        // player.OnStopCutting += Player_OnStopCutting; // Se for usar
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());

        // A anima��o de cortar pode ser ativada por um Trigger ou SetBool no PlayerAnimator.cs
        // animator.SetBool(IS_CUTTING, player.IsInteractingWithCounterOfType<CuttingCounter>() && player.HasKitchenObject());
    }

    // --- M�todos de Evento de Anima��o ---
    // Estes m�todos s�o chamados por "Animation Events" configurados nas anima��es no Animator.

    private void Player_OnPickedSomething(object sender, System.EventArgs e) {
        animator.SetBool(IS_PICKING_UP, true); // Ativa o boolean para a anima��o de pegar
        // isHoldingAndAnimated j� � definido como true em Player.SetKitchenObject()
    }

    private void Player_OnDroppedSomething(object sender, System.EventArgs e) {
        animator.SetBool(IS_DROPPING, true); // Ativa o boolean para a anima��o de soltar
        // isHoldingAndAnimated j� � definido como true em Player.ClearKitchenObject()
    }

    // Fun��o chamada pelo Animation Event no FINAL da anima��o "Pegar"
    public void OnPickingUpAnimationComplete() {
        player.SetHoldingAndAnimated(false); // Libera o travamento
        animator.SetBool(IS_PICKING_UP, false); // Desativa o boolean para transi��o
        animator.SetBool(IS_HOLDING_OBJECT, true); // Ativa a anima��o de segurar cont�nua
    }

    // Fun��o chamada pelo Animation Event no FINAL da anima��o "Soltar"
    public void OnDroppingAnimationComplete() {
        player.SetHoldingAndAnimated(false); // Libera o travamento
        animator.SetBool(IS_DROPPING, false); // Desativa o boolean para transi��o
        animator.SetBool(IS_HOLDING_OBJECT, false); // Desativa a anima��o de segurar
    }

    // Se for usar anima��o de cortar que trava
    /*
    public void OnCuttingAnimationComplete()
    {
        //player.SetHoldingAndAnimated(false); // Se cortar travar e precisar ser liberado aqui
        //animator.SetBool(IS_CUTTING, false);
    }
    */
}