using UnityEngine;

public class Militia : Soldier
{
    int attackDamage = 10;
    float attackCooldown = 1f;
    float lastAttackTime;

    protected override void Start()
    {
        attackRangeSoldier = 1;
        engageRangeSoldier = 2;
        health = 200;
        
        base.Start();
        
        if (gameObject.CompareTag("RedMilitia"))
        {
            enemyTag.Add("BlueMilitia");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else if (gameObject.CompareTag("BlueMilitia"))
        {
            enemyTag.Add("RedMilitia");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");
        }
    }

    override public void Attack(GameObject enemy)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Soldier enemySoldier = enemy.GetComponent<Soldier>();
            if (enemySoldier != null)
            {
                enemySoldier.TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }
}
