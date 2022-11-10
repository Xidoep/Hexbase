using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Fotos : MonoBehaviour
{
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] GameObject foto;
    [SerializeField] Transform parent;
    [SerializeField] SaveHex save;

    RawImage[] fotos = new RawImage[0];

    void OnEnable()
    {
        ActualitzarFotos();
    }

    [ContextMenu("Captures Guardades")]
    void ActualitzarFotos()
    {
        for (int i = 0; i < fotos.Length; i++)
        {
            Destroy(fotos[i]);
        }

        CapturarPantalla.Captura[] captures = capturarPantalla.CapturesGuardades();
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
                save.Load,
                EliminarCaptura
                );
        }
    }

    public void EliminarCaptura(string path, int index)
    {
        //Eliminar de l'ordinador i dels arxius de guardat.
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
