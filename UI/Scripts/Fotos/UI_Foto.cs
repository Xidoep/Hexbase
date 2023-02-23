using UnityEngine;
using UnityEngine.UI;

public class UI_Foto : MonoBehaviour
{
    public void Setup(CapturarPantalla.Captura captura, int indexPartida, System.Action<int> carregar, System.Action<string, int> borrar)
    {
        //this.textura = captura.texture;
        //this.path = captura.path;
        this.captura = captura;
        this.indexPartida = indexPartida;
        this.carregar = carregar;
        this.borrar = borrar;

        rawImage.texture = captura.texture;
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captura.texture.texelSize.x / captura.texture.texelSize.y));
    }
    public void Setup2(CapturarPantalla.Captura captura, int indexPartida, System.Action<int> carregar, System.Action<string, int> borrar)
    {
        //this.textura = captura.texture;
        //this.path = captura.path;
        this.captura = captura;
        this.indexPartida = indexPartida;
        this.carregar = carregar;
        this.borrar = borrar;

        rawImage.texture = captura.texture;
        rectTransform.localScale = new Vector3(1, captura.texture.texelSize.x / captura.texture.texelSize.y, 1);
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captura.texture.texelSize.x / captura.texture.texelSize.y));
    }

    [SerializeField] GameObject fotoZoom;
    [SerializeField] Image bora;
    [SerializeField] RawImage rawImage;
    [SerializeField] RectTransform rectTransform;

    //Texture2D textura;
    //string path;

    int indexPartida;
    CapturarPantalla.Captura captura;

    System.Action<int> carregar;
    System.Action<string, int> borrar;

    public void Habilitar() => bora.raycastTarget = true;
    public void Deshabilitar() => bora.raycastTarget = true;


    public void Zoom()
    {
        Instantiate(fotoZoom).GetComponent<UI_FotoZoom>().Setup(captura.texture, captura.path, indexPartida,carregar,borrar);
        

        //******************************************************************************************
        //Mostrar o amagar el boto de carrear
        //Passar funcio carregar
        //Passar funcio eliminar
        //******************************************************************************************
    }

}
