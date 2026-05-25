using System;
using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Animator animator;
    [SerializeField] private EstadosEnemigo estadoActual;

    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 2f;
    [SerializeField] private Transform controladorFrente;
    [SerializeField] private float radioDeteccionMuro = 0.2f;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float radioDeteccionSuelo = 0.2f;
    [SerializeField] private LayerMask capasSuelo;
    
    [SerializeField] private int direccionActual = -1; // -1 Izquierda, 1 Derecha
    private float tiempoEsperaGiro = 0.5f;
    private float ultimoGiro;

    [Header("Ataque y Detección")]
    [SerializeField] private float rangoDeteccion = 4f;
    [SerializeField] private float rangoDeAtaque = 1.2f;
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private int dañoAtaque = 10;
    [SerializeField] private LayerMask capasJugador;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;
    [SerializeField] private float retrasoDañoAtaque = 0.4f;
    private float tiempoSiguienteAtaque;

    [Header("Daño por Contacto")]
    [SerializeField] private int dañoPorContacto = 5;
    [SerializeField] private float intervaloInmunidadContacto = 1f;
    private float tiempoSiguienteDañoContacto;

    [Header("Esperar")]
    [SerializeField] private float tiempoAEsperar = 1f;
    private float tiempoAEsperarActual;

    private void Awake()
    {
        if (rb2D == null) rb2D = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        estadoActual = EstadosEnemigo.Correr;
    }

    private void Update()
    {
        if (estadoActual == EstadosEnemigo.Muerte) return;

        ManejarTimers();
        DetectarJugador();
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        if (estadoActual == EstadosEnemigo.Muerte) return;

        switch (estadoActual)
        {
            case EstadosEnemigo.Correr:
                Patrullar();
                break;
            case EstadosEnemigo.Esperar:
                rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
                if (tiempoAEsperarActual <= 0)
                {
                    estadoActual = EstadosEnemigo.Correr;
                }
                break;
            case EstadosEnemigo.Atacar:
                rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
                LogicaAtaque();
                break;
        }
    }

    private void Patrullar()
    {
        // Aplicamos velocidad constante
        rb2D.linearVelocity = new Vector2(direccionActual * velocidadCaminar, rb2D.linearVelocity.y);

        // Detección circular usando ContactFilter2D para ignorar triggers y al propio objeto
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.SetLayerMask(capasSuelo);
        filtro.useLayerMask = true;
        filtro.useTriggers = false;

        Collider2D[] resultadosMuro = new Collider2D[5];
        Collider2D[] resultadosSuelo = new Collider2D[5];

        int numMuro = Physics2D.OverlapCircle(controladorFrente.position, radioDeteccionMuro, filtro, resultadosMuro);
        
        // Adelantamos un poco la detección del suelo para que sea más "estricta" antes de llegar al borde vacío
        Vector2 posicionSueloAvanzada = (Vector2)controladorSuelo.position + (Vector2.right * direccionActual * 0.1f);
        int numSuelo = Physics2D.OverlapCircle(posicionSueloAvanzada, radioDeteccionSuelo, filtro, resultadosSuelo);

        bool hayMuro = false;
        for (int i = 0; i < numMuro; i++)
        {
            if (resultadosMuro[i].gameObject != gameObject)
            {
                hayMuro = true;
                break;
            }
        }

        bool haySuelo = false;
        for (int i = 0; i < numSuelo; i++)
        {
            if (resultadosSuelo[i].gameObject != gameObject)
            {
                haySuelo = true;
                break;
            }
        }

        // Si detecta un muro O NO detecta suelo, girar
        if ((hayMuro || !haySuelo) && Time.time > ultimoGiro + tiempoEsperaGiro)
        {
            CambiarDireccion();
        }

        OrientarSprite(direccionActual);
    }

    private void CambiarDireccion()
    {
        direccionActual *= -1;
        ultimoGiro = Time.time;
        estadoActual = EstadosEnemigo.Esperar;
        tiempoAEsperarActual = tiempoAEsperar;
        rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
    }

    private void LogicaAtaque()
    {
        Collider2D jugador = Physics2D.OverlapCircle(transform.position, rangoDeteccion, capasJugador);

        if (jugador == null || (jugador.TryGetComponent(out VidaPlayer vp) && vp.GetVidaActual() <= 0))
        {
            CambiarAEstadoEsperar();
            return;
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.transform.position);
        if (distanciaAlJugador > rangoDeAtaque + 0.3f) 
        {
            CambiarAEstadoEsperar();
            return;
        }

        float dirAlJugador = jugador.transform.position.x - transform.position.x;
        int nuevaDir = dirAlJugador > 0 ? 1 : -1;
        if (nuevaDir != direccionActual)
        {
            direccionActual = nuevaDir;
            OrientarSprite(direccionActual);
        }

        if (tiempoSiguienteAtaque <= 0)
        {
            animator.SetTrigger("Atacar");
            tiempoSiguienteAtaque = tiempoEntreAtaques;
            StartCoroutine(AplicarDañoConRetraso());
        }
    }

    private System.Collections.IEnumerator AplicarDañoConRetraso()
    {
        yield return new WaitForSeconds(retrasoDañoAtaque);

        // Volvemos a detectar al jugador en el momento del impacto
        Collider2D jugador = Physics2D.OverlapCircle(controladorAtaque.position, rangoDeAtaque, capasJugador);
        if (jugador != null && jugador.TryGetComponent(out VidaPlayer vidaPlayer))
        {
            if (vidaPlayer.GetVidaActual() > 0)
            {
                vidaPlayer.TomarDaño(dañoAtaque);
            }
        }
    }

    private void DetectarJugador()
    {
        if (estadoActual == EstadosEnemigo.Atacar || estadoActual == EstadosEnemigo.Muerte) return;

        Collider2D jugador = Physics2D.OverlapCircle(transform.position, rangoDeteccion, capasJugador);
        if (jugador != null && jugador.TryGetComponent(out VidaPlayer vp) && vp.GetVidaActual() > 0)
        {
            float distancia = Vector2.Distance(transform.position, jugador.transform.position);
            if (distancia <= rangoDeAtaque)
            {
                estadoActual = EstadosEnemigo.Atacar;
            }
            else
            {
                float dirAlJugador = jugador.transform.position.x - transform.position.x;
                int nuevaDir = dirAlJugador > 0 ? 1 : -1;
                if (nuevaDir != direccionActual && Time.time > ultimoGiro + tiempoEsperaGiro)
                {
                    direccionActual = nuevaDir;
                    ultimoGiro = Time.time;
                    OrientarSprite(direccionActual);
                }
            }
        }
    }

    private void CambiarAEstadoEsperar()
    {
        estadoActual = EstadosEnemigo.Esperar;
        tiempoAEsperarActual = 0.5f;
        rb2D.linearVelocity = Vector2.zero;
    }

    private void ManejarTimers()
    {
        if (tiempoAEsperarActual > 0) tiempoAEsperarActual -= Time.deltaTime;
        if (tiempoSiguienteAtaque > 0) tiempoSiguienteAtaque -= Time.deltaTime;
        if (tiempoSiguienteDañoContacto > 0) tiempoSiguienteDañoContacto -= Time.deltaTime;
    }

    private void OrientarSprite(float direccion)
    {
        if (direccion > 0) transform.eulerAngles = new Vector3(0, 180, 0);
        else if (direccion < 0) transform.eulerAngles = Vector3.zero;
    }

    private void ActualizarAnimaciones()
    {
        animator.SetFloat("VelocidadHorizontal", Mathf.Abs(rb2D.linearVelocity.x));
    }

    public void DesactivarMovimiento()
    {
        estadoActual = EstadosEnemigo.Muerte;
        rb2D.linearVelocity = Vector2.zero;
        rb2D.bodyType = RigidbodyType2D.Static;
        this.enabled = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out VidaPlayer vidaPlayer))
        {
            if (tiempoSiguienteDañoContacto <= 0)
            {
                vidaPlayer.TomarDaño(dañoPorContacto);
                tiempoSiguienteDañoContacto = intervaloInmunidadContacto;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorFrente)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorFrente.position, radioDeteccionMuro);
        }
        if (controladorSuelo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(controladorSuelo.position, radioDeteccionSuelo);
        }
        if (controladorAtaque)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(controladorAtaque.position, rangoDeAtaque);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
