using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condicio on es busca una quantitat d'estat concrets al voltant de la peça.
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
    Peça[] veins; 

    public override bool Comprovar(Peça peça)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;
        _cohincidit = false;

        veins = peça.VeinsPeça;

        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EstatIgualA(estat)) _quantitat++;
        }

        if(_cohincidit = _quantitat >= quantitat)
        {
            Canviar(peça);
        }
        
        return _cohincidit;
    }
}
