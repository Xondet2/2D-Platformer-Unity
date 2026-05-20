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
    [SerializeField] private int maxCombo = 2;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    private Rigidbody2D rb;
    private Animator animator;
    private VidaPlayer vidaPlayer;

    // Animator parameter hashes
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int ComboStepHash = Animator.StringToHash("comboStep");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");

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
        if (isKnockedBack) return;

        HandleMovementInput();
        HandleAttackInput();
    }

    void FixedUpdate()
    {
        CheckGround();

        if (!isKnockedBack)
        {
            ApplyMovement();
        }
    }

    public void AplicarKnockback(Vector2 posicionDaño)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        animator.SetTrigger(HurtHash);

        Vector2 direccion = ((Vector2)transform.position - posicionDaño).normalized;
        if (direccion == Vector2.zero)
            direccion = Vector2.up;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * knockbackForce, ForceMode2D.Impulse);

        Invoke(nameof(ResetKnockback), knockbackDuration);
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
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
    }

    // 🔥 CORREGIDO: ya no depende de VidaEnemigo
    public void PerformDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.SendMessage(
                "TomarDaño",
                attackDamage,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }

    // 🔓 Animation Event
    public void OpenComboWindow()
    {
        canQueueNext = true;
    }

    // 🔒 Animation Event
    public void CloseComboWindow()
    {
        canQueueNext = false;
    }

    // 🛑 Animation Event al final del ataque
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