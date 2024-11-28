using UnityEngine;

public class Cavalry : Soldier
{
    int attackDamage = 10;  
    float attackCooldown = 1.5f;
    float lastAttackTime;
    float lastSpecialDamage;
    int chargeDamage = 30;
    int specialAttack = 2;
    float specialRecharge = 15f;

    protected override void Start()
    {
        health = 150;
        
        base.Start();
        
        if (gameObject.CompareTag("RedCavalry"))
        {
            enemyTag.Add("BlueMilitia");
            enemyTag.Add("BlueCavalry");
            enemyTag.Add("BlueArcher");
        }
        else if (gameObject.CompareTag("BlueCavalry"))
        {
            enemyTag.Add("RedMilitia");
            enemyTag.Add("RedCavalry");
            enemyTag.Add("RedArcher");
        }
    }

    override public void Attack(GameObject enemy)
    {
        if(Time.time - lastSpecialDamage <= specialRecharge)
        {
            specialAttack = 2;
        }
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Soldier enemySoldier = enemy.GetComponent<Soldier>();
            if (enemySoldier != null)
            {
                if (specialAttack != 0)
                {
                    enemySoldier.TakeDamage(chargeDamage);
                    specialAttack -= 1;
                    if (specialAttack == 0)
                    {
                        lastSpecialDamage = Time.time;
                    }
                }
                else
                {
                    enemySoldier.TakeDamage(attackDamage);
                }
            }
            lastAttackTime = Time.time;
        }
    }
}
