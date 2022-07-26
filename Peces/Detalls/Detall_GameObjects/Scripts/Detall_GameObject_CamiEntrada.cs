using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/GameObject/Tile dependent")]
public class Detall_GameObject_CamiEntrada : Detall_GameObject
{
    [SerializeField] Dependencia[] dependencies;

    //INTERN
    GameObject tmp;
    public override GameObject Get(Peça peça, TilePotencial tile)
    {
        tmp = null;
        for (int i = 0; i < dependencies.Length; i++)
        {
            if (dependencies[i].Cohincideix(tile))
            {
                tmp = detalls[dependencies[i].indexDetall];
                break;
            }
        }
        return tmp != null ? tmp : detalls[0];
    }


}
