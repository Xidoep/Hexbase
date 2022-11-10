using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Foto : MonoBehaviour
{
    public void Setup(Texture2D textura, string path, int indexPartida, Action<int> carregar, Action<string, int> eliminar)
    {
        this.textura = textura;
        this.path = path;
        this.carregable = indexPartida != -1;
        this.carregar = carregar;
        this.eliminar = eliminar;
    }

    [SerializeField] GameObject fotoZoom;


    Texture2D textura;
    string path;
    bool carregable;
    System.Action<int> carregar;
    System.Action<string, int> eliminar;


    public void Zoom()
    {
        UI_FotoZoom tmp = Instantiate(fotoZoom).GetComponent<UI_FotoZoom>();
        tmp.RectTransform.sizeDelta = new Vector2(tmp.RectTransform.sizeDelta.y, tmp.RectTransform.sizeDelta.x * (textura.texelSize.x / textura.texelSize.y) + 140f);
        tmp.Image.texture = textura;

        //******************************************************************************************
        //Mostrar o amagar el boto de carrear
        //Passar funcio carregar
        //Passar funcio eliminar
        //******************************************************************************************
    }

}
