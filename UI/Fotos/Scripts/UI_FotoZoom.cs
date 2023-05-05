using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_FotoZoom : MonoBehaviour
{
    public void Setup(UI_Foto foto, Texture2D textura, string path, int indexPartida, bool partidaJaCarregada, System.Action amagar, System.Action<int> carregar, System.Action<string, int> borrar, System.Action seleccionarLaPrimera)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.x * (textura.texelSize.x / textura.texelSize.y) + 140f);
        rect.sizeDelta = new Vector2(1400, rect.sizeDelta.y);
        image.texture = textura;

        enAmagar.AddListener(amagar.Invoke);

        this.path = path;
        this.indexPartida = indexPartida;
        this.partidaJaCarregada = partidaJaCarregada;

        this.carregar = carregar;
        this.borrar = borrar;
        this.seleccionarLaPrimera = seleccionarLaPrimera;
    }

    [SerializeField] RectTransform rect;
    [SerializeField] RawImage image;
    [SerializeField] XS_Button botoCarregar;
    [SerializeField] XS_Button botoBorrar;

    [Space(20)]
    [SerializeField] Utils_InstantiableFromProject popup_Carregar;
    [SerializeField] Utils_InstantiableFromProject popup_Borrar;
    [SerializeField] Utils_InstantiableFromProject popup_PartidaJaCarregada;

    [Space(20)]
    [SerializeField] UnityEvent enAmagar;

    [Space(20)]
    [SerializeField] string path;
    [SerializeField] int indexPartida;
    [SerializeField] bool partidaJaCarregada;

    System.Action<int> carregar;
    System.Action<string, int> borrar;
    System.Action seleccionarLaPrimera;





    public void Carregar()
    {
        if (!partidaJaCarregada)
            popup_Carregar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(AccioCarregar);
        else popup_PartidaJaCarregada.Instantiate();
    }
    public void Borrar()
    {
        popup_Borrar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(AccioBorrarCaptura);
    }





    void AccioCarregar(bool carregar)
    {
        if (carregar)
        {
            this.carregar.Invoke(indexPartida);
            enAmagar.Invoke();
        }
        else
        {
            botoCarregar.Select();
        }
    }
    void AccioBorrarCaptura(bool borrar)
    {
        if (borrar)
        {
            this.borrar.Invoke(path, indexPartida);
            enAmagar.Invoke();
            seleccionarLaPrimera.Invoke();
        }
        else
        {
            botoBorrar.Select();
        }
    }

    public void Amagar() => enAmagar.Invoke();

}
