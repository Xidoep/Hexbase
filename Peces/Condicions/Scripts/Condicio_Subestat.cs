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
    bool _cohincidit = false;
    List<Pe�a> veins;

    public override bool Comprovar(Pe�a pe�a)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;
        _cohincidit = false;

        veins = pe�a.VeinsPe�a;

        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i].SubestatIgualA(subestat)) _quantitat++;
        }

        if (_cohincidit = _quantitat >= quantitat)
        {
            Canviar(pe�a);
        }

        return _cohincidit;
    }
}
