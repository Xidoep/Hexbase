using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Estats")]
public class Detall_Tiles_Estats : Detall_Tiles
{
    [SerializeField] List<Estat> estats;
    //INTERN
    List<Peça> veins;

    public override int[] Get(Peça peça)
    {
        List<int> tmp = new List<int>();

        veins = peça.VeinsPeça;
        for (int i = 0; i < veins.Count; i++)
        {
            if (estats.Contains(((Peça)veins[i]).Estat)) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
