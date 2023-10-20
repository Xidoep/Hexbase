using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XS_Utils;

public class Ranura : Hexagon, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Apartat("FASES/PROCESSOS")]
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Prediccio prediccio;
    
    [Apartat("OUTLINE")]
    Coroutine compteEnrerra;

    [SerializeField] bool seleccionada;

    bool Seleccionada
    {
        set
        {
            seleccionada = value;
        }
    }
    public override bool EsPeça => false;

    private void OnEnable()
    {
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

        if (!Fase_Colocar.PermesColoarPeça)
            return;;

        colocar.CrearPeça(Coordenades);
        Debugar.Log("Destruir...");
        Destroy(this.gameObject);
    }


    IEnumerator Deseleccionar()
    {
        yield return new WaitForSeconds(3);
        Seleccionada = false;
        compteEnrerra = null;
    }





    //INTERACCIO
    public override void OnPointerDown()
    {
        if (seleccionada)
            return;

        Seleccionada = true;

        compteEnrerra = StartCoroutine(Deseleccionar());
    }

    public override void OnPointerUp()
    {
        if (!seleccionada)
            return;

        Crear();
    }
    public override void OnPointerEnter()
    {
        CursorEstat.Snap(transform.position);

        //prediccio.Predir(Coordenades);
    }
    public override void OnPointerExit()
    {
        CursorEstat.NoSnap();

        //prediccio.AmagarInformacioMostrada();

        Seleccionada = false;
        if (compteEnrerra != null) StopCoroutine(compteEnrerra);
    }














    public void OnPointerDown(PointerEventData eventData) => OnPointerDown();
    public void OnPointerUp(PointerEventData eventData) => OnPointerUp();
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();

}
