using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlueUnitManager : MonoBehaviour
{
    public List<GameObject> soldiers; // Assign in Inspector or dynamically
    public float spacing = 2f;        // Spacing between units
    public float attackRange = 5f;    // Distance at which soldiers attack each other
    public int attackDamage = 10;     // Attack damage per soldier
    public float attackCooldown = 1f; // Time between attacks
    public float moveSpeed = 2f;      // Speed at which soldiers move to formation

    private void SetDestination(GameObject soldier, Vector3 position)
    {
        NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(position);
        }
    }

    public void ArrangeGridInPlace()
    {
        if (soldiers.Count == 0) return;

        // Calculate the group's center based on initial positions
        Vector3 groupCenter = CalculateGroupCenter();
        int rows = Mathf.CeilToInt(Mathf.Sqrt(soldiers.Count));
        int cols = Mathf.CeilToInt((float)soldiers.Count / rows);

        StartCoroutine(MoveSoldiersToFormation(groupCenter, rows, cols));
    }

    private IEnumerator MoveSoldiersToFormation(Vector3 groupCenter, int rows, int cols)
    {
        // Gradually move soldiers to their new positions in the grid formation
        for (int i = 0; i < soldiers.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            // Calculate the target position
            Vector3 targetPosition = groupCenter + new Vector3(col * spacing, 0, row * spacing);

            // Smoothly move soldier to target position
            float elapsedTime = 0f;
            Vector3 startingPosition = soldiers[i].transform.position;

            while (elapsedTime < 1f)
            {
                soldiers[i].transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }
            soldiers[i].transform.position = targetPosition; // Ensure final position is set
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

    // Check for attacks
    public void CheckForAttacks(UnitManager enemyUnit)
    {
        foreach (GameObject soldier in soldiers)
        {
            foreach (GameObject enemy in enemyUnit.soldiers)
            {
                float distance = Vector3.Distance(soldier.transform.position, enemy.transform.position);
                if (distance <= attackRange)
                {
                    Attack(soldier, enemy);
                }
            }
        }
    }

    private void Attack(GameObject attacker, GameObject target)
    {
        // Example attack mechanic: Apply damage
        SoldierHealth targetHealth = target.GetComponent<SoldierHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
            Debug.Log(attacker.name + " attacked " + target.name);
        }
    }

    void Start()
    {
        soldiers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Soldier"));
        ArrangeGridInPlace();
    }

    void Update()
    {
        // Attack check
        if (soldiers.Count > 0 && GameObject.FindWithTag("RedArmy") != null)
        {
            UnitManager redArmy = GameObject.FindWithTag("RedArmy").GetComponent<UnitManager>();
            CheckForAttacks(redArmy);
        }

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
            ArrangeGridInPlace(); // Rearrange the grid at the current group center
    }
}
