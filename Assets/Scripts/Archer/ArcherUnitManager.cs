using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ArcherUnitManager : MonoBehaviour
{
    [SerializeField]
    GameObject prefabCapsule;
    
    public List<GameObject> soldiers;
    public float spacing = 2f;        // Spacing between units
    public List<string> enemyTag = new List<string>();
    public List<string> allyTag = new List<string>();
    public float engageRange = 3f;    // Range to start moving towards the enemy unit
    public float attackRange = 2f;     // Range to start attacking the enemy unit

    public Vector3 unitCenter;
    private bool anySoldierEngaged = false; // To track if any soldier is engaged
    public bool isPanicked = false;

    private void Awake()
    {
        // Initialize soldiers list
        for (int i = 0; i < 9; i++)
        {
            GameObject soldier = Instantiate<GameObject>(prefabCapsule, transform);
            Renderer rend = soldier.GetComponent<Renderer>();

            if (gameObject.CompareTag("RedArcherUnit"))
            {
                soldier.tag = "RedArcher";
                rend.material.color = Color.red;
            }
            else if (gameObject.CompareTag("BlueArcherUnit"))
            {
                soldier.tag = "BlueArcher";
                rend.material.color = Color.blue;
            }

            soldiers.Add(soldier);
        }
    }

    private void Start()
    {
        if (gameObject.CompareTag("RedArcherUnit"))
        {
            enemyTag.Add("BlueSoldier");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");

            allyTag.Add("RedSoldier");
            allyTag.Add("RedCavalry");
            allyTag.Add("RedArcher");

            //enemyTag = "BlueSoldier";
        }
        else if (gameObject.CompareTag("BlueArcherUnit"))
        {
            enemyTag.Add("RedSoldier");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");


            allyTag.Add("BlueSoldier");
            allyTag.Add("BlueCavalry");
            allyTag.Add("BlueArcher");
            //enemyTag = "RedSoldier";
        }

        ArrangeGridInPlace();
    }

    private bool hasArrangedAfterEngagement = false;  // Flag to ensure ArrangeGrid is called only once after engagement

    private void Update()
    {

        for (int i = soldiers.Count - 1; i >= 0; i--) // Iterate backward
        {
            if (soldiers[i] == null || !soldiers[i].activeInHierarchy)
            {
                soldiers.RemoveAt(i); // Safely remove the soldier
            }
        }
        if (soldiers.Count == 0) 
        { 
            Destroy(gameObject);
        }

        unitCenter = CalculateGroupCenter();
        Panic(unitCenter);

        if (isPanicked == false)
        {
            anySoldierEngaged = false; // Reset the engaged flag at the start of each update

            // Check if any soldier is within the engagement range
            foreach (GameObject soldier in soldiers)
            {
                if (soldier == null) continue;

                Archer soldierHealth = soldier.GetComponent<Archer>();
                if (soldierHealth != null)
                {
                    GameObject nearestEnemy = soldierHealth.FindNearestEnemy();

                    if (nearestEnemy != null)
                    {
                        float distance = Vector3.Distance(soldier.transform.position, nearestEnemy.transform.position);

                        if (distance <= attackRange)
                        {
                            //soldierHealth.Attack(nearestEnemy);  // Attack the nearest enemy
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

                    Archer soldierHealth = soldier.GetComponent<Archer>();
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


                hasArrangedAfterEngagement = false;
            }
            else
            {
                if (!hasArrangedAfterEngagement)
                {
                    ArrangeGrid(CalculateGroupCenter(), 3);
                    hasArrangedAfterEngagement = true;
                }
            }
        }

        
    }

    private void SetDestination(GameObject soldier, Vector3 position)
    {
        if (soldier == null) return; // Check if soldier is null
        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled)
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
    private void Panic(Vector3 startPosition)
    {
        float panicRadius = 15f; // Radius to check for allies and enemies
        float fleeRadius = 30f;
        int enemyCount = 0;
        int friendCount = 0;

        // Check for soldiers within the radius
        Collider[] colliders = Physics.OverlapSphere(startPosition, panicRadius);

        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            // Count allies (friend tags)
            if (allyTag.Contains(obj.tag))
            {
                friendCount++;
            }
            // Count enemies (enemy tags)
            else if (enemyTag.Contains(obj.tag))
            {
                enemyCount++;
            }
        }


        // Check panic condition: enemies outnumber friends 3:1
        if (enemyCount >= 3 * friendCount)
        {
            // Move soldiers away from the center of enemies
            Vector3 fleeDirection = (startPosition - CalculateEnemiesCenter(colliders)).normalized;
            Vector3 fleePosition = startPosition + fleeDirection * fleeRadius;

            isPanicked = true;

            Debug.Log("We are panicking ahhhhhhhhhhhhhhhhhh");
            MoveFormation(fleePosition);

            foreach (var soldier in soldiers)
            {
                if(soldier != null && soldier.activeInHierarchy)
                {
                    soldier.GetComponent<NavMeshAgent>().speed = 5f;
                }
            }
                

        }
        else
        {
            foreach (var soldier in soldiers)
            {
                if (soldier != null && soldier.activeInHierarchy)
                {
                    soldier.GetComponent<NavMeshAgent>().speed = 3.5f;
                }
            }
            isPanicked =false;
        }
    }

    private Vector3 CalculateEnemiesCenter(Collider[] colliders)
    {
        Vector3 enemyCenter = Vector3.zero;
        int enemyCount = 0;

        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            // Include only enemies in the center calculation
            if (enemyTag.Contains(obj.tag))
            {
                enemyCenter += obj.transform.position;
                enemyCount++;
            }
        }

        return enemyCount > 0 ? enemyCenter / enemyCount : Vector3.zero;
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