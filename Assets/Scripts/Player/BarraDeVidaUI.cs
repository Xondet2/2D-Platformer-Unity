using System;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaUI : MonoBehaviour
{
    [SerializeField] private Slider sliderBarraDeVida;
    [SerializeField] private VidaPlayer vidaPlayer;
    
    private void Start()
    {
        vidaPlayer = FindFirstObjectByType<VidaPlayer>();

        vidaPlayer.PlayerTomoDaño += CambiarBarraVidaTomarDaño;
        vidaPlayer.PlayerSeCuro += CambiarBarraVidaCuracion;

        IniciarBarraDeVida(vidaPlayer.GetVidaMaxima(), vidaPlayer.GetVidaActual());
    }

    void OnDisable()
    {
        vidaPlayer.PlayerTomoDaño -= CambiarBarraVidaTomarDaño;
        vidaPlayer.PlayerSeCuro -= CambiarBarraVidaCuracion;
    }

    private void IniciarBarraDeVida(int vidaMaxima, int vidaActual)
    {
        sliderBarraDeVida.maxValue = vidaMaxima;
        sliderBarraDeVida.value = vidaActual;
    }

    private void CambiarBarraVidaTomarDaño(int vidaActual)
    {
        sliderBarraDeVida.value = vidaActual;
    }

    private void CambiarBarraVidaCuracion(int vidaActual)
    {
        sliderBarraDeVida.value = vidaActual;
    }
}