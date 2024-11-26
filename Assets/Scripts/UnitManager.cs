using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    GameObject prefabCapsule;
    
    public List<GameObject> soldiers;
    public float spacing = 2f;        // Spacing between units
    public List<string> enemyTag = new List<string>();
    public float engageRange = 3f;    
    public float attackRange = 2f;     // Range to start attacking the enemy unit

    public Vector3 unitCenter;
    private bool anySoldierEngaged = false; // To track if any soldier is engaged

    private void Start()
    {
        // Initialize soldiers list
        for (int i = 0; i < 9; i++)
        {
            GameObject soldier = Instantiate<GameObject>(prefabCapsule, transform);
            Renderer rend = soldier.GetComponent<Renderer>();

            if (gameObject.CompareTag("RedSoldierUnit"))
            {
                soldier.tag = "RedSoldier";
                rend.material.color = Color.red;
            }
            else if (gameObject.CompareTag("BlueSoldierUnit"))
            {
                soldier.tag = "BlueSoldier";
                rend.material.color = Color.blue;
            }

            soldiers.Add(soldier);
        }

        if (gameObject.CompareTag("RedSoldierUnit"))
        {
            enemyTag.Add("BlueSoldier");
            enemyTag.Add("BlueCavlary");
            enemyTag.Add("BlueArcher");

            //enemyTag = "BlueSoldier";
        }
        else if (gameObject.CompareTag("BlueSoldierUnit"))
        {
            enemyTag.Add("RedSoldier");
            enemyTag.Add("RedCavlary");
            enemyTag.Add("RedArcher");

            //enemyTag = "RedSoldier";
        }

        ArrangeGridInPlace();
    }

    private bool hasArrangedAfterEngagement = false;  // Flag to ensure ArrangeGrid is called only once after engagement

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

            hasArrangedAfterEngagement = false;  // Reset flag while soldiers are engaged
        }
        else
        {
            // Call ArrangeGrid only once after all engagement is finished
            if (!hasArrangedAfterEngagement)
            {
                ArrangeGrid(CalculateGroupCenter(), 3);
                hasArrangedAfterEngagement = true;  // Set flag to prevent multiple calls
            }
        }
        else 
        {
            ArrangeGrid(CalculateGroupCenter(), 3);
        }
    }




    private void SetDestination(GameObject soldier, Vector3 position)
    {
        if (soldier == null) return; // Check if soldier is null
        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(position);
        }
    }

    public void ArrangeGrid(Vector3 startPosition, int rows)
    {
        int cols = Mathf.CeilToInt((float)soldiers.Count / rows);

        List<GameObject> activeSoldiers = soldiers.FindAll(soldier => soldier != null); // Filter out dead soldiers

        for (int i = 0; i < activeSoldiers.Count; i++)
        {
            // Skip destroyed soldiers
            if (activeSoldiers[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            Vector3 position = startPosition + new Vector3(col * spacing, 0, row * spacing);
            SetDestination(activeSoldiers[i], position);
        }
    }

    public void MoveFormation(Vector3 targetPosition)
    {
        Vector3 groupCenter = CalculateGroupCenter();

        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            Vector3 offset = soldier.transform.position - groupCenter;
            Vector3 destination = targetPosition + offset;
            SetDestination(soldier, destination);
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
            // Skip destroyed soldiers
            if (soldiers[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            // Calculate new position relative to group center
            Vector3 position = groupCenter + new Vector3(col * spacing, 0, row * spacing);

            // Update soldier's position directly without using NavMeshAgent
            soldiers[i].transform.position = position;
        }
    }

    private Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        int activeSoldiersCount = 0;

        foreach (GameObject soldier in soldiers)
        {
            if (soldier != null && soldier.activeInHierarchy)  
            {
                center += soldier.transform.position;
                activeSoldiersCount++;
            }
        }

        if (activeSoldiersCount == 0) return transform.position;
        return center / activeSoldiersCount;
    }

}