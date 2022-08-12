using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Ranura : Hexagon, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    //VARIABLES
    [Linia]
    [SerializeField] Fase_Colocar colocar;

    System.Action accioCrear;

    //Prev� multiples clics.
    bool autobloquejar = false;
    public override bool EsPe�a => false;

    private void OnEnable()
    {
        transform.localEulerAngles = new Vector3(0, Random.Range(-5, 5), 0);
        transform.localScale = new Vector3(Random.Range(1.1f, 0.9f), 1, Random.Range(1.1f, 0.9f));
    }



    void Crear()
    {
        if (Fase_Colocar.Bloquejat)
        {
            Debug.Log("BLOQUEJAT!!!");
            return;
        }

        if (!Fase_Colocar.PermesColoarPe�a)
            return;

        if (autobloquejar)
            return;

        autobloquejar = true;

        CrearPe�a(); 
    }

    public void CrearPe�a() 
    {
        colocar.CrearPe�a(Coordenades);
        //crearPe�a.Invoke(Coordenades);
        animacioPerCodi.Play();
        Destroy(gameObject, 1);
    }



    //INTERACCIO
    public void OnPointerDown(PointerEventData eventData) => accioCrear = Crear;
    public void OnPointerUp(PointerEventData eventData) => accioCrear?.Invoke();

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Highlight*/ }
    public void OnPointerExit(PointerEventData eventData) => accioCrear = null;
}
