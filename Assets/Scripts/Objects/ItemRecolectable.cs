using UnityEngine;

public enum TipoItem { Moneda, Gema }

public class ItemRecolectable : MonoBehaviour
{
    [SerializeField] private TipoItem tipo;
    [SerializeField] private int valor = 1;
    [SerializeField] private GameObject efectoRecogida;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Objeto {gameObject.name} detectó colisión con: {other.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            InventarioPlayer inventario = other.GetComponent<InventarioPlayer>();
            if (inventario == null)
            {
                // Intentar buscar en el padre por si el collider está en un objeto hijo
                inventario = other.GetComponentInParent<InventarioPlayer>();
            }

            if (inventario != null)
            {
                Debug.Log($"¡{gameObject.name} recolectado por el jugador!");
                inventario.AñadirPuntos(tipo, valor);
                
                if (efectoRecogida != null)
                {
                    Instantiate(efectoRecogida, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}
