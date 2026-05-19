using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    public System.Action<int, int> OnVidaCambiada;

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

    private void Start()
    {
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
    }

    public void TomarDaño(int daño, Vector2 posicionDaño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        
        Debug.Log($"[VidaEnemigo] {gameObject.name} recibió {daño} de daño. Vida actual: {vidaActual}/{vidaMaxima}");
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);

        // Disparar animación de daño
        if (animator != null)
        {
            animator.SetTrigger(HurtHash);
        }

        // Aplicar knockback
        if (TryGetComponent(out ControladorEnemigo movement))
        {
            movement.AplicarKnockback(posicionDaño);
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (estaMuerto) return; // Bloqueo de re-ejecución
        estaMuerto = true;

        // Desactivar el script de movimiento inmediatamente para evitar cambios de estado
        ControladorEnemigo movementScript = GetComponent<ControladorEnemigo>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Detener cualquier movimiento físico
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Disparar animación de muerte
        if (animator != null)
        {
            animator.SetBool(IsDeadHash, true);
            animator.SetTrigger(DieHash);
        }

        // Opcional: Destruir después de un tiempo
        Destroy(gameObject, 3f);
    }

    public int GetVidaActual() => vidaActual;
    public bool EstaMuerto() => estaMuerto;
}
