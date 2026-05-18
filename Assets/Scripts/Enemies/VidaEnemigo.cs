using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Salud")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int vidaActual;

    [Header("Animación")]
    private Animator animator;
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");

    private bool estaMuerto = false;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
    }

    public void TomarDaño(int daño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        // Disparar animación de daño
        if (animator != null)
        {
            animator.SetTrigger(HurtHash);
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        estaMuerto = true;

        // Disparar animación de muerte
        if (animator != null)
        {
            animator.SetBool(IsDeadHash, true);
            animator.SetTrigger(DieHash);
        }

        // En lugar de desactivar el Collider (que lo hace caer), 
        // lo hacemos estático para que se quede en el suelo
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Desactivar el script de movimiento
        ControladorEnemigo movementScript = GetComponent<ControladorEnemigo>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Opcional: Destruir después de un tiempo
        Destroy(gameObject, 3f);
    }

    public int GetVidaActual() => vidaActual;
    public bool EstaMuerto() => estaMuerto;
}
