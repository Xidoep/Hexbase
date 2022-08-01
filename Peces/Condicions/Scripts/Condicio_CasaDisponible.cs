using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byHabitantDisponible")]
public class Condicio_CasaDisponible : Condicio
{
    [Apartat("CONDICIO CASA DISPONIBLE")]
    //[SerializeField] Estat casa;


    //INTERN
    List<Pe�a> veins;
    List<Pe�a> veinsAmbCami;


    public override bool Comprovar(Pe�a pe�a)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;



        return false;
    }

    /*bool Seleccionar(Pe�a pe�a, Casa casa)
    {
        if (casa != null)
        {
            Canviar(pe�a);
            casa.Ocupar(pe�a);
            return true;
        }
        else return true;
    }*/

    protected override void Setup()
    {

        base.Setup();
    }
}
