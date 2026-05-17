using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpForce = 16f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;

    [Header("Combat")]
    public int maxCombo = 2;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private VidaPlayer vidaPlayer;

    // Animator parameter hashes for performance
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int ComboStepHash = Animator.StringToHash("comboStep");

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private int comboStep = 0;
    private bool isAttacking = false;
    private bool canQueueNext = false;
    private bool inputBuffered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        vidaPlayer = GetComponent<VidaPlayer>();
    }

    void Update()
    {
        if (vidaPlayer != null && vidaPlayer.GetVidaActual() <= 0) return;

        HandleMovementInput();
        HandleAttackInput();
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyMovement();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        animator.SetBool(IsGroundedHash, isGrounded);
    }

    void HandleMovementInput()
    {
        if (isAttacking) 
        {
            moveInput = 0;
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void ApplyMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartAttack();
            }
            else if (canQueueNext)
            {
                inputBuffered = true;
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        comboStep = 1;
        inputBuffered = false;
        canQueueNext = false;

        animator.SetBool(IsAttackingHash, true);
        animator.SetInteger(ComboStepHash, comboStep);

        // Aplicar daño al iniciar el ataque (o puedes llamar a PerformDamage desde un Animation Event)
        PerformDamage();
    }

    public void PerformDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out VidaEnemigo enemyHealth))
            {
                enemyHealth.TomarDaño(attackDamage);
            }
        }
    }

    // 🔓 Se llama desde Animation Event
    public void OpenComboWindow()
    {
        canQueueNext = true;
    }

    // 🔒 Se llama desde Animation Event
    public void CloseComboWindow()
    {
        canQueueNext = false;
    }

    // 🛑 Se llama al FINAL de CADA animación de ataque
    public void EndAttack()
    {
        if (inputBuffered && comboStep < maxCombo)
        {
            comboStep++;
            inputBuffered = false;
            canQueueNext = false;

            animator.SetInteger(ComboStepHash, comboStep);
        }
        else
        {
            ResetAttack();
        }
    }

    void ResetAttack()
    {
        comboStep = 0;
        isAttacking = false;
        inputBuffered = false;
        canQueueNext = false;

        animator.SetBool(IsAttackingHash, false);
        animator.SetInteger(ComboStepHash, 0);
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
