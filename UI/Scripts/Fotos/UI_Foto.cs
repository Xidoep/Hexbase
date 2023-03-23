using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Foto : MonoBehaviour
{
    public void Setup(int experiencia, CapturarPantalla.Captura captura, int indexPartida, bool seleccionar, System.Action<UI_Foto, CapturarPantalla.Captura, int> accioZoom, System.Action<UI_Foto> accioEnApuntar)
    {
        rectTransform.localScale = new Vector3(1, captura.texture.texelSize.x / captura.texture.texelSize.y + 0.1f, 1);
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.x * (captura.texture.texelSize.x / captura.texture.texelSize.y));
        if (seleccionar) boto.Select();
        impresio.texture = captura.texture;
        this.experiencia.text = indexPartida != -1 ? experiencia.ToString() : "";

        this.indexPartida = indexPartida;
        this.captura = captura;

        this.accioZoom = accioZoom;
        this.accioEnApuntar = accioEnApuntar;
    }

    [SerializeField] RectTransform rectTransform;
    [SerializeField] XS_Button boto;
    [SerializeField] RawImage impresio;
    [SerializeField] TMP_Text experiencia;

    [Space(20)]
    [SerializeField] int indexPartida;
    [SerializeField] CapturarPantalla.Captura captura;

    System.Action<UI_Foto, CapturarPantalla.Captura, int> accioZoom;
    System.Action<UI_Foto> accioEnApuntar;


    public void Zoom() => accioZoom.Invoke(this, captura, indexPartida);
    public void EnApuntar() => accioEnApuntar.Invoke(this);

}
