using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FotoZoom : MonoBehaviour
{
    public void Setup(Texture2D textura, string path, int indexPartida, System.Action<int> carregar, System.Action<string, int> borrar)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.x * (textura.texelSize.x / textura.texelSize.y) + 140f);
        image.texture = textura;

        this.path = path;
        botoCarregar.SetActive(indexPartida != -1);
        this.indexPartida = indexPartida;
        this.carregar = carregar;
        this.borrar = borrar;
    }

    [SerializeField] RectTransform rect;
    [SerializeField] RawImage image;
    [SerializeField] GameObject botoCarregar;

    [SerializeField] GameObject popupCarregar;
    [SerializeField] GameObject popupBorrar;

    string path;
    int indexPartida;
    System.Action<int> carregar;
    System.Action<string, int> borrar;

    public void AccioCarregar() => carregar.Invoke(indexPartida);
    public void AccioBorrarCaptura() => borrar.Invoke(path, indexPartida);

    public void Carregar()
    {
        Instantiate(popupCarregar).GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioCarregar);
    }
    public void Borrar()
    {
        Instantiate(popupBorrar).GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioBorrarCaptura);
    }
}
