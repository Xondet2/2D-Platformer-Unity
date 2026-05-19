using UnityEngine;
using UnityEngine.UI;

public class BarraVidaEnemigo : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private VidaEnemigo vidaEnemigo;
    [SerializeField] private bool ocultarSiLlena = true;

    private void Awake()
    {
        if (slider == null) slider = GetComponentInChildren<Slider>();
        if (vidaEnemigo == null) vidaEnemigo = GetComponentInParent<VidaEnemigo>();
    }

    private void OnEnable()
    {
        if (vidaEnemigo != null)
        {
            vidaEnemigo.OnVidaCambiada += ActualizarBarra;
        }
    }

    private void OnDisable()
    {
        if (vidaEnemigo != null)
        {
            vidaEnemigo.OnVidaCambiada -= ActualizarBarra;
        }
    }

    private void ActualizarBarra(int vidaActual, int vidaMaxima)
    {
        if (slider != null)
        {
            float porcentaje = (float)vidaActual / vidaMaxima;
            slider.value = porcentaje;
            
            Debug.Log($"[BarraVidaEnemigo] Actualizando barra de {gameObject.name}: {vidaActual}/{vidaMaxima} ({porcentaje * 100}%)");

            if (ocultarSiLlena)
            {
                gameObject.SetActive(vidaActual < vidaMaxima && vidaActual > 0);
            }
        }
    }
}
