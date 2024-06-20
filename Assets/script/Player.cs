using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashSpeed = 30f;
    public float dashDuration = 0.05f;

    private bool isGrounded;
    public Rigidbody2D rb { get; private set; }
    private SpriteRenderer spriteRenderer;
    public Animator animator { get; private set; }
    public int maxHealth = 5;
    public int currentHealth;
    private bool isInvincible = false;
    public float invincibleDuration = 1f;
    public float flashInterval = 0.1f;

    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(1f, 1f);
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    private bool isAttacking = false;

    private bool isDashing = false;
    private int facingDirection = 1;

    public bool canMove { get; set; } = true;

    // 강공격 관련 변수
    public Transform strongAttackPoint;
    public Vector2 strongAttackBoxSize = new Vector2(2f, 2f);
    public int strongAttackDamage = 3;
    private bool isStrongAttacking = false;

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        FindObjectOfType<PlayerHealthBar>().UpdateHealthBar(currentHealth);
    }

    void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!isAttacking && !isDashing && !isStrongAttacking)
        {
            float move = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

            if (move != 0)
            {
                animator.SetBool("isMoving", true);

                if (move > 0)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    facingDirection = 1;
                }
                else if (move < 0)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    facingDirection = -1;
                }
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("isJumping", true);
        }

        if (isGrounded && rb.velocity.y <= 0)
        {
            animator.SetBool("isJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StrongAttack();
        }

        if (Input.GetKeyDown(KeyCode.Z) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    void Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack");
    }

    void StrongAttack()
    {
        isStrongAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("StrongAttack");
    }

    public void OnAttackHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBoxSize, 0f, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    public void OnStrongAttackHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(strongAttackPoint.position, strongAttackBoxSize, 0f, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(strongAttackDamage);
            }
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
    }

    public void OnStrongAttackEnd()
    {
        isStrongAttacking = false;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetTrigger("Dash");

        float originalSpeed = moveSpeed;
        moveSpeed = dashSpeed;

        rb.velocity = new Vector2(facingDirection * dashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        moveSpeed = originalSpeed;
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackBoxSize);

        if (strongAttackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(strongAttackPoint.position, strongAttackBoxSize);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; 

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(1);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player took damage. Current health: {currentHealth}");

        FindObjectOfType<PlayerHealthBar>().UpdateHealthBar(currentHealth);

        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            Debug.Log("Player Dead");
            canMove = false;
            rb.velocity = Vector2.zero;
            isDead = true;

            FindObjectOfType<GameOverManager>().ShowGameOver();
            return;
        }

        StartCoroutine(InvincibilityCoroutine());
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        FindObjectOfType<PlayerHealthBar>().UpdateHealthBar(currentHealth);
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float invincibleEndTime = Time.time + invincibleDuration;

        while (Time.time < invincibleEndTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}