using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;

public class UI_Peca : MonoBehaviour
{
    public void Setup(
        EstatColocable estat,
        AnimacioPerCodi_GameObject_Referencia outline,
        AnimacioPerCodi desapareixre,
        AnimacioPerCodi desapareixreParent,
        AnimacioPerCodi primeraPosicio,
        AnimacioPerCodi primeraPosicioParent,
        AnimacioPerCodi segonaPosicio,
        AnimacioPerCodi segonaPosicioParent
        )
    {
        this.estat = estat;
        this.outline = outline;
        this.desapareixre = desapareixre;
        this.desapareixreParent = desapareixreParent;
        this.primeraPosicio = primeraPosicio;
        this.primeraPosicioParent = primeraPosicioParent;
        this.segonaPosicio = segonaPosicio;
        this.segonaPosicioParent = segonaPosicioParent;
    }

    [SerializeField] EstatColocable estat;
    Transform[] childs;
    public bool resaltat;
    [SerializeField] Fase_Colocar colocar;

    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi_GameObject_Referencia outline;
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi desapareixre;
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi desapareixreParent;
    [Space(5)]
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi primeraPosicio;
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi primeraPosicioParent;
    [Space(5)]
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi segonaPosicio;
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi segonaPosicioParent;

    //Posar una var que posi RESALTADA.
    //Quan vagi el MostrarSeleccioanda de Menu_FreeStyle,
    //mirar totes les creades i només aplicar la animacio aa aquelles que estan Resaltades.

    public bool Seleccionada => estat == colocar.Seleccionada;

    UI_Peca instanciada;

    public void Seleccionar() 
    {
        Debug.Log("Seleccionar");
        colocar.Seleccionar(estat);
        outline.PointerUp();
        resaltat = true;
    }

    public void Resaltar() 
    {
        Debug.Log("Resaltar");
        outline.PointerEnter();
        resaltat = true;
        
        //outline.gameObject.SetActive(true);
    } 
    public void Desresaltar()
    {
        if (Seleccionada)
            return;

        outline.PointerExit();
        resaltat = false;
        //outline.Amagar();
    }
    public void MostrarOAmagar(EstatColocable seleccionat)
    {
        if(estat == seleccionat)
            outline.PointerEnter();
        else outline.PointerExit();
    }

    public UI_Peca Crear(bool capa_UIPeces = true)
    {
        instanciada = Instantiate(this);

        if (capa_UIPeces)
        {
            childs = instanciada.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].SetCapa_UIPeces();
            }
        }

        return instanciada;
    }

    public void Desapareixre()
    {
        desapareixre.Play(transform);
        desapareixreParent.Play(transform.parent.GetComponent<RectTransform>());
    }

    public void Posicio1()
    {
        primeraPosicio.Play(transform);
        primeraPosicioParent.Play(transform.parent.GetComponent<RectTransform>());
    }
    public void Posicio2()
    {
        segonaPosicio.Play(transform);
        segonaPosicioParent.Play(transform.parent.GetComponent<RectTransform>());
    }

    /*private void OnEnable()
    {
        childs = transform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].SetCapa_UIPeces();
        }
    }*/

    private void OnValidate()
    {
        if (colocar == null) colocar = XS_Editor.LoadAssetAtPath<Fase_Colocar>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Colocar.asset");
    }
}
