using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    int damage = 15;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.up = rb.velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        
        if (hit.CompareTag("RedMilitia") || hit.CompareTag("BlueMilitia") || hit.CompareTag("RedCavalry") || hit.CompareTag("BlueCavalry") || hit.CompareTag("RedArcher") || hit.CompareTag("BlueArcher"))
        {
            hit.GetComponent<Soldier>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}