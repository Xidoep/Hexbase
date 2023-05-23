using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using XS_Utils;

public class Boto : Hexagon, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, EstatColocable estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);

        boto.onClick.AddListener(onClick.Invoke);
        boto.OnEnter += OnPointerEnter;
        boto.OnExit += OnPointerExit;
    }

    readonly WaitForSeconds waitForSeconds = new(0.5f);

    [SerializeField] Collider collider;
    [SerializeField] UnityEvent onClick;
    [SerializeField] XS_Button boto;
    [Apartat("INFORMACIO")]
    [SerializeField] LocalizedString texte;
    [SerializeField] Informacio informacio;

    [SerializeField] bool interactable = true;
    public override bool EsPeça => false;
    public LocalizedString Texte => texte;
    public bool Interactable { get => interactable; set => interactable = value; }

    void OnEnable()
    {
        StartCoroutine(ActivarCollider());
    }

    IEnumerator ActivarCollider()
    {
        yield return waitForSeconds;
        collider.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData) => Click();

    public void Click() 
    {
        if (!interactable)
            return;

        onClick?.Invoke();
    } 




    public override void OnPointerEnter() 
    {
        if (!interactable)
            return;

        informacio.Mostrar(this);
    }
    public override void OnPointerExit() 
    {
        if (!interactable)
            return;

        informacio.Amagar(this);
    }

    public void Navegacio(bool activar)
    {
        Navigation navigation = new Navigation();
        navigation.mode = activar ? Navigation.Mode.Automatic : Navigation.Mode.None;

        boto.navigation = navigation;
    }
    public void Seleccionar() => boto.Select();











    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();








    void OnValidate()
    {
        //collider = GetComponent<Collider>();

        if(informacio == null) informacio = XS_Editor.LoadAssetAtPath<Informacio>("Assets/XidoStudio/Hexbase/Peces/Informacio/Texte.asset");
    }
}
