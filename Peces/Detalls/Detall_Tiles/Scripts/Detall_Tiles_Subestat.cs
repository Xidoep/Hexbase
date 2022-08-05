using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Subestat")]
public class Detall_Tiles_Subestat : Detall_Tiles
{
    [SerializeField] Subestat subestat;

    //INTERN
    List<Peça> veins;
    public override int[] Get(Peça peça)
    {
        List<int> tmp = new List<int>();

        veins = peça.VeinsPeça;
        for (int i = 0; i < veins.Count; i++)
        {
            if ((veins[i]).SubestatIgualA(subestat)) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
