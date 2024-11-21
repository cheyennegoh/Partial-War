using UnityEngine;

public class SoldierHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle soldier death (e.g., destroy the game object)
        Destroy(gameObject);
    }
}