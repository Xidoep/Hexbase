using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Boto : Hexagon, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);

        mostrarInformacio += InformacioMostrar;
        amagarInformacio += InformacioAmagar;

        boto.onClick.AddListener(onClick.Invoke);
        boto.OnEnter += OnPointerEnter;
        boto.OnExit += OnPointerExit;
    }

    readonly WaitForSeconds waitForSeconds = new(0.5f);

    [SerializeField] Collider collider;
    [SerializeField] UnityEvent onClick;
    [SerializeField] XS_Button boto;
    [Apartat("INFORMACIO")]
    [SerializeField] Informacio[] informacions;
    Informacio.Unitat informacioMostrada;

    public Informacio.Unitat InformacioMostrada { get => informacioMostrada; set => informacioMostrada = value; }

    public override bool EsPeça => false;

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

    public void Click() => onClick?.Invoke();


    public void Navegacio(bool activar)
    {
        Navigation navigation = new Navigation();
        navigation.mode = activar ? Navigation.Mode.Automatic : Navigation.Mode.None;

        boto.navigation = navigation;
    }
    public void Seleccionar() => boto.Select();

    public void InformacioMostrar(Hexagon hexagon, bool proveides)
    {
        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Mostrar(hexagon, proveides);
        }
    }
    public void InformacioAmagar(Hexagon hexagon)
    {
        if (informacions.Length == 0)
        {
            return;
        }

        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Amagar(hexagon);
        }
    }








    public override void OnPointerEnter() => mostrarInformacio?.Invoke(this, true);
    public override void OnPointerExit() => amagarInformacio?.Invoke(this);











    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();








    void OnValidate()
    {
        collider = GetComponent<Collider>();
    }
}
