using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byOcupat")]
public class Condicio_Ocupat : Condicio
{
    public override bool AfterWFC => false;
    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat, Grups grups, Estat cami)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        if (pe�a.Ocupat) 
        {
            Canviar(pe�a);
            return true;
        }
        else return false;
    }
}
