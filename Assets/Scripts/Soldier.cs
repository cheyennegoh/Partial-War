using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    protected List<string> enemyTag = new List<string>();
    // protected float attackRangeSoldier;  // Melee attack range 
    // protected float engageRangeSoldier;  // Engagement range (move towards enemy)
    public int health;
    protected NavMeshAgent navMeshAgent;

    virtual protected void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    public GameObject FindNearestEnemy()
    {
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (string tag in enemyTag)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

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
        }

        return nearestEnemy;
    }

    virtual public void Attack(GameObject enemy) {}
    virtual public void Attack(GameObject enemy, bool isCharge) { }
    virtual public void Attack(GameObject enemy, float distance) { }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}
