using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Caminables")]
public class Detall_Tiles_Caminables : Detall_Tiles
{

    //INTERN
    List<Pe�a> veins;
    public override int[] Get(Pe�a pe�a)
    {
        List<int> tmp = new List<int>();

        veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i].Subestat.Caminable) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
