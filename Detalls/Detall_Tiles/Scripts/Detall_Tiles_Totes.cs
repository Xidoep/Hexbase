using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Tiles/All")]
public class Detall_Tiles_Totes : Detall_Tiles
{
    public override int[] Get(Peça peça) => new int[] { 0, 1, 2, 3, 4, 5 };
}
