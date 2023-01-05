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
    List<Pe�a> myVeins;

    public override bool Comprovar(Pe�a pe�a, Grups grups, Estat cami, bool canviar, System.Action<Pe�a, bool> enConfirmar, System.Action<Pe�a, int> enCanviar)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;

        myVeins = pe�a.VeinsPe�a;

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (subestats.Contains(myVeins[i].Subestat)) _quantitat++;
        }

        if (_quantitat >= quantitat)
        {
            enConfirmar.Invoke(pe�a, canviar);
            if (canviar)
                Canviar(pe�a, enCanviar);
            return true;
        }

        return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();
    }
}
