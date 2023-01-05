using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/bySubestats")]
public class Condicio_Subestats : Condicio
{
    [Apartat("CONDICIO SUBESTAT")]
    [SerializeField] List<Subestat> subestats;
    [SerializeField] [Range(1, 6)] int quantitat = 1;

    //INTERN
    int _quantitat = 0;
    List<Peça> myVeins;

    public override bool Comprovar(Peça peça, Grups grups, Estat cami, bool canviar, System.Action<Peça, bool> enConfirmar, System.Action<Peça, int> enCanviar)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;

        myVeins = peça.VeinsPeça;

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (subestats.Contains(myVeins[i].Subestat)) _quantitat++;
        }

        if (_quantitat >= quantitat)
        {
            enConfirmar.Invoke(peça, canviar);
            if (canviar)
                Canviar(peça, enCanviar);
            return true;
        }

        return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();
    }
}
