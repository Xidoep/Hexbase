using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byProducte")]
public class Condicio_Producte : Condicio
{
    [Apartat("CONDICIO PRODUCTE")]
    [SerializeField] Subestat subestat;

    //INTERN
    List<Pe�a> veins;

    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;


        veins = GetVeinsAcordingToOptions(pe�a);

        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i].SubestatIgualA(subestat) && veins[i].LLiure)
            {
                veins[i].Ocupar(pe�a);
                Canviar(pe�a);
                return true;
            }
        }

        return false;
    }

    protected override void Setup()
    {
        base.Setup();

        cami = true;
        grup = false;
    }
}
