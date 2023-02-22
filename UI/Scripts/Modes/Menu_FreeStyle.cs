using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XS_Utils;

public class Menu_FreeStyle : MonoBehaviour
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

            peça.transform.localScale = new Vector3(100, 75, 100);
            peça.transform.localRotation = Quaternion.Euler(-30, 0, 0);

            XS_Button button = parent.GetComponent<XS_Button>();
            UI_Peca uiPeca = peça.GetComponent<UI_Peca>();

            button.onClick.AddListener(uiPeca.Seleccionar);
            button.onClick.AddListener(MostrarSeleccionada);
            button.OnEnter += uiPeca.Resaltar;
            button.OnExit += uiPeca.Desresaltar;

            if(peces[i] == colocar.Seleccionada)
            {
                uiPeca.Resaltar();
                uiPeca.Seleccionar();
            }

            creades.Add(uiPeca);
        }


    }


    void MostrarSeleccionada()
    {
        for (int i = 0; i < creades.Count; i++)
        {
            if (creades[i].resaltat && !creades[i].Seleccionada)
            {
                creades[i].Desresaltar();
            }
            /*if (creades[i].Seleccionada)
            {
                creades[i].Resaltar();
                continue;
            }*/

            
        }
    }

    void DesseleccionarAltres()
    {
        for (int i = 0; i < creades.Count; i++)
        {
            if (creades[i].Seleccionada)
            {
                creades[i].Resaltar();
                continue;
            }

            creades[i].Desresaltar();
        }
    }

}
