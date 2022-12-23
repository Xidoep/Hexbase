using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condicio on es busca una quantitat d'estat concrets al voltant de la pe�a.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byEstat")]
public class Condicio_Estat : Condicio
{
    [Apartat("CONDICIO ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] [Range(1, 6)] int quantitat = 1;

    //INTERN
    int _quantitat = 0;
    bool _cohincidit = false;
    List<Pe�a> myVeins;

    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat, Grups grups, Estat cami, System.Action<Pe�a, int> enCanviar)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;
        _cohincidit = false;

        myVeins = GetVeinsAcordingToOptions(pe�a, grups, cami);

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (myVeins[i].EstatIgualA(estat)) _quantitat++;
        }

        if(_cohincidit = _quantitat >= quantitat)
        {
            Canviar(pe�a, enCanviar);
        }
        
        return _cohincidit;
    }
}
