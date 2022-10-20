using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Centre Tile")]
public class Detall_Modif_CentreTile : Detall_Modificacio
{
    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        detall.transform.position = tile.Peça.transform.position + tile.Peça.transform.forward * 0.6f;
    }
}
