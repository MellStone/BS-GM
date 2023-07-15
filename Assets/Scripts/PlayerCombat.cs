using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public int lightAttackDamage;
    public int heavyAttackDamage;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayer;

    private bool isAttacking = false;
    private Animator animator;

    public float health;

    private void Start()
    {
        //animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            LightAttack();
        }
        if (Input.GetButtonDown("Fire3") && !isAttacking)
        {
            HeavyAttack();
        }
    }
    private void LightAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("LightAttack");

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyBehavior>().TakeDamage(lightAttackDamage);
        }

        //float attackDelay = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(AttackCooldown(0.3f));
    }
    private void HeavyAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("HeavyAttack");

        // Выполняем проверку попадания по врагам
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            // Применяем урон врагам
            EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.TakeDamage(heavyAttackDamage);
            }
        }

        // Задержка перед возможностью следующей атаки
        //float attackDelay = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(AttackCooldown(0.4f));
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }




    ///
    /// 
    ///
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    ///
    ///
    ///

    private IEnumerator AttackCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }
}
