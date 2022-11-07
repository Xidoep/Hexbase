using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/All amb excepcions")]
public class Detall_Tiles_TotesAmbExepcions : Detall_Tiles
{
    [SerializeField] List<Tile> prohibits;
    List<int> tmp;
    public override int[] Get(Peça peça) 
    {
        tmp = new List<int>();

        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (!prohibits.Contains(peça.Tiles[i].PossibilitatsVirtuals.Get(0).Tile)) tmp.Add(i);
        }

        return tmp.ToArray();
    }
}
