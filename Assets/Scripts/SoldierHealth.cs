using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierHealth : MonoBehaviour
{
    public string enemyTag;
    public float attackRange = 1f;  // Melee range
    public float detectRange = 10f; // Detection range
    public int health = 100;
    public int attackDamage = 10;  // Set attack damage to 10
    public float attackCooldown = 1f;

    private float lastAttackTime;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        //enemyTag = gameObject.name.Contains("Red") ? "BlueSoldier" : "RedSoldier";
        enemyTag = "BlueSoldier";
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
            else if (distance <= detectRange)
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

    private void Attack(GameObject enemy)
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
}

