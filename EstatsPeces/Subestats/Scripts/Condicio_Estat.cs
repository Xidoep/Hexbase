using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byEstat")]
public class Condicio_Estat : Condicio
{
    [SerializeField] EstatPeça estat;
    [SerializeField] int quantitat;

    int _quantitat = 0;

    public override bool Comprovar(Hexagon peça)
    {
        _quantitat = 0;
        for (int i = 0; i < peça.Veins.Length; i++)
        {
            if (peça.Veins[i].EstatIgualA(peça.Estat)) _quantitat++;
        }

        return _quantitat >= quantitat;
    }
}
