using UnityEngine;

public enum TipoItem { Moneda, Gema }

public class ItemRecolectable : MonoBehaviour
{
    [SerializeField] private TipoItem tipo;
    [SerializeField] private int valor = 1;
    [SerializeField] private GameObject efectoRecogida;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventarioPlayer inventario = other.GetComponent<InventarioPlayer>();
            if (inventario != null)
            {
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
