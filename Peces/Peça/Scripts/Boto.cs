using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Boto : Hexagon, IPointerClickHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);
    
    }

    [SerializeField] UnityEvent onClick;


    public override bool EsPeça => false;


    public void OnPointerClick(PointerEventData eventData) => Click();

    public void Click() => onClick?.Invoke();



}
