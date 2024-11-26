using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> soldiers;
    public string enemyTag;
    public float engageRange = 10f;    // Range to start moving towards the enemy unit
    public float attackRange = 3f;    // Range to start attacking the enemy unit
    public float attackCooldown = 2f; // Cooldown between attacks
    private float lastAttackTime;
    public Vector3 unitCenter;

    private void Start()
    {
        // Assign enemy tag based on the unit's tag
        if (gameObject.CompareTag("RedSoldierUnit"))
        {
            enemyTag = "BlueSoldier";
        }
        else if (gameObject.CompareTag("BlueSoldierUnit"))
        {
            enemyTag = "RedSoldier";
        }

        Debug.Log("Enemy tag set to: " + enemyTag);


        // Initialize soldiers list
        soldiers = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("RedSoldier") || child.CompareTag("BlueSoldier"))
            {
                soldiers.Add(child.gameObject);
            }
        }
    }

    private void Update()
    {
        unitCenter = CalculateGroupCenter();
        Vector3 nearestEnemyUnit = FindNearestEnemyUnit();
        GameObject nearestEnemySoldier = FindNearestEnemySoldier();

        // Check if a valid enemy soldier is found
        if (nearestEnemySoldier != null && nearestEnemyUnit != null)
        {
            float distance = Vector3.Distance(CalculateGroupCenter(), nearestEnemySoldier.transform.position);

            if (distance <= attackRange)
            {
                AttackEnemyUnit(nearestEnemySoldier);
            }
            else if (distance <= engageRange)
            {
                MoveFormationIndividual(nearestEnemySoldier.transform.position);
            }
        }
    }


    private GameObject FindNearestEnemySoldier()
    {
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemyUnits)
        {
            float distance = Vector3.Distance(CalculateGroupCenter(), enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private Vector3 FindNearestEnemyUnit()
    {
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        Vector3 potentialEnemyCenter = Vector3.zero;  // To store the nearest enemy unit's center position

        foreach (GameObject enemy in enemyUnits)
        {
            UnitManager enemyUnitManager = enemy.GetComponent<UnitManager>();

            if (enemyUnitManager != null)
            {
                Vector3 enemyUnitCenter = enemy.transform.parent != null ? enemy.transform.parent.position : enemy.transform.position;

                float distance = Vector3.Distance(transform.parent.position, enemyUnitCenter);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    potentialEnemyCenter = enemyUnitCenter;  // Update the potential enemy center
                }
            }
        }

        return potentialEnemyCenter;  // Return the position of the nearest enemy unit's center
    }



    private void AttackEnemyUnit(GameObject enemyUnit)
    {
        // Iterate through this unit's soldiers and attack individual soldiers in the enemy unit
        UnitManager enemyUnitManager = enemyUnit.GetComponent<UnitManager>();
        if (enemyUnitManager != null)
        {
            foreach (GameObject soldier in soldiers)
            {
                if (soldier == null) continue;

                // Find an enemy soldier to attack
                if (enemyUnitManager.soldiers.Count > 0)
                {
                    GameObject targetSoldier = enemyUnitManager.soldiers[0]; // Simplistic: attack the first soldier
                    SoldierHealth targetHealth = targetSoldier.GetComponent<SoldierHealth>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(soldier.GetComponent<SoldierHealth>().attackDamage);
                        Debug.Log(soldier.name + " attacked " + targetSoldier.name);
                    }
                }
            }
        }
    }

    private void MoveFormation(Vector3 targetPosition)
    {
        Vector3 groupCenter = CalculateGroupCenter();
        Debug.Log("Group center: " + groupCenter + ", Moving towards: " + targetPosition);

        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isActiveAndEnabled)
            {
                Vector3 offset = soldier.transform.position - groupCenter;
                Vector3 destination = targetPosition + offset;
                Debug.Log(soldier.name + " moving to: " + destination);
                agent.SetDestination(destination);
            }
            else
            {
                Debug.LogWarning(soldier.name + " does not have a valid NavMeshAgent.");
            }
        }
    }

    private void MoveFormationIndividual(Vector3 targetPosition)
    {
        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isActiveAndEnabled)
            {
                // Each soldier moves individually towards the target
                Debug.Log(soldier.name + " moving to: " + targetPosition);
                agent.SetDestination(targetPosition);
            }
            else
            {
                Debug.LogWarning(soldier.name + " does not have a valid NavMeshAgent.");
            }
        }
    }



    private Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        foreach (GameObject soldier in soldiers)
        {
            if (soldier != null)
            {
                center += soldier.transform.position;
            }
        }
        return center / soldiers.Count;
    }
}
