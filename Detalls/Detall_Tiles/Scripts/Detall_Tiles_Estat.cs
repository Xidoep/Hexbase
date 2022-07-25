using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Estat")]
public class Detall_Tiles_Estat : Detall_Tiles
{
    [SerializeField] Estat estat;
    public override int[] Get(Peça peça)
    {
        List<int> tmp = new List<int>();

        Hexagon[] veins = peça.Veins;
        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i] != null && veins[i].EsPeça)
            {
                if (((Peça)veins[i]).EstatIgualA(estat)) tmp.Add(i);
            }
        }
        return tmp.ToArray();
    }
}
