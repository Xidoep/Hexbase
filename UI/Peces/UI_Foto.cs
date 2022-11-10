using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Foto : MonoBehaviour
{
    [SerializeField] GameObject fotoZoom;
    [Space(20)]
    [SerializeField] RectTransform rect;
    [SerializeField] RawImage image;
 
    Texture2D foto;



    public Texture2D Foto { set => foto = value; }
    public RectTransform RectTransform => rect;
    public RawImage Image => image;

    public void Zoom()
    {
        UI_Foto tmp = Instantiate(fotoZoom).GetComponent<UI_Foto>();
        tmp.Foto = foto;
        tmp.RectTransform.sizeDelta = new Vector2(tmp.RectTransform.sizeDelta.y, tmp.RectTransform.sizeDelta.x * (foto.texelSize.x / foto.texelSize.y));
        tmp.Image.texture = foto;
    }
}
