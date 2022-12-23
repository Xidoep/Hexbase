using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byOcupat")]
public class Condicio_Ocupat : Condicio
{
    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat, Grups grups, Estat cami, System.Action<Pe�a, int> enCanviar)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        if (pe�a.Ocupat) 
        {
            Canviar(pe�a, enCanviar);
            return true;
        }
        else return false;
    }
}
