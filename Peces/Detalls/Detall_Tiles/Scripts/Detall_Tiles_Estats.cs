using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Estats")]
public class Detall_Tiles_Estats : Detall_Tiles
{
    [SerializeField] List<Estat> estats;
    //INTERN
    List<Pe�a> veins;

    public override int[] Get(Pe�a pe�a)
    {
        List<int> tmp = new List<int>();

        veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Count; i++)
        {
            if (estats.Contains(((Pe�a)veins[i]).Estat)) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
