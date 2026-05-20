using System;
using UnityEngine;

public class InventarioPlayer : MonoBehaviour
{
    public Action<int> OnMonedasChanged;
    public Action<int> OnGemasChanged;
    public Action<int> OnLlavesChanged;

    [SerializeField] private int cantidadLlaves = 0;
    [SerializeField] private int monedas = 0;
    [SerializeField] private int gemas = 0;

    public void AñadirLlave()
    {
        cantidadLlaves++;
        Debug.Log($"Llave recogida. Total: {cantidadLlaves}");
        OnLlavesChanged?.Invoke(cantidadLlaves);
    }

    public bool TieneLlave()
    {
        return cantidadLlaves > 0;
    }

    public void UsarLlave()
    {
        if (cantidadLlaves > 0)
        {
            cantidadLlaves--;
            Debug.Log($"Llave usada. Restantes: {cantidadLlaves}");
            OnLlavesChanged?.Invoke(cantidadLlaves);
        }
    }

    public void AñadirPuntos(TipoItem tipo, int cantidad)
    {
        if (tipo == TipoItem.Moneda)
        {
            monedas += cantidad;
            Debug.Log($"¡Moneda recogida! Total Monedas: {monedas}");
            OnMonedasChanged?.Invoke(monedas);
        }
        else if (tipo == TipoItem.Gema)
        {
            gemas += cantidad;
            Debug.Log($"¡Gema recogida! Total Gemas: {gemas}");
            OnGemasChanged?.Invoke(gemas);
        }
    }

    public int GetMonedas() => monedas;
    public int GetGemas() => gemas;
    public int GetLlaves() => cantidadLlaves;
}
