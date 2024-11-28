using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviour
{
    [SerializeField]
    Slider healthSlider;    // Reference to the health slider UI

    [SerializeField]
    Transform healthBar;  // Reference to the health bar Canvas

    [SerializeField]
    Image bannerIcon;

    List<GameObject> soldiers;
    int currentHealth;     // The current total health of all soldiers

    float heightOffset = 2f;  // Offset to position health bar above the unit
    float bannerOffset = 0.5f;
    float bannerOppacity = 0.8f;

    void Start()
    {
        soldiers = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("RedMilitia") || child.CompareTag("BlueMilitia") || child.CompareTag("RedCavalry") || child.CompareTag("BlueCavalry") || child.CompareTag("RedArcher") || child.CompareTag("BlueArcher"))
            {
                soldiers.Add(child.gameObject);
            }
        }

        currentHealth = CalculateTotalHealth();

        if (healthSlider != null)
        {
            healthSlider.maxValue = currentHealth;
            healthSlider.value = currentHealth;
        }

        SetBannerOpacity(bannerOppacity);
        PositionHealthBar();
    }

    void Update()
    {
        // Recalculate the total health in case any soldier's health has changed
        currentHealth = CalculateTotalHealth();

        // Update the health slider if needed
        if (healthSlider != null && healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (healthBar != null)
        {
            PositionHealthBar();
            PositionBannerIcon();
        }

        if (soldiers.Count == 0 || currentHealth <= 0)
        {
            RemoveHealthBar();

        }
    }

    int CalculateTotalHealth()
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
                continue;
            }

            Soldier soldierHealth = soldier.GetComponent<Soldier>();
            if (soldierHealth != null)
            {
                totalHealth += soldierHealth.health;
            }
        }

        return totalHealth;
    }

    // Position the health bar above the unit and make it face the camera
    void PositionHealthBar()
    {
        if (healthBar == null) return;

        Vector3 groupCenter = CalculateGroupCenter();

        healthBar.position = groupCenter + new Vector3(0, heightOffset, 0);
    }

    void PositionBannerIcon()
    {
        if (bannerIcon == null || healthBar == null) return;

        bannerIcon.transform.position = healthBar.position + new Vector3(0, bannerOffset, 0);
    }

    void SetBannerOpacity(float opacity)
    {
        if (bannerIcon != null)
        {
            Color color = bannerIcon.color;
            color.a = opacity;
            bannerIcon.color = color;
        }
    }

    Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        foreach (GameObject soldier in soldiers)
        {
            center += soldier.transform.position;
        }
        return center / soldiers.Count;
    }

    void RemoveHealthBar()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
    }
}
