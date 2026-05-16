using UnityEngine;

public class CurarAlTocar : MonoBehaviour
{
    [SerializeField] private int cantidadCuracion;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out VidaPlayer vidaPlayer))
        {
            vidaPlayer.CurarVida(cantidadCuracion);
        }
    }
}