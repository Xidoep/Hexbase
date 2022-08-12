using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ZonaBloqueigColocar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        Fase_Colocar.Bloquejar();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Fase_Colocar.Desbloquejar();
    }



    void OnMouseEnter()
    {
        //Fase_Colocar.Bloquejar();
    }
    void OnMouseExit()
    {
        //Fase_Colocar.Desbloquejar();
    }

    public void Bloquejar()
    {
        Debug.LogError("trig enter");
        Fase_Colocar.Bloquejar();
    }
    public void Desbloquejar()
    {
        Debug.LogError("trig exit");
        Fase_Colocar.Desbloquejar();
    }
}
