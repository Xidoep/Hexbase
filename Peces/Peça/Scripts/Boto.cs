using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using XS_Utils;

public class Boto : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);
    }

    [SerializeField] UnityEvent onClick;

    public override bool EsPeça => false;


    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onClick.Invoke();
    }

    private void OnDestroy()
    {
        Buidar();
    }
}
