using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byOcupat")]
public class Condicio_Ocupat : Condicio
{
    public override bool Comprovar(Peça peça, Proximitat proximitat, Grups grups, Estat cami, System.Action<Peça, int> enCanviar)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        if (peça.Ocupat) 
        {
            Canviar(peça, enCanviar);
            return true;
        }
        else return false;
    }
}
