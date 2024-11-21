using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> soldiers; // Assign in Inspector or dynamically
    public float spacing = 2f;        // Spacing between units

    private void SetDestination(GameObject soldier, Vector3 position)
    {
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
        foreach (GameObject soldier in soldiers)
        {
            center += soldier.transform.position;
        }
        return center / soldiers.Count;
    }

    public void MoveFormation(Vector3 targetPosition)
    {
        Vector3 groupCenter = CalculateGroupCenter();

        foreach (GameObject soldier in soldiers)
        {
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
            int row = i / cols;
            int col = i % cols;

            // Calculate new position relative to group center
            Vector3 position = groupCenter + new Vector3(col * spacing, 0, row * spacing);

            // Update soldier's position directly without using NavMeshAgent
            soldiers[i].transform.position = position;
        }
    }

    void Start()
    {
        string parentName = gameObject.name;
        GameObject redArmy = GameObject.Find(parentName);
        if (redArmy != null)
        {
            // Find all soldiers under the "Red Army 1" group
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
        if (Input.GetMouseButtonDown(0)) // Left-click to move the whole unit
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MoveFormation(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            ArrangeGrid(CalculateGroupCenter(), 3); // Arrange grid at current group center

    }
}
