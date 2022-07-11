using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byHabitantDisponible")]
public class Condicio_HabitantDisponible : Condicio
{
    [Space(20)]
    [Header("HABITANT DISPONIBLE")]
    [SerializeField] Estat casa;

    //INTERN
    Pe�a[] veins;

    public override bool Comprovar(Pe�a pe�a)
    {
        if (pe�a.Subestat == objectiu)
            return false;

        veins = pe�a.VeinsPe�a;

        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EstatIgualA(casa)) 
            {
                if (veins[i].TeCases)
                {
                    Habitant habitant = veins[i].HabitantLLiure;
                    if(habitant != null)
                    {
                        Canviar(pe�a);
                        habitant.Ocupar(pe�a);
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
