using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using XS_Utils;

public class UI_Fotos : MonoBehaviour
{
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] SaveHex save;
    [SerializeField] Grups grups;
    [SerializeField] Fase faseEnCarregar;
    [SerializeField] Modes modes;
    [Linia]
    [SerializeField] GameObject foto;
    [SerializeField] GameObject fotoZoom;
    [SerializeField] Transform parent;
    [SerializeField] Input_EsdevenimentPerBinding esdevenimentPerBinding;

    public static GameObject fotoZoomed;

    UI_Foto[] fotos = new UI_Foto[0];
    CapturarPantalla.Captura[] captures;

    void OnEnable()
    {
        ActualitzarFotos();
        capturarPantalla.OnCapturatRegistrar(save.AddCaptura);
        capturarPantalla.OnCapturatRegistrar(ActualitzarFotos);
    }

    private void OnDisable()
    {
        capturarPantalla.OnCapturatDesregistrar(save.AddCaptura);
        capturarPantalla.OnCapturatDesregistrar(ActualitzarFotos);
    }


    [ContextMenu("Captures Guardades")]
    void ActualitzarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            Destroy(fotos[i].gameObject);
        }

        captures = capturarPantalla.CapturesGuardades();
        fotos = new UI_Foto[captures.Length];

        for (int i = 0; i < captures.Length; i++)
        {
            fotos[i] = Instantiate(foto, parent).GetComponent<UI_Foto>();
            int indexPartida = save.ExisteixCaptura(captures[i].path);

            fotos[i].Setup2(
               this,
               captures[i],
               indexPartida,
               i == 0
               );
        }
    }


    void ActualitzarFotos(string path) => ActualitzarFotos();



    public void ZoomIn(UI_Foto foto, CapturarPantalla.Captura captura, int indexPartida)
    {
        fotoZoomed = Instantiate(fotoZoom);
        fotoZoomed.GetComponent<UI_FotoZoom>().Setup(foto, captura.texture, captura.path, indexPartida, ZoomOut, Load, EliminarCaptura);
        esdevenimentPerBinding.enabled = false;
    }
    public void ZoomOut()
    {
        esdevenimentPerBinding.enabled = true;
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
        ActualitzarFotos();
    }

    public void HabilitarFotos()
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
    }

    private void OnValidate()
    {
        esdevenimentPerBinding = GetComponent<Input_EsdevenimentPerBinding>();
    }
}
