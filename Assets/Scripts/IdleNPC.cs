using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleNPC : MonoBehaviour {
    public Animator animator; // Arraste o componente Animator do seu NPC aqui
    public string idleAnimationName = "Idle"; // Nome da sua animação de idle

    void Start() {
        // Garante que o componente Animator esteja atribuído
        if (animator == null) {
            animator = GetComponent<Animator>();
            if (animator == null) {
                Debug.LogError("Componente Animator não encontrado no GameObject!");
                enabled = false; // Desativa o script se não houver Animator
                return;
            }
        }

        // Inicia a animação de idle
        animator.Play(idleAnimationName);
    }

    // A animação de idle, por ser um loop configurado no Animator,
    // se repetirá automaticamente sem necessidade de código adicional aqui.
}
