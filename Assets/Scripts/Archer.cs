using UnityEngine;

public class Archer : Soldier
{
    [SerializeField]
    GameObject prefabArrow;

    float arrowVelocity = 700f;
    float elapsedSeconds = 0f;

    protected override void Start()
    {
        attackRangeSoldier = 20;
        engageRangeSoldier = 2;
        health = 10;
        
        base.Start();
        
        if (gameObject.CompareTag("RedArcher"))
        {
            enemyTag.Add("BlueMilitia");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else
        {
            enemyTag.Add("RedMilitia");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");
        }
    }

    override public void Attack(GameObject enemy)
    {
        elapsedSeconds += Time.deltaTime;

        if (elapsedSeconds > 1f)
        {
            if (enemy == null) return;

            Vector3 arrowDirection = enemy.transform.position - transform.position;
            arrowDirection.Normalize();

            GameObject arrow = Instantiate(prefabArrow, transform.position, transform.rotation);
            Vector3 change = new Vector3(0, 1f, 0);
            Vector3 final = change + arrowDirection;
            final = final.normalized;
            arrow.GetComponent<Rigidbody>().AddForce(arrowVelocity * final);

            Physics.IgnoreCollision(arrow.GetComponent<Collider>(), GetComponent<Collider>());

            elapsedSeconds = 0;
        }
    }
}
