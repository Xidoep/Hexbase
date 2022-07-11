using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Estats")]
public class Detall_Tiles_Estats : Detall_Tiles
{
    [SerializeField] List<Estat> estats;
    public List<Estat> Estats => estats;

    public override int[] Get(Peça peça)
    {
        List<int> tmp = new List<int>();

        Hexagon[] veins = peça.Veins;
        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EsPeça)
            {
                if (estats.Contains(((Peça)veins[i]).Estat)) tmp.Add(i);
            }
        }
        return tmp.ToArray();
    }
}
