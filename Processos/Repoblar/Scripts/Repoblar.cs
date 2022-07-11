using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Estat casa;
    [SerializeField] Detall_Tiles_Estat detall_Tiles;

    //INTERN
    int[] veins;

    public void Proces(List<Peça> peces)
    {

        for (int p = 0; p < peces.Count; p++)
        {
            if (!peces[p].EstatIgualA(casa))
                continue;

            AfegirCasa(peces[p], 1 + detall_Tiles.Get(peces[p]).Length);
        }
    }

    public void AfegirCasa(Peça peça, int cases)
    {
        if (cases > peça.Cases) //S'han de crear tantes cases com fagi falta
        {
            for (int i = peça.Cases; i < cases; i++)
            {
                peça.AddCasa();
                //Crear
            }
        }
        else if(cases < peça.Cases)
        {
            for (int i = cases; i < peça.Cases; i++)
            {
                peça.RemoveCasa();
                //Remove
            }
        } //S'han de borrar tantes cases com faci falta;


    }
}


