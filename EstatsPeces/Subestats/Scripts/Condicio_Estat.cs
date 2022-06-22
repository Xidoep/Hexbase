using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byEstat")]
public class Condicio_Estat : Condicio
{
    [SerializeField] EstatPe�a estat;
    [SerializeField] int quantitat;

    int _quantitat = 0;

    public override bool Comprovar(Hexagon pe�a)
    {
        _quantitat = 0;
        for (int i = 0; i < pe�a.Veins.Length; i++)
        {
            if (pe�a.Veins[i].EstatIgualA(pe�a.Estat)) _quantitat++;
        }

        return _quantitat >= quantitat;
    }
}
