using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;

public class UI_Peca : MonoBehaviour
{
    public void Setup(EstatColocable estat, AnimacioPerCodi_GameObject_Referencia outline)
    {
        this.estat = estat;
        this.outline = outline;
    }

    [SerializeField] EstatColocable estat;
    [SerializeField] AnimacioPerCodi_GameObject_Referencia outline;
    Transform[] childs;
    public bool resaltat;
    [SerializeField] Fase_Colocar colocar;

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
