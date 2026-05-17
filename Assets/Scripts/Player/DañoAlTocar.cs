using UnityEngine;
public class DañoAlTocar : MonoBehaviour
{
    [SerializeField] private int dañoPorToque;

    [SerializeField] private LayerMask capaObjetivo;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo procesar si el objeto está en la capa objetivo (ej. Player)
        if (((1 << collision.gameObject.layer) & capaObjetivo) != 0)
        {
            Debug.Log("Goblin tocó al OBJETIVO: " + collision.gameObject.name);
            if (collision.TryGetComponent(out VidaPlayer vidaPlayer))
            {
                vidaPlayer.TomarDaño(dañoPorToque);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & capaObjetivo) != 0)
        {
            Debug.Log("Goblin chocó con el OBJETIVO: " + collision.gameObject.name);
            if (collision.collider.TryGetComponent(out VidaPlayer vidaPlayer))
            {
                vidaPlayer.TomarDaño(dañoPorToque);
            }
        }
    }
}
