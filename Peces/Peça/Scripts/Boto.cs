using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using XS_Utils;

public class Boto : Hexagon, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
 

    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);
    
    }
    [SerializeField] Fase menu;
    [Linia]
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;
    [SerializeField] UnityEvent onClick;
    //[Linia]
    //[SerializeField] GameObject outline;

    public override bool EsPeça => false;

    void OnEnable()
    {
        menu.OnFinish += AmagarEnAcabarFase;
    }
    void OnDisable()
    {
        menu.OnFinish -= AmagarEnAcabarFase;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //outline.SetActive(true);
        //outline.material.SetFloat(SELECCIONAT_ID, 1);
        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //outline.material.SetFloat(SELECCIONAT_ID, 0);
        onExit?.Invoke();
    }

    void OnDestroy()
    {
        Buidar();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }


    void AmagarEnAcabarFase()
    {
        //Canviar aixo per una animacio
        Destroy(this.gameObject);
    }

    void OnValidate()
    {
        menu = XS_Editor.LoadAssetAtPath<Fase>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Menu.asset");
    }
}
