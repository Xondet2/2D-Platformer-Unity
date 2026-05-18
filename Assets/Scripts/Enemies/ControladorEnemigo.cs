using UnityEngine;

public class ControladorEnemigo : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private Transform[] puntosPatrulla;
    private int puntoActual = 0;

    [Header("Detección de Bordes")]
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float distanciaDeteccion = 0.5f;
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private Animator animator;
    private bool mirandoDerecha = true;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (capaSuelo.value == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No se ha asignado ninguna capa a 'capaSuelo'. La detección de bordes no funcionará.");
        }
    }

    private void Update()
    {
        Patrullar();
    }

    private void Patrullar()
    {
        if (rb == null) return;
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        // Chequeo de seguridad: ¿Hay suelo delante?
        if (detectorSuelo != null)
        {
            RaycastHit2D haySuelo = Physics2D.Raycast(detectorSuelo.position, Vector2.down, distanciaDeteccion, capaSuelo);
            if (haySuelo.collider == null)
            {
                // Si no hay suelo, forzar cambio de punto de patrulla para dar la vuelta
                puntoActual = (puntoActual + 1) % puntosPatrulla.Length;
                return;
            }
        }

        Transform objetivo = puntosPatrulla[puntoActual];
        if (objetivo == null) return;
        float distanciaX = objetivo.position.x - transform.position.x;
        float direccionX = Mathf.Sign(distanciaX);

        if (Mathf.Abs(distanciaX) > 0.5f) // Umbral más generoso
        {
            rb.linearVelocity = new Vector2(direccionX * velocidad, rb.linearVelocity.y);

            if (animator != null) animator.SetFloat(SpeedHash, 1f);

            // Invertimos la lógica del Flip si el sprite camina de espaldas
            if (direccionX > 0 && !mirandoDerecha) Flip();
            else if (direccionX < 0 && mirandoDerecha) Flip();
        }
        else
        {
            puntoActual = (puntoActual + 1) % puntosPatrulla.Length;
        }
    }

    private void Flip()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmos()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < puntosPatrulla.Length; i++)
        {
            if (puntosPatrulla[i] != null)
            {
                // Dibujar el punto
                Gizmos.DrawWireSphere(puntosPatrulla[i].position, 0.3f);

                // Dibujar línea hacia el siguiente punto
                int siguiente = (i + 1) % puntosPatrulla.Length;
                if (puntosPatrulla[siguiente] != null)
                {
                    Gizmos.DrawLine(puntosPatrulla[i].position, puntosPatrulla[siguiente].position);
                }
            }
        }
    }
}
