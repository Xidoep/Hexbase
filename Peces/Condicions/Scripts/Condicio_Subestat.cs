using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/bySubestat")]
public class Condicio_Subestat : Condicio
{
    [Apartat("CONDICIO SUBESTAT")]
    [SerializeField] Subestat subestat;
    [SerializeField] [Range(1,6)] int quantitat = 1;

    //INTERN
    int _quantitat = 0;
    List<Peça> myVeins;

    public override bool Comprovar(Peça peça, Grups grups, Estat cami, System.Action<Peça, int> enCanviar)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;

        myVeins = peça.VeinsPeça;

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (myVeins[i].SubestatIgualA(subestat)) _quantitat++;
        }

        if (_quantitat >= quantitat)
        {
            //Canviar(peça, enCanviar);
            return true;
        }

        return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();
    }
}
