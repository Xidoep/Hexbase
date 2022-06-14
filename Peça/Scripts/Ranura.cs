using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ranura : Hexagon, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool bloquejar = false;

    public override bool EsPeça => false;

    public override void Iniciar() 
    {
        transform.localEulerAngles = new Vector3(0, Random.Range(-5, 5), 0);
        transform.localScale = new Vector3(Random.Range(1.1f, 0.9f), 1, Random.Range(1.1f, 0.9f));
    }
    public override void Actualitzar() { }

    void Crear()
    {
        if (bloquejar)
            return;

        bloquejar = true;

        CrearPeça();
        animacioPerCodi.Play();
        Destroy(gameObject, 1);
    }



    //INTERACCIO
    public void OnPointerDown(PointerEventData eventData) => Crear();
    public void OnPointerUp(PointerEventData eventData) => Crear();

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Highlight*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Back to normal*/ }
}
