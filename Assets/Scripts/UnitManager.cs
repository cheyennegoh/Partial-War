using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> soldiers;
    public string enemyTag;
    public float engageRange = 3f;    // Range to start moving towards the enemy unit
    public float attackRange = 2f;     // Range to start attacking the enemy unit

    public Vector3 unitCenter;
    private bool anySoldierEngaged = false; // To track if any soldier is engaged

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

        anySoldierEngaged = false; // Reset the engaged flag at the start of each update

        // Check if any soldier is within the engagement range
        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
            if (soldierHealth != null)
            {
                GameObject nearestEnemy = soldierHealth.FindNearestEnemy();

                if (nearestEnemy != null)
                {
                    float distance = Vector3.Distance(soldier.transform.position, nearestEnemy.transform.position);

                    if (distance <= attackRange)
                    {
                        soldierHealth.Attack(nearestEnemy);  // Attack the nearest enemy
                    }
                    else if (distance <= engageRange)
                    {
                        anySoldierEngaged = true;  // Flag that at least one soldier is engaged

                        // Move towards the enemy if within engagement range
                        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
                        if (agent != null && agent.isActiveAndEnabled)
                        {
                            agent.SetDestination(nearestEnemy.transform.position);
                        }
                    }
                }
            }
        }

        // If any soldier is engaged, make all soldiers move toward their nearest enemies
        if (anySoldierEngaged)
        {
            foreach (GameObject soldier in soldiers)
            {
                if (soldier == null) continue;

                SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
                if (soldierHealth != null)
                {
                    GameObject nearestEnemy = soldierHealth.FindNearestEnemy();
                    if (nearestEnemy != null)
                    {
                        // Move all soldiers towards the nearest enemy
                        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
                        if (agent != null && agent.isActiveAndEnabled)
                        {
                            agent.SetDestination(nearestEnemy.transform.position);
                        }
                    }
                }
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
