using UnityEngine;

public class CajaDestructible : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] prefabsRecompensas;
    [SerializeField] private Transform puntoAparicionRecompensa;

    [Header("Configuración")]
    [SerializeField] private int cantidadMinima = 1;
    [SerializeField] private int cantidadMaxima = 3;
    [SerializeField] private float fuerzaExplosion = 4f;
    [SerializeField] private float tiempoParaDesaparecer = 2f;

    private bool haSidoDestruida = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    // El Player usa SendMessage("TomarDaño", ...) en su ataque
    public void TomarDaño(int cantidad)
    {
        if (haSidoDestruida) return;
        
        DestruirCaja();
    }

    private void DestruirCaja()
    {
        haSidoDestruida = true;

        // Cambiar al siguiente frame (animación de rota)
        if (animator != null)
        {
            animator.SetTrigger("Romper");
        }

        // Soltar recompensas
        SoltarItems();

        // Desactivar el collider para que el jugador no choque con la caja rota
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // Desaparecer después de 2 segundos
        Destroy(gameObject, tiempoParaDesaparecer);
    }

    private void SoltarItems()
    {
        if (prefabsRecompensas == null || prefabsRecompensas.Length == 0) return;

        int cantidad = Random.Range(cantidadMinima, cantidadMaxima + 1);

        for (int i = 0; i < cantidad; i++)
        {
            GameObject prefabAleatorio = prefabsRecompensas[Random.Range(0, prefabsRecompensas.Length)];
            Vector3 posicion = puntoAparicionRecompensa != null ? puntoAparicionRecompensa.position : transform.position;
            
            // 🔥 CORRECCIÓN: Forzar que el item aparezca un poco más adelante en el eje Z para que no lo tape la caja
            posicion.z = -0.1f;
            
            GameObject item = Instantiate(prefabAleatorio, posicion, Quaternion.identity);
            
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direccionSalto = new Vector2(Random.Range(-0.6f, 0.6f), 1f).normalized;
                rb.AddForce(direccionSalto * fuerzaExplosion, ForceMode2D.Impulse);
            }
        }
    }
}
