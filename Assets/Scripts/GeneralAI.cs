using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAI : MonoBehaviour
{
    public List<string> enemyTag = new List<string>();
    public List<string> allyTag = new List<string>();

    void Start()
    {
        if (gameObject.CompareTag("RedArmy"))
        {
            enemyTag.Add("BlueMilitiaUnit");
            enemyTag.Add("BlueCavalryUnit");
            enemyTag.Add("BlueArcherUnit");

            allyTag.Add("RedMilitiaUnit");
            allyTag.Add("RedCavalryUnit");
            allyTag.Add("RedArcherUnit");
        }
        else if (gameObject.CompareTag("BlueArmy"))
        {
            enemyTag.Add("RedMilitiaUnit");
            enemyTag.Add("RedCavalryUnit");
            enemyTag.Add("RedArcherUnit");

            allyTag.Add("BlueMilitiaUnit");
            allyTag.Add("BlueCavalryUnit");
            allyTag.Add("BlueArcherUnit");
        }
    }

    void Update()
    {
        OrderUnitsToMove();
    }

    void OrderUnitsToMove()
    {
        foreach (Transform child in transform)
        {
            if (child.tag.Contains("Unit"))
            {
                UnitManager unitManager = child.GetComponent<UnitManager>();
                Vector3 nearestEnemyPosition = FindNearestEnemy(unitManager.groupCenter);
                if (Vector3.Distance(nearestEnemyPosition, unitManager.groupCenter) > 5f)
                {
                    unitManager.underGeneralCommand = true;
                    unitManager.MoveFormation(nearestEnemyPosition);
                }
                else
                {
                    unitManager.underGeneralCommand = false;
                }
            }
        }
    }

    private Vector3 FindNearestEnemy(Vector3 unitPosition)
    {
        float closestDistance = float.MaxValue;
        Vector3 closestEnemyPosition = Vector3.zero;

        foreach (string tag in enemyTag)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                UnitManager enemyUnitManager = enemy.GetComponent<UnitManager>();
                Vector3 enemyPosition = enemyUnitManager.groupCenter;
                float distance = Vector3.Distance(unitPosition, enemyPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemyPosition = enemyPosition;
                }
            }
        }

        return closestEnemyPosition;
    }

}
