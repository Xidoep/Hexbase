using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XS_Utils;

public class UI_Seleccio : MonoBehaviour, IPointerDownHandler
{
    public void Setup(Estat estat, Fase_Colocar colocar)
    {
        this.estat = estat;
        seleccionar = colocar.Seleccionar;
    }

    Estat estat;
    System.Action<Estat> seleccionar;

    public void OnPointerDown(PointerEventData eventData)
    {
        seleccionar.Invoke(estat);
    }

}
