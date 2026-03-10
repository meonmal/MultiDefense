using System.Collections;
using UnityEngine;

public class Hero : Character
{
    public float attackRange = 1.0f;
    public float attackSpeed = 1.0f;
    public Monster target;
    public LayerMask monsterLayer;

    private void Update()
    {
        CheckForMonsters();
    }

    private void CheckForMonsters()
    {
        Collider2D[] monsterInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, monsterLayer);
        if(monsterInRange.Length > 0)
        {
            target = monsterInRange[0].GetComponent<Monster>();
            attackSpeed += Time.deltaTime;
            if(attackSpeed >= 1.0f)
            {
                attackSpeed = 0.0f;
                AttackMonster(target);
            }
        }
        else
        {
            target = null;
        }
    }

    private void AttackMonster(Monster monster)
    {
        AnimatorChange("Attack", true);
        monster.GetDamage(10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
