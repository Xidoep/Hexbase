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

    [SerializeField] GameObject outline;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Estat estat;

    Transform[] childs;

    public void Seleccionar() => colocar.Seleccionar(estat);

    //public void Outline(bool mostrar) => meshRenderer.material.SetFloat(SELECCIONAT_ID, mostrar ? 1 : 0);
    //public void Mostrar() => meshRenderer.material.SetFloat(SELECCIONAT_ID, 1);
    //public void Amagar() => meshRenderer.material.SetFloat(SELECCIONAT_ID, 0);

    public void Mostrar() => outline.SetActive(true);



    private void OnEnable()
    {
        childs = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.layer = 5;
        }
    }
}
