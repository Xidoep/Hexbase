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
    [SerializeField] Fase colocar;
    [Linia]
    [SerializeField] GameObject foto;
    [SerializeField] Transform parent;

    //RawImage[] fotos = new RawImage[0];
    UI_Foto[] fotos = new UI_Foto[0];
    CapturarPantalla.Captura[] captures;
    Grid grid;
    void OnEnable()
    {
        grid = FindObjectOfType<Grid>();
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
        //Texture2D[] captures = capturarPantalla.CapturesGuardades();
        //fotos = new RawImage[captures.Length];
        fotos = new UI_Foto[captures.Length];

        for (int i = 0; i < captures.Length; i++)
        {
            /*GameObject tmp = Instantiate(foto, parent);

            fotos[i] = tmp.GetComponentInChildren<RawImage>();
            fotos[i].texture = captures[i].texture;

            RectTransform rectTransform = tmp.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captures[i].texture.texelSize.x / captures[i].texture.texelSize.y));

            tmp.GetComponent<UI_Foto>().Setup(
                captures[i],
                indexPartida,
                Load,
                EliminarCaptura
                );
            */

            fotos[i] = Instantiate(foto, parent).GetComponent<UI_Foto>();
            int indexPartida = save.ExisteixCaptura(captures[i].path);

            fotos[i].Setup(
               captures[i],
               indexPartida,
               Load,
               EliminarCaptura
               );
        }
    }
    public void ActualitzarFotos(string path) => ActualitzarFotos();

    public void Load(int index)
    {
        Debugar.Log("Load");
        grid.Resetejar();
        save.Load(index, grups, colocar);
        //StartCoroutine(LoadFile(index));
    }
    IEnumerator LoadFile(int index)
    {
        yield return new WaitForSeconds(2);
        save.Load(index, grups, colocar);
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
}
