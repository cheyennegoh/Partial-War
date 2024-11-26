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
        soldiers = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("RedSoldier") || child.CompareTag("BlueSoldier"))
            {
                soldiers.Add(child.gameObject);
            }
        }

        currentHealth = CalculateTotalHealth();

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
        // Only position the health bar if it's still present
        if (healthBar != null)
        {
            PositionHealthBar();
        }

        // If no soldiers are left or the unit's health is zero, remove the health bar
        if (soldiers.Count == 0 || currentHealth <= 0)
        {
            RemoveHealthBar();
        }
    }

    private int CalculateTotalHealth()
    {
        int totalHealth = 0;

        // Iterate over all soldiers, skipping any that have been destroyed
        for (int i = soldiers.Count - 1; i >= 0; i--)  // Loop backward to safely remove elements
        {
            GameObject soldier = soldiers[i];

            // If soldier is destroyed, remove it from the list
            if (soldier == null)
            {
                soldiers.RemoveAt(i);
                continue;  // Skip to the next soldier
            }

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
        if (healthBar == null) return;

        Vector3 groupCenter = CalculateGroupCenter();

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

    // Remove the health bar when the unit has no soldiers or health
    private void RemoveHealthBar()
    {
        // Only try to remove the health bar if it exists
        if (healthBar != null)
        {
            // Destroy the health bar or deactivate it based on your needs
            Destroy(healthBar.gameObject);  // If you want to destroy it
            // healthBar.gameObject.SetActive(false);  // If you want to deactivate it instead
        }
    }
}
