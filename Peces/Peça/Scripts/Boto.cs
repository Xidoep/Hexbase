using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Boto : Hexagon, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);
    
    }

    [SerializeField] UnityEvent onClick;
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;

    public override bool EsPeça => false;


    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }



}
