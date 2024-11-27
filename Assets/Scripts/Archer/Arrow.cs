using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    public int damage = 15;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.up = rb.velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;


        if (hit.CompareTag("RedSoldier") || hit.CompareTag("BlueSoldier"))
        {
            hit.GetComponent<SoldierHealth>().TakeDamage(damage);
        }
        if (hit.CompareTag("RedCavalry") || hit.CompareTag("BlueCavalry"))
        {
            hit.GetComponent<Cavalry>().TakeDamage(damage);
        }
        if (hit.CompareTag("RedArcher") || hit.CompareTag("BlueArcher"))
        {
            hit.GetComponent<Archer>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}