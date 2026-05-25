using UnityEngine;

public class VidaPlayerAnimator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private VidaPlayer vidaPlayer;

    [Header("Parámetros del Animator")]
    [SerializeField] private string triggerDano = "Hurt";
    [SerializeField] private string triggerMuerte = "Die";
    [SerializeField] private string boolMuerte = "isDead";

    private void OnValidate()
    {
        // Forzamos que el parámetro sea siempre "isDead" para evitar errores de inspector/escena
        if (boolMuerte == "IsDead") boolMuerte = "isDead";
    }

    private void Awake()
    {
        // Si no se asignan en el inspector, intentamos buscarlos en el mismo objeto
        if (animator == null) animator = GetComponent<Animator>();
        if (vidaPlayer == null) vidaPlayer = GetComponent<VidaPlayer>();
    }

    private void OnEnable()
    {
        if (vidaPlayer != null)
        {
            vidaPlayer.PlayerTomoDaño += ManejarDaño;
            vidaPlayer.PlayerSeCuro += ManejarCuracion;
        }
    }

    private void OnDisable()
    {
        if (vidaPlayer != null)
        {
            vidaPlayer.PlayerTomoDaño -= ManejarDaño;
            vidaPlayer.PlayerSeCuro -= ManejarCuracion;
        }
    }

    private void ManejarDaño(int vidaActual)
    {
        ActualizarParametros(vidaActual, true);
    }

    private void ManejarCuracion(int vidaActual)
    {
        ActualizarParametros(vidaActual, false);
    }

    private void ActualizarParametros(int vidaActual, bool fueDano)
    {
        if (animator == null) return;

        if (vidaActual > 0)
        {
            animator.SetBool(boolMuerte, false);
            if (fueDano)
            {
                animator.SetTrigger(triggerDano);
            }
        }
        else
        {
            animator.SetTrigger(triggerMuerte);
            animator.SetBool(boolMuerte, true);
        }
    }
}
