using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleNPC : MonoBehaviour {
    public Animator animator; // Arraste o componente Animator do seu NPC aqui
    public string idleAnimationName = "Idle"; // Nome da sua anima��o de idle

    void Start() {
        // Garante que o componente Animator esteja atribu�do
        if (animator == null) {
            animator = GetComponent<Animator>();
            if (animator == null) {
                Debug.LogError("Componente Animator n�o encontrado no GameObject!");
                enabled = false; // Desativa o script se n�o houver Animator
                return;
            }
        }

        // Inicia a anima��o de idle
        animator.Play(idleAnimationName);
    }

    // A anima��o de idle, por ser um loop configurado no Animator,
    // se repetir� automaticamente sem necessidade de c�digo adicional aqui.
}
