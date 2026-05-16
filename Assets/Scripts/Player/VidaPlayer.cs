using System;
using UnityEngine;

public class VidaPlayer : MonoBehaviour
{
    public Action<int> PlayerTomoDaño;
    public Action<int> PlayerSeCuro;

    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    public void TomarDaño(int daño)
    {
        int vidaTemporal = vidaActual - daño;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        PlayerTomoDaño?.Invoke(vidaActual);

        if (vidaActual <= 0)
        {
            DestruirPlayer();
        }
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