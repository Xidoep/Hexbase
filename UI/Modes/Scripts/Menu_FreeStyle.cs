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
    [SerializeField] Referencies referencies;

    List<UI_Peca> creades;

    private void OnEnable()
    {
        if (creades != null)
            return;

        creades = new List<UI_Peca>();
        for (int i = 0; i < referencies.Colocables.Length; i++)
        {
            GameObject parent = Instantiate(prefab, this.parent);
            parent.transform.position = Vector3.zero;

            UI_Peca uiPeca = (UI_Peca)(referencies.Colocables[i].Prefab.Crear().SetTransform(Vector3.zero, Quaternion.Euler(-30, 0, 0), new Vector3(100, 75, 100), parent.transform));
            //UI_Peca uiPeca = referencies.Estats[i].Prefab.Crear();
            //uiPeca.SetTransform(Vector3.zero, Quaternion.Euler(-30, 0, 0), new Vector3(100, 75, 100), parent.transform);

            ((RectTransform)parent.transform).anchoredPosition3D = Vector3.zero;


            //GameObject peça = Instantiate(referencies.Estats[i].Prefag, Vector3.zero, Quaternion.identity, parent.transform);

            //RectTransform rect = parent.GetComponent<RectTransform>();
            //rect.anchoredPosition3D = Vector3.zero;

            //peça.transform.localScale = new Vector3(100, 75, 100);
           // peça.transform.localRotation = Quaternion.Euler(-30, 0, 0);

            XS_Button button = parent.GetComponent<XS_Button>();
            //UI_Peca uiPeca = peça.GetComponent<UI_Peca>();

            button.onClick.AddListener(uiPeca.Seleccionar);
            button.onClick.AddListener(MostrarSeleccionada);
            button.OnEnter += uiPeca.Resaltar;
            button.OnExit += uiPeca.Desresaltar;

            if(referencies.Colocables[i] == colocar.Seleccionada)
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
            
        }
    }

}
