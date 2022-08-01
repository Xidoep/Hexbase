using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Subestat")]
public class Detall_Tiles_Subestat : Detall_Tiles
{
    [SerializeField] Subestat subestat;
    public override int[] Get(Pe�a pe�a)
    {
        List<int> tmp = new List<int>();

        List<Hexagon> veins = pe�a.Veins;
        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i] != null && veins[i].EsPe�a)
            {
                if (((Pe�a)veins[i]).SubestatIgualA(subestat)) tmp.Add(i);
            }
        }
        return tmp.ToArray();
    }
}
