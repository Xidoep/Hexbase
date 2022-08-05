using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Subestats")]
public class Detall_Tiles_Subestats : Detall_Tiles
{
    [SerializeField] List<Subestat> subestats;

    //INTERN
    List<Peça> veins;
    public override int[] Get(Peça peça)
    {
        List<int> tmp = new List<int>();

        veins = peça.VeinsPeça;
        for (int i = 0; i < veins.Count; i++)
        {
           
            if (subestats.Contains(((Peça)veins[i]).Subestat)) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
