using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviour
{
    public Slider healthSlider;    // Reference to the health slider UI
    public List<GameObject> soldiers;  // List of soldier objects
    public int maxHealth = 900;    // Maximum health of the unit
    private int currentHealth;     // The current total health of all soldiers

    public Transform healthBar;  // Reference to the health bar Canvas (World Space)
    public float heightOffset = 2f;  // Offset to position health bar above the unit

    private void Start()
    {
        // Find all child objects tagged as "Soldier" under the current GameObject
        soldiers = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Soldier"))
            {
                soldiers.Add(child.gameObject);
            }
        }

        // Initialize the health at the start based on the soldier health values
        currentHealth = CalculateTotalHealth();

        // Set the initial health of the health bar
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Position the health bar above the unit (group of soldiers)
        PositionHealthBar();
    }

    private void Update()
    {
        // Recalculate the total health in case any soldier's health has changed
        currentHealth = CalculateTotalHealth();

        // Update the health slider if needed
        if (healthSlider != null && healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        // Position the health bar above the unit in the world
        PositionHealthBar();
    }

    // Calculate the total health by summing the health of each soldier
    private int CalculateTotalHealth()
    {
        int totalHealth = 0;
        foreach (GameObject soldier in soldiers)
        {
            SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
            if (soldierHealth != null)
            {
                totalHealth += soldierHealth.health;
            }
        }
        return totalHealth;
    }

    // Position the health bar above the unit and make it face the camera
    private void PositionHealthBar()
    {
        // Get the center of the unit (group of soldiers)
        Vector3 groupCenter = CalculateGroupCenter();

        // Set the health bar's position above the group center (adjust the Y value for height)
        healthBar.position = groupCenter + new Vector3(0, heightOffset, 0);

    }

    // Calculate the group's center based on the soldiers' positions
    private Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        foreach (GameObject soldier in soldiers)
        {
            center += soldier.transform.position;
        }
        return center / soldiers.Count;
    }
}
