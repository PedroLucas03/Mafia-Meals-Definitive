using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform Player;
    public float enemySpeed;
    public float stoppingDistance = 3f; // Distância para parar de se mover
    public Transform weaponPivot; // Objeto para controlar a mira da arma

    [SerializeField] private float Timer = 1f; // Cadência de tiro mais rápida
    private float bulletTime;
    public GameObject enemyBullet;
    public Transform spawnPoint;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        // Controle de movimento
        if (distanceToPlayer > stoppingDistance)
        {
            enemy.isStopped = false;
            enemy.SetDestination(Player.position);
        }
        else
        {
            enemy.isStopped = true; // Para de se mover
        }

        // Mira a arma no jogador
        if (weaponPivot != null)
        {
            Vector3 lookDirection = (Player.position - weaponPivot.position).normalized;
            weaponPivot.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Atira apenas quando parado
        if (GetComponent<StateManager>().currentState is AttackState && enemy.isStopped)
        {
            ShootAtPlayer();
        }
    }

    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if (bulletTime > 0) return;

        bulletTime = Timer;

        // Direção precisa para o jogador (considerando altura)
        Vector3 playerCenter = Player.position + Vector3.up * 0.5f;
        Vector3 shootDirection = (playerCenter - spawnPoint.position).normalized;

        GameObject bulletObj = Instantiate(enemyBullet, spawnPoint.position, Quaternion.LookRotation(shootDirection));
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();

        bulletRig.velocity = shootDirection * enemySpeed; // Método mais preciso que AddForce
        Destroy(bulletObj, 3f);
    }
}