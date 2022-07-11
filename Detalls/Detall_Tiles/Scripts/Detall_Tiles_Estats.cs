using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Estats")]
public class Detall_Tiles_Estats : Detall_Tiles
{
    [SerializeField] List<Estat> estats;
    public List<Estat> Estats => estats;

    public override int[] Get(Pe�a pe�a)
    {
        List<int> tmp = new List<int>();

        Hexagon[] veins = pe�a.Veins;
        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EsPe�a)
            {
                if (estats.Contains(((Pe�a)veins[i]).Estat)) tmp.Add(i);
            }
        }
        return tmp.ToArray();
    }
}
