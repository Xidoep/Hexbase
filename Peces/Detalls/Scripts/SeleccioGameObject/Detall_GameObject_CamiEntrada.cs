using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/GameObject/Tile dependent")]
public class Detall_GameObject_CamiEntrada : Detall_GameObject
{
    [SerializeField] Estat vei;
    [SerializeField] Dependencia[] dependencies;

    //INTERN
    GameObject tmp;
    public override GameObject Get(Peça peça, TilePotencial tile)
    {
        if(vei != null)
        {
            if (!Cohincidieix(tile.Veins[0].Peça.Estat))
                return null;
        }

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

    bool Cohincidieix(Estat estat) => estat == vei;
}
