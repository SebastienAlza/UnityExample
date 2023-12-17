using System.Collections;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Character Controller 2D")]
    [Header("Setup Character")]
    public SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public int extraJumpsValue = 1;
    public float jumpForce = 10f;
    public float angleConstraint = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 2.0f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.0f;
    public float durationOfAttack = 0.1f;
    public float OffsetImpactPosition = 0.1f;
    public float attackRange = 1.0f;
    public float attackForce = 10.0f;
    public LayerMask attackableObjectsLayer;

    [Header("Wall Sliding")]
    public float wallSlidingSpeed = 0.5f;
    public LayerMask wallLayer;

    private Rigidbody2D rb2D;
    private Animator animator;
    private bool isGrounded;
    private bool isDashing;
    private bool isAttacking;
    private bool isWallSliding;
    private float attackCooldownTimer;
    private float dashCooldownTimer;
    private int extraJumps;
    private Transform platform;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
        HandleAttack();
        HandleWallSliding();
        UpdateCooldowns();
        UpdateAnimations();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGroundCollisionEnter2D(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        CheckGroundCollisionExit2D(collision);
    }

    private void HandleMovement()
    {
        float moveInputX = Input.GetAxis("Horizontal");

        if (!isWallSliding)
        {
            rb2D.velocity = new Vector2(moveInputX * moveSpeed, rb2D.velocity.y);
        }
        else
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }

        if (moveInputX > 0)
            spriteRenderer.flipX = false;
        else if (moveInputX < 0)
            spriteRenderer.flipX = true;
    }

    private void HandleJump()
    {
        if (isGrounded)
            extraJumps = extraJumpsValue;

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || extraJumps > 0 || isWallSliding))
            Jump();
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0)
            StartCoroutine(Dash());
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isAttacking && attackCooldownTimer <= 0)
            StartCoroutine(Attack());
    }

    private void HandleWallSliding()
    {
        isWallSliding = IsTouchingWall() && !isGrounded;

        if (isWallSliding)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Max(rb2D.velocity.y, -wallSlidingSpeed));
        }
    }

    private bool IsTouchingWall()
    {
        Vector2 direction = new Vector2(spriteRenderer.flipX ? -1 : 1, 0);
        return Physics2D.Raycast(transform.position, direction, 0.5f, wallLayer);
    }

    private void UpdateCooldowns()
    {
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsJumping", !isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(rb2D.velocity.x) * (!isGrounded ? 0 : 1));
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsWallSliding", isWallSliding);
    }

    private void Jump()
    {
        if (rb2D == null) return;
        rb2D.velocity = Vector2.up * jumpForce;
        extraJumps--;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float dashTimeRemaining = dashTime;
        Vector2 dashVelocity = new Vector2(spriteRenderer.flipX ? -dashSpeed : dashSpeed, rb2D.velocity.y);

        while (dashTimeRemaining > 0)
        {
            rb2D.velocity = dashVelocity;
            dashTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        isDashing = false;
        dashCooldownTimer = dashCooldown;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(durationOfAttack);

        Vector3 offsetCollision = new Vector3(transform.position.x + (spriteRenderer.flipX ? -OffsetImpactPosition : OffsetImpactPosition), transform.position.y, transform.position.z);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(offsetCollision, attackRange, attackableObjectsLayer);
        foreach (Collider2D hit in hitObjects)
        {
            Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
            if (hitRb != null)
            {
                Vector2 direction = hit.transform.position - offsetCollision;
                hitRb.AddForce(direction.normalized * attackForce, ForceMode2D.Impulse);
            }
        }

        isAttacking = false;
        attackCooldownTimer = attackCooldown;
    }

    void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;
        Vector3 offsetCollision = new Vector3(transform.position.x + (spriteRenderer.flipX ? -OffsetImpactPosition : OffsetImpactPosition), transform.position.y, transform.position.z);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(offsetCollision, attackRange);
    }

    private void CheckGroundCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        float angle = Vector2.Angle(normal, Vector2.up);

        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform")) && angle < angleConstraint)
        {
            isGrounded = true;
            if (collision.gameObject.CompareTag("MovingPlatform")) AttachToPlatform(collision.transform);
        }
    }

    private void CheckGroundCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform"))
        {
            isGrounded = false;
            if (collision.transform == platform) DetachFromPlatform();
        }
    }

    private void AttachToPlatform(Transform platformTransform)
    {
        platform = platformTransform;
        transform.parent = platform;
    }

    private void DetachFromPlatform()
    {
        transform.parent = null;
        platform = null;
    }
}
