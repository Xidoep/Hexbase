using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XS_Utils;

public class Ranura : Hexagon, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    //VARIABLES
   /* [Linia]
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;
   */
    [Apartat("FASES/PROCESSOS")]
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Prediccio prediccio;
    
    [Apartat("OUTLINE")]
    //[SerializeField] GameObject outline;
    Coroutine compteEnrerra;
    //System.Action accioCrear;

    [SerializeField] bool seleccionada;

    bool Seleccionada
    {
        set
        {
            seleccionada = value;
            /*if (seleccionada)
            {
                clickDown.Play(transform);
            }
            else
            {
                clickUp.Play(transform);
            }*/
        }
    }

    //Prev� multiples clics.
    //bool autobloquejar = false;
    public override bool EsPe�a => false;

    private void OnEnable()
    {
        //accioCrear = Crear;
        
        transform.localEulerAngles = new Vector3(0, Random.Range(-5, 5), 0);
        transform.localScale = new Vector3(Random.Range(1.1f, 0.9f), 1, Random.Range(1.1f, 0.9f));
    }



    public void Crear()
    {
        if (Fase_Colocar.Bloquejat)
        {
            Debugar.Log("BLOQUEJAT!!!");
            return;
        }

        if (!Fase_Colocar.PermesColoarPe�a)
            return;;

        colocar.CrearPe�a(Coordenades);

        //Destroy(this.gameObject);

        Debugar.Log("Destruir...");
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

        Crear();
        //accioCrear?.Invoke();
    } 

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        //outline.SetActive(true);
        //onEnter?.Invoke();

        prediccio.Predir(Coordenades);
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        prediccio.AmagarInformacioMostrada();

        Seleccionada = false;
        if (compteEnrerra != null) StopCoroutine(compteEnrerra);

        //onExit?.Invoke();
    }
}
