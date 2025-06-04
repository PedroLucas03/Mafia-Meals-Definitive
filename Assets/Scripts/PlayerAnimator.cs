using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";
    private const string IS_PICKING_UP = "IsPickingUp";
    private const string IS_HOLDING_OBJECT = "IsHoldingObject"; // Parâmetro para o estado de segurar contínuo
    private const string IS_DROPPING = "IsDropping";
    private const string IS_CUTTING = "IsCutting"; // Se você for usar essa animação

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

        // A animação de cortar pode ser ativada por um Trigger ou SetBool no PlayerAnimator.cs
        // animator.SetBool(IS_CUTTING, player.IsInteractingWithCounterOfType<CuttingCounter>() && player.HasKitchenObject());
    }

    // --- Métodos de Evento de Animação ---
    // Estes métodos são chamados por "Animation Events" configurados nas animações no Animator.

    private void Player_OnPickedSomething(object sender, System.EventArgs e) {
        animator.SetBool(IS_PICKING_UP, true); // Ativa o boolean para a animação de pegar
        // isHoldingAndAnimated já é definido como true em Player.SetKitchenObject()
    }

    private void Player_OnDroppedSomething(object sender, System.EventArgs e) {
        animator.SetBool(IS_DROPPING, true); // Ativa o boolean para a animação de soltar
        // isHoldingAndAnimated já é definido como true em Player.ClearKitchenObject()
    }

    // Função chamada pelo Animation Event no FINAL da animação "Pegar"
    public void OnPickingUpAnimationComplete() {
        player.SetHoldingAndAnimated(false); // Libera o travamento
        animator.SetBool(IS_PICKING_UP, false); // Desativa o boolean para transição
        animator.SetBool(IS_HOLDING_OBJECT, true); // Ativa a animação de segurar contínua
    }

    // Função chamada pelo Animation Event no FINAL da animação "Soltar"
    public void OnDroppingAnimationComplete() {
        player.SetHoldingAndAnimated(false); // Libera o travamento
        animator.SetBool(IS_DROPPING, false); // Desativa o boolean para transição
        animator.SetBool(IS_HOLDING_OBJECT, false); // Desativa a animação de segurar
    }

    // Se for usar animação de cortar que trava
    /*
    public void OnCuttingAnimationComplete()
    {
        //player.SetHoldingAndAnimated(false); // Se cortar travar e precisar ser liberado aqui
        //animator.SetBool(IS_CUTTING, false);
    }
    */
}