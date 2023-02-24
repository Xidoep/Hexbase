using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_FotoZoom : MonoBehaviour
{
    public void Setup(UI_Foto foto, Texture2D textura, string path, int indexPartida, System.Action amagar, System.Action<int> carregar, System.Action<string, int> borrar)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.x * (textura.texelSize.x / textura.texelSize.y) + 140f);
        image.texture = textura;

        this.path = path;
        botoCarregar.SetActive(indexPartida != -1);
        this.indexPartida = indexPartida;
        this.carregar = carregar;
        this.borrar = borrar;
        enAmagar.AddListener(amagar.Invoke);
    }

    [SerializeField] RectTransform rect;
    [SerializeField] RawImage image;
    [SerializeField] GameObject botoCarregar;
    [Space(20)]
    [SerializeField] Utils_InstantiableFromProject popup_Carregar;
    [SerializeField] Utils_InstantiableFromProject popup_Borrar;
    [SerializeField] GameObject popupCarregar;
    [SerializeField] GameObject popupBorrar;
    [Space(20)]
    [SerializeField] UnityEvent enAmagar;

    string path;
    int indexPartida;
    System.Action<int> carregar;
    System.Action<string, int> borrar;

    public void AccioCarregar() => carregar.Invoke(indexPartida);
    public void AccioBorrarCaptura() => borrar.Invoke(path, indexPartida);

    public void Carregar() => popup_Carregar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioCarregar);
    public void Borrar() => popup_Borrar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioBorrarCaptura);
    public void Amagar() => enAmagar.Invoke();

}
