using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Centre Tile")]
public class Detall_Modif_CentreTile : Detall_Modificacio
{
    [SerializeField] Vector3 random;
    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        detall.transform.position = tile.TileFisic.transform.position + Centre(tile) + RandomForward(tile) + RandomRight(tile);
    }

    Vector3 Centre(TilePotencial tile) => tile.TileFisic.transform.forward * 0.6f;
    Vector3 RandomForward(TilePotencial tile) => tile.TileFisic.transform.forward * (Random.Range(random.z, -random.z));
    Vector3 RandomRight(TilePotencial tile) => tile.TileFisic.transform.right * (Random.Range(random.x, -random.x));

}
