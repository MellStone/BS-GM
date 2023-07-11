using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public int damage;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayer;
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }
    private void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyBehavior>().TakeDamage(damage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
