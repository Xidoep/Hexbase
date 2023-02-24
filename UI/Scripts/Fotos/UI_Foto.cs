using UnityEngine;
using UnityEngine.UI;

public class UI_Foto : MonoBehaviour
{
    public void Setup(CapturarPantalla.Captura captura, int indexPartida)
    {
        this.captura = captura;
        this.indexPartida = indexPartida;

        rawImage.texture = captura.texture;
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captura.texture.texelSize.x / captura.texture.texelSize.y));
    }
    public void Setup2(UI_Fotos album, CapturarPantalla.Captura captura, int indexPartida, bool seleccionar)
    {
        this.album = album;
        this.captura = captura;
        this.indexPartida = indexPartida;

        rawImage.texture = captura.texture;
        rectTransform.localScale = new Vector3(1, captura.texture.texelSize.x / captura.texture.texelSize.y, 1);
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captura.texture.texelSize.x / captura.texture.texelSize.y));
        if (seleccionar) bora.GetComponent<XS_Button>().Select();
    }
    [SerializeField] UI_Fotos album;

    [SerializeField] Image bora;
    [SerializeField] RawImage rawImage;
    [SerializeField] RectTransform rectTransform;

    int indexPartida;
    CapturarPantalla.Captura captura;

    public void Habilitar() => bora.raycastTarget = true;
    public void Deshabilitar() => bora.raycastTarget = true;


    public void Zoom() => album.ZoomIn(this, captura, indexPartida);

}
