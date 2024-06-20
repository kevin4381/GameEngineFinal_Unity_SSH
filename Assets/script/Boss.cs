using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour , IDamageable
{
    public int maxHealth = 50;
    private int currentHealth;
    public float detectionRange = 10f;
    public float attackInterval = 10f;
    public int attackDamage = 5;
    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(2f, 2f);
    public LayerMask playerLayer;

    private bool isDead = false;
    private bool isAttacking = false;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public GameObject clearUI;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        StartCoroutine(ClearUIRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackInterval);

        isAttacking = false;
    }

    public void OnAttackHit()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackBoxSize, 0f, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<Player>().TakeDamage(attackDamage);
        }
    }

    IEnumerator ClearUIRoutine()
    {
        yield return new WaitForSeconds(5f);
        clearUI.SetActive(true);
    }

    public void ActivateBoss()
    {
        gameObject.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackBoxSize);
    }
}