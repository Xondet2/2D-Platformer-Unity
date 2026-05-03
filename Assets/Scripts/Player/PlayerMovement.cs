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

    private Rigidbody2D rb;
    private Animator animator;

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
    }

    void Update()
    {
        CheckGround();

        if (!isAttacking)
        {
            HandleMovement();
            HandleJump();
        }

        HandleAttackInput();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        animator.SetBool("isGrounded", isGrounded);
    }

    void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
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

        animator.SetBool("isAttacking", true);
        animator.SetInteger("comboStep", comboStep);
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

            animator.SetInteger("comboStep", comboStep);
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

        animator.SetBool("isAttacking", false);
        animator.SetInteger("comboStep", 0);
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
