using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    GameObject prefabMilitia;

    [SerializeField]
    GameObject prefabCavalry;

    [SerializeField]
    GameObject prefabArcher;
    
    List<GameObject> soldiers = new List<GameObject>();
    float spacing = 2f;        // Spacing between units
    List<string> enemyTag = new List<string>();
    List<string> allyTag = new List<string>();
    float engageRange;    // Range to start moving towards the enemy unit
    float attackRange;     // Range to start attacking the enemy unit
    public bool anySoldierEngaged = false; // To track if any soldier is engaged
    bool isPanicked = false;

    bool isArranged = true;  // Flag to ensure ArrangeGrid is called only once after engagement

    private bool isCharge = false; // Cavalry Charge stats
    private float lastChargeTime = 0f;
    private float chargeDuration = 5f;
    private float chargeCooldown = 0f;
    public bool underGeneralCommand = true;
    public Vector3 groupCenter;

    void Awake()
    {
        // Initialize soldiers list
        for (int i = 0; i < 9; i++)
        {
            CreateSoldier();
        }

        ArrangeGridInPlace();
    }
    
    void Start()
    {
        if (gameObject.CompareTag("RedMilitiaUnit") || gameObject.CompareTag("BlueMilitiaUnit"))
        {
            engageRange = 10f;
            attackRange = 3f;
        }
        else if (gameObject.CompareTag("RedCavalryUnit") || gameObject.CompareTag("BlueCavalryUnit"))
        {
            engageRange = 10f;
            attackRange = 2f;
        }
        else 
        {
            engageRange = 40f;
            attackRange = 20f;
        }
        
        if (gameObject.CompareTag("RedMilitiaUnit") || gameObject.CompareTag("RedCavalryUnit") || gameObject.CompareTag("RedArcherUnit"))
        {
            enemyTag.Add("BlueMilitia");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");

            allyTag.Add("RedMilitia");
            allyTag.Add("RedCavalry");
            allyTag.Add("RedArcher");
        }
        else
        {
            enemyTag.Add("RedMilitia");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");

            allyTag.Add("BlueMilitia");
            allyTag.Add("BlueCavalry");
            allyTag.Add("BlueArcher");
        }
    }

    void Update()
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
        groupCenter = CalculateGroupCenter();
        Panic(groupCenter);

        if (isPanicked) return;
        
        anySoldierEngaged = false; // Reset the engaged flag at the start of each update

        // Check if any soldier is within the engagement range
        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            Soldier soldierHealth = soldier.GetComponent<Soldier>();
            if (soldierHealth == null) continue;
            
            GameObject nearestEnemy = soldierHealth.FindNearestEnemy();
            if (nearestEnemy == null) continue;
            
            float distance = Vector3.Distance(soldier.transform.position, nearestEnemy.transform.position);
            NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();

            if (distance <= attackRange)
            {
                // Stop moving and attack the enemy
                if (agent != null && agent.isActiveAndEnabled)
                {
                    agent.SetDestination(soldier.transform.position); // Stop the agent
                }
                
                // Attack
                if (soldier.tag.Contains("Cavalry"))
                {
                    UseCharge();
                    soldierHealth.Attack(nearestEnemy, isCharge);
                }
                else if (soldier.tag.Contains("Archer"))
                {
                    soldierHealth.Attack(nearestEnemy, distance);
                }
                else
                {
                    soldierHealth.Attack(nearestEnemy);  // Attack the nearest enemy
                }
                anySoldierEngaged = true;
            }
            else if (distance <= engageRange)
            {
                anySoldierEngaged = true;  // Flag that at least one soldier is engaged

                // Move towards the enemy if within engagement range
                if (agent != null && agent.isActiveAndEnabled)
                {
                    agent.SetDestination(nearestEnemy.transform.position);
                }
            }
        }

        if (anySoldierEngaged)
        {
            isArranged = false;
        }
        else
        {
            if (!isArranged)
            {
                ArrangeGrid(CalculateGroupCenter(), 3);
                isArranged = true;
            }
        }
    }
    public void UseCharge()
    {

        if (lastChargeTime == 0f && chargeCooldown >= 15f)
        {
            isCharge = true;
            lastChargeTime = Time.time;
        }
        if (chargeDuration >= chargeCooldown)
        {
            chargeCooldown = Time.time - lastChargeTime;
        }
        else
        {
            isCharge = false;
            chargeCooldown = Time.time - lastChargeTime;
            if (chargeCooldown >= 15)
            {
                lastChargeTime = 0f;
            }
        }
    }

    void CreateSoldier()
    {
        GameObject soldier;
        
        // Instantiate prefab
        if (gameObject.CompareTag("RedMilitiaUnit") || gameObject.CompareTag("BlueMilitiaUnit"))
        {
            soldier = Instantiate(prefabMilitia, transform);
        }
        else if (gameObject.CompareTag("RedCavalryUnit") || gameObject.CompareTag("BlueCavalryUnit"))
        {
            soldier = Instantiate(prefabCavalry, transform);
        }
        else 
        {
            soldier = Instantiate(prefabArcher, transform);
        }

        // Tag object
        if (gameObject.CompareTag("RedMilitiaUnit"))
        {
            soldier.tag = "RedMilitia";
        }
        else if (gameObject.CompareTag("BlueMilitiaUnit"))
        {
            soldier.tag = "BlueMilitia";
        }
        else if (gameObject.CompareTag("RedCavalryUnit"))
        {
            soldier.tag = "RedCavalry";
        }
        else if (gameObject.CompareTag("BlueCavalryUnit"))
        {
            soldier.tag = "BlueCavalry";
        }
        else if (gameObject.CompareTag("RedArcherUnit"))
        {
            soldier.tag = "RedArcher";
        }
        else
        {
            soldier.tag = "BlueArcher";
        }

        // Set material color
        if (gameObject.CompareTag("RedMilitiaUnit") || gameObject.CompareTag("RedCavalryUnit") || gameObject.CompareTag("RedArcherUnit"))
        {
            soldier.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            soldier.GetComponent<Renderer>().material.color = Color.blue;
        }

        soldiers.Add(soldier);
    }

    void SetDestination(GameObject soldier, Vector3 position)
    {
        if (soldier == null) return; // Check if soldier is null
        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(position);
        }
    }

    void ArrangeGrid(Vector3 startPosition, int rows)
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

    void ArrangeGridInPlace()
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

    void Panic(Vector3 startPosition)
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

            MoveFormation(fleePosition);

            foreach (var soldier in soldiers)
            {
                if (soldier != null && soldier.activeInHierarchy)
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
            isPanicked = false;
        }
    }

    Vector3 CalculateEnemiesCenter(Collider[] colliders)
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

    Vector3 CalculateGroupCenter()
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