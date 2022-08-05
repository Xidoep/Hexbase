using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/Subestats")]
public class Detall_Tiles_Subestats : Detall_Tiles
{
    [SerializeField] List<Subestat> subestats;

    //INTERN
    List<Pe�a> veins;
    public override int[] Get(Pe�a pe�a)
    {
        List<int> tmp = new List<int>();

        veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Count; i++)
        {
           
            if (subestats.Contains(((Pe�a)veins[i]).Subestat)) tmp.Add(i);
        }
        return tmp.ToArray();
    }
}
