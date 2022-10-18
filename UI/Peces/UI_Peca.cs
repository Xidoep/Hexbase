using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Peca : MonoBehaviour
{
    public void Setup(Estat estat)
    {
        this.estat = estat;
    }

    const string SELECCIONAT_ID = "_Seleccionat";

    [SerializeField] UI_Outline outline;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Estat estat;
    [SerializeField] bool seleccionada;
    Transform[] childs;

    System.Action deseleccionarAltres;

    public System.Action DeseleccionarAltres { set => deseleccionarAltres = value; }

    public void Seleccionar() 
    {
        if (seleccionada)
            return;

        colocar.Seleccionar(estat);
        deseleccionarAltres.Invoke();
        seleccionada = true;
    }
    public void Deseleccionar()
    {
        if (!seleccionada)
            return;

        seleccionada = false;
        Amagar();
    }
    //public void Outline(bool mostrar) => meshRenderer.material.SetFloat(SELECCIONAT_ID, mostrar ? 1 : 0);
    //public void Mostrar() => meshRenderer.material.SetFloat(SELECCIONAT_ID, 1);
    //public void Amagar() => meshRenderer.material.SetFloat(SELECCIONAT_ID, 0);

    public void Mostrar() 
    {
        if (seleccionada)
            return;

        outline.gameObject.SetActive(true);
    } 
    public void Amagar()
    {
        if (seleccionada)
            return;

        outline.Amagar();
    }


    private void OnEnable()
    {
        childs = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.layer = 5;
        }
    }
}
