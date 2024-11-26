using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> soldiers;
    public string enemyTag;
    public float engageRange = 3f;    // Range to start moving towards the enemy unit
    public float attackRange = 2f;     // Range to start attacking the enemy unit
    public float spacing = 2f;

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
        ArrangeGridInPlace();
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

    public void ArrangeGrid(Vector3 startPosition, int rows)
    {
        int cols = Mathf.CeilToInt((float)soldiers.Count / rows);

        for (int i = 0; i < soldiers.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            Vector3 position = startPosition + new Vector3(col * spacing, 0, row * spacing);
            SetDestination(soldiers[i], position);
        }
    }
    public void ArrangeGridInPlace()
    {
        if (soldiers.Count == 0) return;

        // Calculate the group's center based on initial positions
        Vector3 groupCenter = CalculateGroupCenter();
        int rows = Mathf.CeilToInt(Mathf.Sqrt(soldiers.Count));
        int cols = Mathf.CeilToInt((float)soldiers.Count / rows);

        for (int i = 0; i < soldiers.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            // Calculate new position relative to group center
            Vector3 position = groupCenter + new Vector3(col * spacing, 0, row * spacing);

            // Update soldier's position directly without using NavMeshAgent
            soldiers[i].transform.position = position;
        }
    }
    private void SetDestination(GameObject soldier, Vector3 position)
    {
        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(position);
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
