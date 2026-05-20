using UnityEngine;

public class Cofre : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool estaAbierto = false;
    [Header("Recompensas")]
    [SerializeField] private GameObject[] prefabsRecompensas;
    [SerializeField] private int cantidadMinima = 3;
    [SerializeField] private int cantidadMaxima = 6;
    [SerializeField] private float fuerzaExplosion = 5f;
    [SerializeField] private Transform puntoAparicionRecompensa;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (estaAbierto) return;

        if (other.CompareTag("Player"))
        {
            InventarioPlayer inventario = other.GetComponent<InventarioPlayer>();
            
            if (inventario != null && inventario.TieneLlave())
            {
                AbrirCofre(inventario);
            }
            else
            {
                Debug.Log("Cofre bloqueado. Necesitas una llave.");
            }
        }
    }

    private void AbrirCofre(InventarioPlayer inventario)
    {
        estaAbierto = true;
        inventario.UsarLlave();

        if (animator != null)
        {
            animator.SetTrigger("Abrir");
        }

        Invoke(nameof(SoltarItems), 0.5f); // Esperar un poco a que la tapa se abra
    }

    private void SoltarItems()
    {
        Debug.Log($"SoltarItems llamado. Cantidad de prefabs configurados: {prefabsRecompensas.Length}");
        
        if (prefabsRecompensas.Length == 0)
        {
            Debug.LogError("¡ERROR: No hay prefabs de recompensa asignados en el Inspector del Cofre!");
            return;
        }

        int cantidad = Random.Range(cantidadMinima, cantidadMaxima + 1);
        Debug.Log($"Instanciando {cantidad} items...");

        for (int i = 0; i < cantidad; i++)
        {
            GameObject prefabAleatorio = prefabsRecompensas[Random.Range(0, prefabsRecompensas.Length)];
            Vector3 posicion = puntoAparicionRecompensa != null ? puntoAparicionRecompensa.position : transform.position;
            
            GameObject item = Instantiate(prefabAleatorio, posicion, Quaternion.identity);
            Debug.Log($"Item {i+1} instanciado: {item.name}");
            
            // Añadir un pequeño impulso físico
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direccionSalto = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
                rb.AddForce(direccionSalto * fuerzaExplosion, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning($"El item {item.name} no tiene Rigidbody2D, no saltará.");
            }
        }
    }
}
