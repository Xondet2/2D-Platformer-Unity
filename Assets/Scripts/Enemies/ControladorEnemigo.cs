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

    [Header("Combate")]
    [SerializeField] private int dañoDeContacto = 10;
    [SerializeField] private float tiempoEntreAtaques = 1f;
    [SerializeField] private float rangoAtaque = 1.5f;
    private float cronometroAtaque;
    private bool isAttacking = false;

    [Header("Detección")]
    [SerializeField] private float rangoDeteccion = 5f;
    [SerializeField] private LayerMask capaJugador;
    private Transform jugador;

    private enum EstadoEnemigo { Patrulla, Ataque }
    private EstadoEnemigo estadoActual = EstadoEnemigo.Patrulla;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool mirandoDerecha = true;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

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
        if (isKnockedBack) return;

        ActualizarCronometro();
        DetectarJugador();

        switch (estadoActual)
        {
            case EstadoEnemigo.Patrulla:
                Patrullar();
                break;
            case EstadoEnemigo.Ataque:
                GestionarEstadoAtaque();
                break;
        }
    }

    private void ActualizarCronometro()
    {
        if (cronometroAtaque > 0)
        {
            cronometroAtaque -= Time.deltaTime;
        }
    }

    private void DetectarJugador()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, rangoDeteccion, capaJugador);
        
        if (hit != null)
        {
            if (jugador == null) Debug.Log($"[ControladorEnemigo] {gameObject.name} detectó al jugador.");
            jugador = hit.transform;
            estadoActual = EstadoEnemigo.Ataque;
        }
        else
        {
            if (jugador != null) Debug.Log($"[ControladorEnemigo] {gameObject.name} perdió de vista al jugador.");
            jugador = null;
            if (estadoActual == EstadoEnemigo.Ataque)
            {
                estadoActual = EstadoEnemigo.Patrulla;
                isAttacking = false;
                if (animator != null) 
                {
                    animator.SetBool(IsAttackingHash, false);
                    animator.SetFloat(SpeedHash, 0f); // Forzar Idle inicial al volver a patrulla
                }
            }
        }
    }

    private void GestionarEstadoAtaque()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        // 1. Posicionarse frente al jugador
        OrientarHaciaJugador();

        // 2. Si está en rango, atacar
        if (distanciaAlJugador <= rangoAtaque)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null) animator.SetFloat(SpeedHash, 0f);

            if (cronometroAtaque <= 0 && !isAttacking)
            {
                EjecutarAtaque();
            }
        }
        else
        {
            // Acercarse al jugador si no está en rango de ataque (opcional, pero mejora el posicionamiento)
            MoverHaciaJugador();
        }
    }

    private void OrientarHaciaJugador()
    {
        if (jugador == null) return;
        float direccionX = jugador.position.x - transform.position.x;
        if (direccionX > 0 && mirandoDerecha) Flip();
        else if (direccionX < 0 && !mirandoDerecha) Flip();
    }

    private void MoverHaciaJugador()
    {
        if (jugador == null) return;
        float direccionX = Mathf.Sign(jugador.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direccionX * velocidad, rb.linearVelocity.y);
        if (animator != null) animator.SetFloat(SpeedHash, 1f);

        // Asegurar que mira hacia donde camina
        if (direccionX > 0 && mirandoDerecha) Flip();
        else if (direccionX < 0 && !mirandoDerecha) Flip();
    }

    private void EjecutarAtaque()
    {
        isAttacking = true;
        cronometroAtaque = tiempoEntreAtaques;

        // --- PUNTO DE INTEGRACIÓN DE ANIMACIÓN ---
        // Aquí es donde se activa la lógica visual del ataque.
        if (animator != null)
        {
            animator.SetBool(IsAttackingHash, true);
            animator.SetTrigger(AttackTriggerHash); 
        }
        // ------------------------------------------

        // Nota: isAttacking debería resetearse mediante un Animation Event al final de la animación 
        // o mediante un Invoke si no se usan eventos.
        Invoke(nameof(FinalizarAtaque), 0.5f); // Simulación temporal del fin de animación
    }

    private void FinalizarAtaque()
    {
        isAttacking = false;
        if (animator != null) animator.SetBool(IsAttackingHash, false);
    }

    public void AplicarKnockback(Vector2 posicionDaño)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        
        Vector2 direccion = ((Vector2)transform.position - posicionDaño).normalized;
        if (direccion == Vector2.zero) direccion = Vector2.up;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * knockbackForce, ForceMode2D.Impulse);

        Invoke(nameof(ResetKnockback), knockbackDuration);
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enabled) return;
        if (collision.CompareTag("Player") && cronometroAtaque <= 0)
        {
            VidaPlayer vidaPlayer = collision.GetComponentInParent<VidaPlayer>();
            if (vidaPlayer != null)
            {
                Debug.Log($"Goblin atacando al jugador. Daño: {dañoDeContacto}");
                vidaPlayer.TomarDaño(dañoDeContacto, transform.position);
                cronometroAtaque = tiempoEntreAtaques;
            }
            else
            {
                Debug.LogWarning("Se detectó colisión con 'Player' pero no se encontró el componente VidaPlayer.");
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!enabled) return;
        if (collision.gameObject.CompareTag("Player") && cronometroAtaque <= 0)
        {
            VidaPlayer vidaPlayer = collision.gameObject.GetComponentInParent<VidaPlayer>();
            if (vidaPlayer != null)
            {
                Debug.Log($"Goblin chocando con el jugador. Daño: {dañoDeContacto}");
                vidaPlayer.TomarDaño(dañoDeContacto, transform.position);
                cronometroAtaque = tiempoEntreAtaques;
            }
            else
            {
                Debug.LogWarning("Se detectó choque con 'Player' pero no se encontró el componente VidaPlayer.");
            }
        }
    }

    private void Patrullar()
    {
        if (rb == null) return;
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) 
        {
            if (animator != null) animator.SetFloat(SpeedHash, 0f); // Idle si no hay puntos
            return;
        }

        // Chequeo de seguridad: ¿Hay suelo delante?
        if (detectorSuelo != null)
        {
            RaycastHit2D haySuelo = Physics2D.Raycast(detectorSuelo.position, Vector2.down, distanciaDeteccion, capaSuelo);
            if (haySuelo.collider == null)
            {
                // Si no hay suelo, forzar cambio de punto de patrulla para dar la vuelta
                puntoActual = (puntoActual + 1) % puntosPatrulla.Length;
                if (animator != null) animator.SetFloat(SpeedHash, 0f); // Breve pausa al girar
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

            if (animator != null) animator.SetFloat(SpeedHash, 1f); // Caminando

            // Invertimos la lógica del Flip si el sprite camina de espaldas
            if (direccionX > 0 && !mirandoDerecha) Flip();
            else if (direccionX < 0 && mirandoDerecha) Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Detenerse en el punto
            if (animator != null) animator.SetFloat(SpeedHash, 0f); // Animación Idle

            puntoActual = (puntoActual + 1) % puntosPatrulla.Length;
            Debug.Log($"[ControladorEnemigo] {gameObject.name} llegó al punto, cambiando a: {puntoActual}");
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
        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

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
