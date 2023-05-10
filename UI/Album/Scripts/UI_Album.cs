using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.UI;

using XS_Utils;

public class UI_Album : MonoBehaviour
{
    public static GameObject fotoZoomed;

    [Header("REFERENCIEs")]
    [SerializeField] Guardat guardat;
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] SaveHex save;
    [SerializeField] Grups grups;
    [SerializeField] Fase faseEnCarregar;
    [SerializeField] Modes modes;
    [SerializeField] Fase_Menu menu;
    [SerializeField] UI_Menu ui;

    [Apartat("PREFABS")]
    [SerializeField] GameObject foto;
    [SerializeField] GameObject fotoZoom;
    [SerializeField] Utils_InstantiableFromProject fondoClicable_NoFotos;

    [Apartat("ELEMENTS")]
    [SerializeField] Input_EsdevenimentPerBinding esdevenimentPerBinding;
    [SerializeField] XS_ScrollRect scrollRect;

    [Apartat("TRADUCCIONS")]
    [SerializeField] TMP_Text carpetaCaptures;
    //[SerializeField] TMP_Text capcalera;
    //[SerializeField] LocalizedString local_Album;
    //[SerializeField] LocalizedString local_PartidesGuardades;

    #region Intern
    UI_Foto[] fotos = new UI_Foto[0];
    CapturarPantalla.Captura[] captures;
    int indexPartida;
    Lector lector;
    float posicio;
    string[] paths;
    #endregion


    void OnEnable()
    {
        scrollRect.SetContentSize((Vector2.one * 260) * (Vector2.one / (float)guardat.Get("InterficeSize", 0.8f)));
        carpetaCaptures.text = capturarPantalla.RootPath;
        //gridLayoutGroup.cellSize = (Vector2.one * 260) * (Vector2.one / (float)guardat.Get("InterficeSize", 0.8f));
        //content.transform.localScale = Vector3.one / (float)guardat.Get("InterficeSize", 0.8f);
        Actualitzar();
    }

    void Actualitzar()
    {
        BorrarFotos();

        paths = save.Paths;
        captures = new CapturarPantalla.Captura[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            captures[i] = capturarPantalla.GetCapturaGuardada(paths[i]);
        }

        CrearFotos(captures);
    }

    void BorrarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            Destroy(fotos[i].gameObject);
        }
    }
    void CrearFotos(CapturarPantalla.Captura[] captures)
    {
        if (captures.Length == 0)
        {
            fondoClicable_NoFotos.Instantiate();
            return;
        }

        fotos = new UI_Foto[captures.Length];


        for (int i = 0; i < captures.Length; i++)
        {
            if (string.IsNullOrEmpty(captures[i].path))
                continue;

            indexPartida = save.CapturaToIndex(captures[i].path);

            fotos[i] = Instantiate(foto, scrollRect.content).GetComponent<UI_Foto>();
            fotos[i].Setup(
               indexPartida != -1 ? save.Experiencia(indexPartida) : 0,
               captures[i],
               indexPartida,
               ZoomIn,
               PosicionarContent
               ); ;
        }
        SeleccionarLaPrimera();
    }

    public void SeleccionarLaPrimera() 
    {
        if (fotos.Length == 0)
            return;

        fotos[0].Seleccionar();
    } 

    void ZoomIn(UI_Foto foto, CapturarPantalla.Captura captura, int indexPartida)
    {
        fotoZoomed = Instantiate(fotoZoom);
        esdevenimentPerBinding.enabled = false;
        fotoZoomed.GetComponent<UI_FotoZoom>().Setup(foto, captura.texture, captura.path, indexPartida, save.Actual == indexPartida, ZoomOut, Carregar, EliminarCaptura, SeleccionarLaPrimera);
    }
    void ZoomOut()
    {
        esdevenimentPerBinding.enabled = true;
    }

    void Carregar(int partida)
    {
        menu.Carregar(partida);
        ui.Resume();
    }
    void EliminarCaptura(string path, int index)
    {
        capturarPantalla.EliminarCaptura(path);
        save.RemoveCaptura(index, path);
        Actualitzar();
        scrollRect.Iniciar();
    }



    void PosicionarContent(UI_Foto foto)
    {
        if(!lector) lector = scrollRect.content.gameObject.AddComponent<Lector>();
        posicio = -(foto.GetComponent<RectTransform>().anchoredPosition.x - 130) + 600;
        Debug.Log(posicio);
        //content.anchoredPosition = new Vector3(posicio, 0, 0);
        scrollRect.content.SetupAndPlay(lector, new Animacio_RectPosicio(scrollRect.content.anchoredPosition, new Vector2(posicio, 0), Corba.EasyInEasyOut), 1, 0, Transicio.clamp);

    }

    private void OnValidate()
    {
        esdevenimentPerBinding = GetComponent<Input_EsdevenimentPerBinding>();
        guardat = XS_Editor.LoadGuardat<Guardat>();
    }

}
