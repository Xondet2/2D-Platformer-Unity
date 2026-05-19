using System;
using UnityEngine;

public class VidaPlayer : MonoBehaviour
{
    public Action<int> PlayerTomoDaño;
    public Action<int> PlayerSeCuro;

    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;

    [Header("Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerabilidad = 1f;
    private bool esInvulnerable = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TomarDaño(int daño, Vector2 posicionDaño)
    {
        if (esInvulnerable) return;

        int vidaTemporal = vidaActual - daño;
        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);
        vidaActual = vidaTemporal;

        Debug.Log($"Jugador recibe {daño} de daño. Vida actual: {vidaActual}");

        PlayerTomoDaño?.Invoke(vidaActual);

        // Aplicar knockback
        if (TryGetComponent(out PlayerMovement movement))
        {
            movement.AplicarKnockback(posicionDaño);
        }

        if (vidaActual <= 0)
        {
            DestruirPlayer();
        }
        else
        {
            StartCoroutine(Invulnerabilidad());
        }
    }

    private System.Collections.IEnumerator Invulnerabilidad()
    {
        esInvulnerable = true;
        
        float tiempoPasado = 0;
        while (tiempoPasado < tiempoInvulnerabilidad)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled; // Parpadeo
            
            yield return new WaitForSeconds(0.1f);
            tiempoPasado += 0.1f;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        
        esInvulnerable = false;
    }

    private void DestruirPlayer()
    {
        Destroy(gameObject);
    }

    public void CurarVida(int curacion)
    {
        int vidaTemporal = vidaActual + curacion;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        PlayerSeCuro?.Invoke(vidaActual);
    }


    public int GetVidaMaxima() => vidaMaxima;
    public int GetVidaActual() => vidaActual;
}