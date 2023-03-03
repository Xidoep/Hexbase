using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Foto : MonoBehaviour
{
    public void Setup(UI_Album album, int experiencia, CapturarPantalla.Captura captura, int indexPartida, bool seleccionar)
    {
        this.album = album;
        this.captura = captura;
        this.indexPartida = indexPartida;

        rectTransform.localScale = new Vector3(1, captura.texture.texelSize.x / captura.texture.texelSize.y + 0.1f, 1);
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captura.texture.texelSize.x / captura.texture.texelSize.y));
        
        if (seleccionar) boto.Select();
        //boto.OnEnter += EnApuntar;
        
        impresio.texture = captura.texture;

        this.experiencia.text = indexPartida != -1 ? experiencia.ToString() : "";

    }
    UI_Album album;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] XS_Button boto;
    [SerializeField] RawImage impresio;
    [SerializeField] TMP_Text experiencia;

    [Space(20)]
    [SerializeField] int indexPartida;
    [SerializeField] CapturarPantalla.Captura captura;

    //public void Habilitar() => bora.raycastTarget = true;
    //public void Deshabilitar() => bora.raycastTarget = true;


    public void Zoom() => album.ZoomIn(this, captura, indexPartida);

    public void EnApuntar() => album.PosicionarContent(this);

}
