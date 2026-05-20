// ==========================================
// MENU DE OPCIONES ESTILO RPG / DIABLO
// Unity + C#
// ==========================================

using UnityEngine;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    [Header("SLIDERS")]
    public Slider volumenGeneral;
    public Slider efectosSonido;

    [Header("TEXTOS")]
    public Text txtVolumenGeneral;
    public Text txtEfectos;

    [Header("AUDIOS")]
    public AudioSource musica;
    public AudioSource efectos;

    void Start()
    {
        // Cargar valores guardados
        volumenGeneral.value = PlayerPrefs.GetFloat("VolumenGeneral", 1f);
        efectosSonido.value = PlayerPrefs.GetFloat("EfectosSonido", 0.7f);

        ActualizarVolumenGeneral(volumenGeneral.value);
        ActualizarEfectos(efectosSonido.value);
    }

    // ======================================
    // VOLUMEN GENERAL
    // ======================================
    public void ActualizarVolumenGeneral(float valor)
    {
        AudioListener.volume = valor;

        // Sincronizar slider si se llama desde código
        if (volumenGeneral.value != valor)
            volumenGeneral.value = valor;

        txtVolumenGeneral.text =
            Mathf.RoundToInt(valor * 100) + "%";

        PlayerPrefs.SetFloat("VolumenGeneral", valor);
    }

    public void SilenciarGeneral() => ActualizarVolumenGeneral(0f);
    public void MaximoGeneral() => ActualizarVolumenGeneral(1f);

    // ======================================
    // EFECTOS DE SONIDO
    // ======================================
    public void ActualizarEfectos(float valor)
    {
        if (efectos != null)
            efectos.volume = valor;

        // Sincronizar slider si se llama desde código
        if (efectosSonido.value != valor)
            efectosSonido.value = valor;

        txtEfectos.text =
            Mathf.RoundToInt(valor * 100) + "%";

        PlayerPrefs.SetFloat("EfectosSonido", valor);
    }

    public void SilenciarEfectos() => ActualizarEfectos(0f);
    public void MaximoEfectos() => ActualizarEfectos(1f);

    // ======================================
    // BOTON VOLVER
    // ======================================
    public void Volver()
    {
        PlayerPrefs.Save(); // Asegurar que los cambios se guarden
        gameObject.SetActive(false);

        // También puedes cargar otra escena:
        // SceneManager.LoadScene("MenuPrincipal");
    }
    public AudioClip sonidoHover;
    public AudioSource uiAudio;

    public void ReproducirTick()
    {
        uiAudio.PlayOneShot(sonidoHover);
    }

}