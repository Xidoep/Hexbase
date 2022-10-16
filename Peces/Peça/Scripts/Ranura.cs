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
    [Linia]
    [SerializeField] MeshRenderer outline;
    System.Action accioCrear;

    //Prevé multiples clics.
    bool autobloquejar = false;
    public override bool EsPeça => false;

    private void OnEnable()
    {
        accioCrear = Crear;
        
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
        //animacioPerCodi.Play();
        animacio.Play(gameObject);
        Destroy(gameObject, 1);
    }



    //INTERACCIO
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) => accioCrear?.Invoke();

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        outline.material.SetFloat(SELECCIONAT_ID, 1);
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        outline.material.SetFloat(SELECCIONAT_ID, 0);
    }
}
