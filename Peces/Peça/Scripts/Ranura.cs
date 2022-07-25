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

    //Prevé multiples clics.
    bool autobloquejar = false;
    public override bool EsPeça => false;

    private void OnEnable()
    {
        transform.localEulerAngles = new Vector3(0, Random.Range(-5, 5), 0);
        transform.localScale = new Vector3(Random.Range(1.1f, 0.9f), 1, Random.Range(1.1f, 0.9f));
    }



    void Crear()
    {
        if (!Fase_Colocar.PermesColoarPeça)
            return;

        if (autobloquejar)
            return;

        autobloquejar = true;

        CrearPeça(); 
    }

    public void CrearPeça() 
    {
        colocar.CrearPeça(Coordenades);
        //crearPeça.Invoke(Coordenades);
        animacioPerCodi.Play();
        Destroy(gameObject, 1);
    } 



    //INTERACCIO
    public void OnPointerDown(PointerEventData eventData) {/*Nuse!*/ }
    public void OnPointerUp(PointerEventData eventData) => Crear();

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Highlight*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Back to normal*/ }
}
