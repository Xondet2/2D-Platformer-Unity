using UnityEngine;
public class DañoAlTocar : MonoBehaviour
{
    [SerializeField] private int dañoPorToque;

    [SerializeField] private LayerMask capaObjetivo;

    void OnTriggerEnter2D(Collider2D collision)
    {
        ProcesarDaño(collision.gameObject);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        ProcesarDaño(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarDaño(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ProcesarDaño(collision.gameObject);
    }

    private void ProcesarDaño(GameObject objeto)
    {
        // Solo procesar si el objeto está en la capa objetivo (ej. Player)
        if (((1 << objeto.layer) & capaObjetivo) != 0)
        {
            VidaPlayer vidaPlayer = objeto.GetComponentInParent<VidaPlayer>();
            if (vidaPlayer != null)
            {
                Debug.Log($"Objeto dañino ({gameObject.name}) tocó al jugador. Daño: {dañoPorToque}");
                vidaPlayer.TomarDaño(dañoPorToque, transform.position);
            }
        }
    }
}
