using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condicio on es busca una quantitat d'estat concrets al voltant de la pe�a.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byEstat")]
public class Condicio_Estat : Condicio
{
    [Linia]
    [Header("ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] int quantitat;
    //INTERN
    int _quantitat = 0;
    bool _cohincidit = false;
    Pe�a[] veins; 

    public override bool Comprovar(Pe�a pe�a)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;
        _cohincidit = false;

        veins = pe�a.VeinsPe�a;

        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EstatIgualA(estat)) _quantitat++;
        }

        if(_cohincidit = _quantitat >= quantitat)
        {
            Canviar(pe�a);
        }
        
        return _cohincidit;
    }
}
