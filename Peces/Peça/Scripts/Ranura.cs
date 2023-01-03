using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XS_Utils;

public class Ranura : Hexagon, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    //VARIABLES
    [Linia]
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;
    [Linia]
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Proximitat proximitat;
    
    [Apartat("OUTLINE")]
    [SerializeField] GameObject outline;
    [SerializeField] Animacio_Scriptable clickDown;
    [SerializeField] Animacio_Scriptable clickUp;
    Coroutine compteEnrerra;
    System.Action accioCrear;

    [SerializeField] bool seleccionada;

    bool Seleccionada
    {
        set
        {
            seleccionada = value;
            if (seleccionada)
            {
                clickDown.Play(gameObject);
            }
            else
            {
                clickUp.Play(gameObject);
            }
        }
    }

    //Prev� multiples clics.
    //bool autobloquejar = false;
    public override bool EsPe�a => false;

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
            Debugar.Log("BLOQUEJAT!!!");
            return;
        }

        if (!Fase_Colocar.PermesColoarPe�a)
            return;

        /*if (autobloquejar)
            return;

        autobloquejar = true;
        */
        //CrearPe�a();

        colocar.CrearPe�a(Coordenades);
        //crearPe�a.Invoke(Coordenades);
        //animacioPerCodi.Play();
        animacio.Play(this.gameObject);
        Destroy(this.gameObject);

        Debugar.Log("Destruir...");
    }

    public void CrearPe�a() 
    {
        colocar.CrearPe�a(Coordenades);
        //crearPe�a.Invoke(Coordenades);
        //animacioPerCodi.Play();
        animacio.Play(this.gameObject);
        Destroy(this.gameObject);
    }

    IEnumerator Deseleccionar()
    {
        yield return new WaitForSeconds(3);
        Seleccionada = false;
        compteEnrerra = null;
    }


    //INTERACCIO
    public void OnPointerDown(PointerEventData eventData) 
    {
        if (seleccionada)
            return;

        Seleccionada = true;
        
        compteEnrerra = StartCoroutine(Deseleccionar());
    }
    public void OnPointerUp(PointerEventData eventData) 
    {
        if (!seleccionada)
            return;

        accioCrear?.Invoke();
    } 

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        outline.SetActive(true);
        onEnter?.Invoke();
        proximitat.PossiblesCombinacions(Coordenades);
        //outline.material.SetFloat(SELECCIONAT_ID, 1);
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        proximitat.AmagarInformacioMostrada(Coordenades);

        Seleccionada = false;
        if (compteEnrerra != null) StopCoroutine(compteEnrerra);
        onExit?.Invoke();
        //outline.material.SetFloat(SELECCIONAT_ID, 0);
    }
}
