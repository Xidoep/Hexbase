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
    List<Peça> veins;
    List<Peça> veinsAmbCami;


    public override bool Comprovar(Peça peça)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;



        return false;
    }

    /*bool Seleccionar(Peça peça, Casa casa)
    {
        if (casa != null)
        {
            Canviar(peça);
            casa.Ocupar(peça);
            return true;
        }
        else return true;
    }*/

    protected override void Setup()
    {

        base.Setup();
    }
}
