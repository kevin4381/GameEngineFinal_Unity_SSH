using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour, IDamageable
{
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public int maxHealth = 5;
    private int currentHealth;

    public int attackDamage = 1;

    private bool isAttacking = false;
    private bool isDead = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Transform player;

    public Transform attackHitbox;
    public Vector2 attackHitboxSize;

    private EnemyManager enemyManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (!isAttacking)
            {
                MoveTowardsPlayer();
            }
            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

    }

    void MoveTowardsPlayer()
    {
        if (player.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        animator.SetBool("isMoving", true);
    }



    void Flip()
    {
        if (transform.eulerAngles.y == 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f); 

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackHitbox.position, attackHitboxSize, 0);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                hitPlayer.GetComponent<Player>().TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
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
        isAttacking = false;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");

        rb.gravityScale = 0;

        GetComponent<Collider2D>().enabled = false;

        enemyManager.EnemyKilled();

        Destroy(gameObject, 1f); 
    }

    void OnDrawGizmosSelected()
    {
        if (attackHitbox == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackHitbox.position, attackHitboxSize);
    }
}