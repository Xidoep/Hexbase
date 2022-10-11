using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byEstats")]
public class Condicio_Estats : Condicio
{
    [Apartat("CONDICIO ESTAT")]
    [SerializeField] List<Estat> estats;
    [SerializeField] [Range(1, 6)] int quantitat = 1;

    //INTERN
    int _quantitat = 0;
    List<Pe�a> myVeins;

    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat, Grups grups, Estat cami)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        _quantitat = 0;
        myVeins = GetVeinsAcordingToOptions(pe�a, grups, cami);

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (estats.Contains(myVeins[i].Estat)) _quantitat++;
        }

        if (_quantitat >= quantitat)
        {
            Canviar(pe�a);
            return true;
        }

        return false;
    }
}