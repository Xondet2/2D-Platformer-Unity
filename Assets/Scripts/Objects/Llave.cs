using UnityEngine;

public class Llave : MonoBehaviour
{
    [SerializeField] private GameObject efectoRecogida;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventarioPlayer inventario = other.GetComponent<InventarioPlayer>();
            if (inventario == null)
            {
                inventario = other.gameObject.AddComponent<InventarioPlayer>();
            }

            inventario.AñadirLlave();

            if (efectoRecogida != null)
            {
                Instantiate(efectoRecogida, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
