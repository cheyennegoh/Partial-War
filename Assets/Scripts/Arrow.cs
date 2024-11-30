using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    int damage = 20;
    public string archerTag;

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
        
        FriendlyFireOff(archerTag, hit);
        //FriendlyFireOn(hit);
    }

    private void FriendlyFireOn(GameObject hit)
    {
        if (hit.CompareTag("RedMilitia") || hit.CompareTag("BlueMilitia") || hit.CompareTag("RedCavalry") || hit.CompareTag("BlueCavalry") || hit.CompareTag("RedArcher") || hit.CompareTag("BlueArcher"))
        {
            hit.GetComponent<Soldier>().TakeDamage(damage);
        }

        if (!hit.CompareTag("Arrow"))
        {
            Destroy(gameObject);
        }
    }

    private void FriendlyFireOff(string archerTag, GameObject hit)
    {
        if (archerTag == "RedArcher")
        {
            if ( hit.CompareTag("BlueMilitia") || hit.CompareTag("BlueCavalry") || hit.CompareTag("BlueArcher"))
            {
                hit.GetComponent<Soldier>().TakeDamage(damage);
            }

            if (!hit.CompareTag("Arrow"))
            {
                Destroy(gameObject);
            }

        }
        else if (archerTag == "BlueArcher")
        {
            if (hit.CompareTag("RedMilitia") || hit.CompareTag("RedCavalry") || hit.CompareTag("RedArcher"))
            {
                hit.GetComponent<Soldier>().TakeDamage(damage);
            }

            if (!hit.CompareTag("Arrow"))
            {
                Destroy(gameObject);
            }
        }
    }
}