using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Fotos : MonoBehaviour
{
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] SaveHex save;
    [SerializeField] Grups grups;
    [SerializeField] Fase colocar;
    [Linia]
    [SerializeField] GameObject foto;
    [SerializeField] Transform parent;

    RawImage[] fotos = new RawImage[0];
    CapturarPantalla.Captura[] captures;
    void OnEnable()
    {
        ActualitzarFotos();
        capturarPantalla.OnCapturatRegistrar(save.AddCaptura);
    }

    private void OnDisable()
    {
        capturarPantalla.OnCapturatDesregistrar(save.AddCaptura);
    }

    [ContextMenu("Captures Guardades")]
    void ActualitzarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            Destroy(fotos[i]);
        }

        captures = capturarPantalla.CapturesGuardades();
        //Texture2D[] captures = capturarPantalla.CapturesGuardades();
        fotos = new RawImage[captures.Length];

        for (int i = 0; i < captures.Length; i++)
        {
            GameObject tmp = Instantiate(foto, parent);

            fotos[i] = tmp.GetComponentInChildren<RawImage>();
            fotos[i].texture = captures[i].texture;
            RectTransform rectTransform = tmp.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captures[i].texture.texelSize.x / captures[i].texture.texelSize.y));

            int indexPartida = save.ExisteixCaptura(captures[i].path);
            tmp.GetComponent<UI_Foto>().Setup(
                captures[i].texture, 
                captures[i].path,
                indexPartida,
                Load,
                EliminarCaptura
                );
        }
    }


    public void Load(int index)
    {
        save.Load(index, grups, colocar);
    }
    public void EliminarCaptura(string path, int index)
    {
        capturarPantalla.EliminarCaptura(path);
        save.RemoveCaptura(index, path);
    }

    public void HabilitarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            fotos[i].raycastTarget = true;
        }
    }
    public void DeshabilitarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            fotos[i].raycastTarget = false;
        }
    }
}
