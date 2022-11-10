using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Fotos : MonoBehaviour
{
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] GameObject foto;
    [SerializeField] Transform parent;

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

        Texture2D[] captures = capturarPantalla.CapturesGuardades();
        fotos = new RawImage[captures.Length];

        for (int i = 0; i < captures.Length; i++)
        {
            GameObject tmp = Instantiate(foto, parent);

            fotos[i] = tmp.GetComponentInChildren<RawImage>();
            fotos[i].texture = captures[i];
            RectTransform rectTransform = tmp.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captures[i].texelSize.x / captures[i].texelSize.y));

            tmp.GetComponent<UI_Foto>().Foto = captures[i];
        }
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
