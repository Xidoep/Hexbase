using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XS_Utils;

public class UI_Peces : MonoBehaviour
{
    const string SELECCIONAT_ID = "_Seleccionat";

    const float OFFSET_CAMERA_Y = -2.5f;
    const float OFFSET_CAMERA_Z = 20f;
    const float OFFSET_CAMERA_X = 2f;
    const float OFFSET_CAMERA_Rot = -30f;

    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Estat[] peces;
    //INTERN
    Transform[] childs;
    List<UI_Peca> creades;

    private void OnEnable()
    {
        if (creades != null)
            return;

        creades = new List<UI_Peca>();
        for (int i = 0; i < peces.Length; i++)
        {
            GameObject parent = Instantiate(prefab, this.parent);
            parent.transform.position = Vector3.zero;

            GameObject peça = Instantiate(peces[i].Prefag, Vector3.zero, Quaternion.identity, parent.transform);

            RectTransform rect = parent.GetComponent<RectTransform>();
            rect.anchoredPosition3D = Vector3.zero;

            //peça.transform.SetParent(parent.transform);
            peça.transform.localScale = new Vector3(100, 75, 100);
            peça.transform.localRotation = Quaternion.Euler(-30, 0, 0);


            XS_Button button = parent.GetComponent<XS_Button>();
            UI_Peca uiPeca = peça.GetComponent<UI_Peca>();
            button.onClick.AddListener(uiPeca.Seleccionar);
            button.onEnter.AddListener(uiPeca.Mostrar);
            button.onExit.AddListener(uiPeca.Amagar);
            uiPeca.DeseleccionarAltres = DesseleccionarAltres;

            if(peces[i] == colocar.Seleccionada)
            {
                uiPeca.Mostrar();
                uiPeca.Seleccionar();
            }

            creades.Add(uiPeca);
        }


    }

    void DesseleccionarAltres()
    {
        for (int i = 0; i < creades.Count; i++)
        {
            creades[i].Deseleccionar();
        }
    }
}
