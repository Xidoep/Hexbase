using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Peca : MonoBehaviour
{
    public void Setup(Estat estat)
    {
        this.estat = estat;
    }

    [SerializeField] UI_Outline outline;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Estat estat;
    Transform[] childs;


    public bool Seleccionada => estat == colocar.Seleccionada;


    public void Seleccionar() 
    {
        colocar.Seleccionar(estat);
    }

    public void Mostrar() 
    {
        outline.gameObject.SetActive(true);
    } 
    public void Amagar()
    {
        if (Seleccionada)
            return;

        outline.Amagar();
    }
    public void MostrarOAmagar(Estat seleccionat)
    {
        if(estat == seleccionat)
            outline.gameObject.SetActive(true);
        else outline.Amagar();
    }

    private void OnEnable()
    {
        childs = transform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].SetCapa_UIPeces();
        }
    }
}
