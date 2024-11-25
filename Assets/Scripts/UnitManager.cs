using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> soldiers;
    public float spacing = 2f;        // Spacing between units
    public string enemyTag;

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

        for (int i = 0; i < soldiers.Count; i++)
        {
            // Skip destroyed soldiers
            if (soldiers[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            Vector3 position = startPosition + new Vector3(col * spacing, 0, row * spacing);
            SetDestination(soldiers[i], position);
        }
    }

    private Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        int validSoldiersCount = 0;

        foreach (GameObject soldier in soldiers)
        {
            // Skip destroyed soldiers
            if (soldier == null) continue;

            center += soldier.transform.position;
            validSoldiersCount++;
        }

        if (validSoldiersCount > 0)
        {
            return center / validSoldiersCount;
        }
        return Vector3.zero;
    }

    public void MoveFormation(Vector3 targetPosition)
    {
        Vector3 groupCenter = CalculateGroupCenter();

        foreach (GameObject soldier in soldiers)
        {
            // Skip destroyed soldiers
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

    public void AssignEnemyTagToSoldiers()
    {
        foreach (GameObject soldier in soldiers)
        {
            SoldierHealth soldierScript = soldier.GetComponent<SoldierHealth>();
            if (soldierScript != null)
            {
                soldierScript.enemyTag = enemyTag;
            }
        }
    }

    void Start()
    {
        /*if (gameObject.name.Contains("Red"))
        {
            enemyTag = "BlueSoldier";
        }
        else if (gameObject.name.Contains("Blue"))
        {
            enemyTag = "RedSoldier";
        }*/

        enemyTag = "BlueSoldier";

        AssignEnemyTagToSoldiers();

        string parentName = gameObject.name;
        GameObject redArmy = GameObject.Find(parentName);
        if (redArmy != null)
        {
            //Find all the soldiers in the related Unit
            soldiers = new List<GameObject>();
            foreach (Transform child in redArmy.transform)
            {
                if (child.CompareTag("Soldier"))
                {
                    soldiers.Add(child.gameObject);
                }
            }
        }

        ArrangeGridInPlace();
    }

    void Update()
    {
        // Move formation on left-click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MoveFormation(hit.point);
            }
        }

        // Rearrange grid with 'G'
        if (Input.GetKeyDown(KeyCode.G))
        {
            ArrangeGrid(CalculateGroupCenter(), 3);
        }

        // Automatically move toward enemies if they are nearby
        foreach (GameObject soldier in soldiers)
        {
            if (soldier == null) continue;

            SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
            if (soldierHealth != null)
            {
                GameObject nearestEnemy = soldierHealth.FindNearestEnemy();
                if (nearestEnemy != null)
                {
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
