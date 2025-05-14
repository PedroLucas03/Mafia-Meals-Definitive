using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(other.gameObject); // Mata o jogador (ou pode chamar um sistema de vida)
            Destroy(gameObject); // Destroi a bala
        }
    }
}