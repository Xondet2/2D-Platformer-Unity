using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro en tu proyecto

public class UI_Inventario : MonoBehaviour
{
    [Header("Referencias de Texto")]
    [SerializeField] private TextMeshProUGUI textoMonedas;
    [SerializeField] private TextMeshProUGUI textoGemas;
    [SerializeField] private TextMeshProUGUI textoLlaves;

    private InventarioPlayer inventario;

    private void Start()
    {
        // Buscamos al jugador para suscribirnos a sus eventos
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inventario = player.GetComponent<InventarioPlayer>();
            
            if (inventario != null)
            {
                // Suscribirse a eventos
                inventario.OnMonedasChanged += ActualizarTextoMonedas;
                inventario.OnGemasChanged += ActualizarTextoGemas;
                inventario.OnLlavesChanged += ActualizarTextoLlaves;

                // Inicializar valores actuales
                ActualizarTextoMonedas(inventario.GetMonedas());
                ActualizarTextoGemas(inventario.GetGemas());
                ActualizarTextoLlaves(inventario.GetLlaves());
            }
        }
    }

    private void OnDestroy()
    {
        // Desvincular eventos para evitar errores de memoria
        if (inventario != null)
        {
            inventario.OnMonedasChanged -= ActualizarTextoMonedas;
            inventario.OnGemasChanged -= ActualizarTextoGemas;
            inventario.OnLlavesChanged -= ActualizarTextoLlaves;
        }
    }

    private void ActualizarTextoMonedas(int cantidad)
    {
        if (textoMonedas != null) textoMonedas.text = cantidad.ToString();
    }

    private void ActualizarTextoGemas(int cantidad)
    {
        if (textoGemas != null) textoGemas.text = cantidad.ToString();
    }

    private void ActualizarTextoLlaves(int cantidad)
    {
        if (textoLlaves != null) textoLlaves.text = cantidad.ToString();
    }
}
