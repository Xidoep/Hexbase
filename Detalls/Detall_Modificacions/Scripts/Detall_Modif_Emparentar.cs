using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Emparentar")]
public class Detall_Modif_Emparentar : Detall_Modificacio
{
    public override void Modificar(GameObject tile, GameObject detall)
    {
        detall.transform.SetParent(tile.transform);
    }
}
