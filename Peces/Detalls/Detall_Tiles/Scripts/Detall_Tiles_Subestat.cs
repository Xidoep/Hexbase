using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Subestat")]
public class Detall_Tiles_Subestat : Detall_Tiles
{
    [SerializeField] Subestat subestat;

    //INTERN
    List<Hexagon> veins;
    List<int> tmp;
    public override int[] Get(Peça peça)
    {
        tmp = new List<int>();

        veins = peça.Veins;
        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i] != null && veins[i].EsPeça)
                if (((Peça)veins[i]).SubestatIgualA(subestat)) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
