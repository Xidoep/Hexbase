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
        rect.sizeDelta = new Vector2(1400, rect.sizeDelta.y);
        image.texture = textura;

        this.path = path;
        this.indexPartida = indexPartida;
        this.carregar = carregar;
        this.borrar = borrar;

        enAmagar.AddListener(amagar.Invoke);

        botoCarregar.gameObject.SetActive(indexPartida != -1);
        //botoBorrar.gameObject.SetActive(indexPartida == -1);
    }

    [SerializeField] RectTransform rect;
    [SerializeField] RawImage image;
    [SerializeField] XS_Button botoCarregar;
    [SerializeField] XS_Button botoBorrar;

    [Space(20)]
    [SerializeField] Utils_InstantiableFromProject popup_Carregar;
    [SerializeField] Utils_InstantiableFromProject popup_Borrar;
    [SerializeField] Utils_InstantiableFromProject popup_BorrarPartida;

    [Space(20)]
    [SerializeField] UnityEvent enAmagar;

    [Space(20)]
    [SerializeField] string path;
    [SerializeField] int indexPartida;

    System.Action<int> carregar;
    System.Action<string, int> borrar;

    public void AccioCarregar() => carregar.Invoke(indexPartida);
    public void AccioBorrarCaptura() => borrar.Invoke(path, indexPartida);

    public void Carregar() => popup_Carregar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioCarregar, Amagar);
    public void Borrar()
    {
        if (indexPartida == -1)
            BorrarFoto();
        else BorrarPartida();
    }
    public void Amagar() => enAmagar.Invoke();

    void BorrarFoto()
    {
        popup_Borrar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioBorrarCaptura, Amagar);
    }
    void BorrarPartida()
    {
        popup_BorrarPartida.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegat>().Registrar(AccioBorrarCaptura, Amagar);
    }
}
