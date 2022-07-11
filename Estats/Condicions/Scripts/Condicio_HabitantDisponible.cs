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
    Peça[] veins;

    public override bool Comprovar(Peça peça)
    {
        if (peça.Subestat == objectiu)
            return false;

        veins = peça.VeinsPeça;

        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EstatIgualA(casa)) 
            {
                if (veins[i].TeCases)
                {
                    Habitant habitant = veins[i].HabitantLLiure;
                    if(habitant != null)
                    {
                        Canviar(peça);
                        habitant.Ocupar(peça);
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
