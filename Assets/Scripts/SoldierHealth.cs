using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierHealth : MonoBehaviour
{
    public string enemyTag;
    public float attackRange = 1f;  // Melee attack range 
    public float engageRange = 2f;  // Engagement range (move towards enemy)

    public int health = 200;
    public int attackDamage = 10;  // Set attack damage to 10
    public float attackCooldown = 2f;

    private float lastAttackTime;
    private NavMeshAgent navMeshAgent;
    public bool isEngaged = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Set the enemy tag based on this soldier's team
        if (gameObject.CompareTag("RedSoldier"))
        {
            enemyTag = "BlueSoldier";
        }
        else if (gameObject.CompareTag("BlueSoldier"))
        {
            enemyTag = "RedSoldier";
        }
    }

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            if (distance <= attackRange)
            {
                // Stop moving and attack the enemy
                if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
                {
                    navMeshAgent.SetDestination(transform.position); // Stop the agent
                }
                Attack(nearestEnemy);
            }
            else if (distance <= engageRange)
            {
                // Move toward the enemy if within detection range but outside attack range
                if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
                {
                    navMeshAgent.SetDestination(nearestEnemy.transform.position);
                }
            }
        }
    }

    public GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    public void Attack(GameObject enemy)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            SoldierHealth enemySoldier = enemy.GetComponent<SoldierHealth>();
            if (enemySoldier != null)
            {
                // Deal the attack damage (10 in this case)
                enemySoldier.TakeDamage(attackDamage); // Attack damage is 10
            }
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}

