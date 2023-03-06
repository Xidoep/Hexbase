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

    [Apartat("PREFABS")]
    [SerializeField] GameObject foto;
    [SerializeField] GameObject fotoZoom;
    [SerializeField] Utils_InstantiableFromProject fondoClicable_NoFotos;

    [Apartat("ELEMENTS")]
    //[SerializeField] Transform parent;
    [SerializeField] Input_EsdevenimentPerBinding esdevenimentPerBinding;
    //[SerializeField] RectTransform content;
    //[SerializeField] ProvesContent contentDinamic;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] XS_ScrollRect scrollRect;

    [Apartat("TRADUCCIONS")]
    [SerializeField] TMP_Text capcalera;
    [SerializeField] LocalizedString local_Album;
    [SerializeField] LocalizedString local_PartidesGuardades;

    #region Intern
    UI_Foto[] fotos = new UI_Foto[0];
    CapturarPantalla.Captura[] captures;
    int indexPartida;
    Lector lector;
    float posicio;
    #endregion


    void OnEnable()
    {
        scrollRect.SetContentSize((Vector2.one * 260) * (Vector2.one / (float)guardat.Get("InterficeSize", 0.8f)));
        //gridLayoutGroup.cellSize = (Vector2.one * 260) * (Vector2.one / (float)guardat.Get("InterficeSize", 0.8f));
        //content.transform.localScale = Vector3.one / (float)guardat.Get("InterficeSize", 0.8f);
        Actualitzar();
    }

    void Actualitzar()
    {
        if (!save.NomesGuardats)
        {
            local_Album.WriteOn(capcalera);
            ActualitzarFotos();
        }
        else
        {
            local_PartidesGuardades.WriteOn(capcalera);
            ActualitzarPartides();
        }
    }

    void ActualitzarFotos()
    {
        BorrarFotos();

        CrearFotos(capturarPantalla.GetCapturesGuardades());

    }
    void ActualitzarPartides()
    {
        BorrarFotos();

        captures = new CapturarPantalla.Captura[save.FilesLength];

        for (int i = 0; i < save.FilesLength; i++)
        {
            string path = save.GetCapturaMesRecent(i);
            if (string.IsNullOrEmpty(path))
                continue;

            captures[i] = capturarPantalla.GetCapturaGuardada(save.GetCapturaMesRecent(i));
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
            indexPartida = save.CapturaToIndex(captures[i].path);

            if (save.NomesGuardats && indexPartida == -1)
                continue;

            fotos[i] = Instantiate(foto, scrollRect.content).GetComponent<UI_Foto>();
            //contentDinamic.Add(fotos[i].gameObject);
            fotos[i].Setup(
               this,
               indexPartida != -1 ? save.Experiencia(indexPartida) : 0,
               captures[i],
               indexPartida,
               i == 0
               ); ;
        }
        //contentDinamic.Provar();
        //scrollRect.Iniciar();
    }


    //void ActualitzarFotos(string path) => ActualitzarFotos();



    public void ZoomIn(UI_Foto foto, CapturarPantalla.Captura captura, int indexPartida)
    {
        fotoZoomed = Instantiate(fotoZoom);
        fotoZoomed.GetComponent<UI_FotoZoom>().Setup(foto, captura.texture, captura.path, indexPartida, ZoomOut, Load, EliminarCaptura);
        //gridLayoutGroup.padding = new RectOffset(0, 0, 40, 0);
        //gridLayoutGroup.cellSize = new Vector2(1400, 1400);
        esdevenimentPerBinding.enabled = false;
        //esdevenimentPerBinding_ZoomOut.enabled = true;
    }
    public void ZoomOut()
    {
        //gridLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
        //gridLayoutGroup.cellSize = new Vector2(260, 260);
        esdevenimentPerBinding.enabled = true;
        //esdevenimentPerBinding_ZoomOut.enabled = false;

    }

    public void Load(int index)
    {
        Debugar.Log("Load");
        Grid.Instance.Resetejar();
        StartCoroutine(LoadFile(index));
    }
    IEnumerator LoadFile(int index)
    {
        yield return new WaitForSeconds(1);
        save.Load(index, grups, faseEnCarregar);
        modes.Set((Mode)save.Mode);
    }
    public void EliminarCaptura(string path, int index)
    {
        capturarPantalla.EliminarCaptura(path);
        save.RemoveCaptura(index, path);
        Actualitzar();
    }

    /*public void HabilitarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            fotos[i].Habilitar();
        }
    }
    public void DeshabilitarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            fotos[i].Deshabilitar();
        }
    }*/


    public void PosicionarContent(UI_Foto foto)
    {
        if(!lector) lector = scrollRect.content.gameObject.AddComponent<Lector>();
        posicio = -(foto.GetComponent<RectTransform>().anchoredPosition.x - 130) + 600;
        Debug.Log(posicio);
        //content.anchoredPosition = new Vector3(posicio, 0, 0);
        scrollRect.content.SetupAndPlay(lector, new Animacio_RectPosicio(scrollRect.content.anchoredPosition, new Vector3(posicio, 0, 0), Corba.EasyInEasyOut), 1, Transicio.clamp);

        //new Animacio_RectPosicio(content.anchoredPosition, new Vector3(-(750 + foto.transform.localPosition.x),0,0)).Play(content, 1, Transicio.clamp);

    }

    private void OnValidate()
    {
        esdevenimentPerBinding = GetComponent<Input_EsdevenimentPerBinding>();
        guardat = XS_Editor.LoadGuardat<Guardat>();
    }

}
