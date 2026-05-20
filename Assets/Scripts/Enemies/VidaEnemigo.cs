using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;
    [SerializeField] private Animator animator;
    [SerializeField] private MovimientoEnemigo movimientoEnemigo;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        if (animator == null) animator = GetComponent<Animator>();
        if (movimientoEnemigo == null) movimientoEnemigo = GetComponent<MovimientoEnemigo>();
    }

    public void TomarDaño(int cantidadDeDaño)
    {
        if (vidaActual <= 0) return;

        vidaActual -= cantidadDeDaño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (animator != null)
        {
            animator.SetTrigger("Herido");
        }

        if (vidaActual <= 0)
        {
            Muerte();
        }
    }

    private void Muerte()
    {
        if (animator != null)
        {
            animator.SetTrigger("Muerte");
        }

        if (movimientoEnemigo != null)
        {
            movimientoEnemigo.DesactivarMovimiento();
        }

        // Desactivar colisionadores para que no estorbe ni reciba más daño
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
        }

        // Opcional: Destruir después de un tiempo para que se vea la animación
        Destroy(gameObject, 2f);
    }
}