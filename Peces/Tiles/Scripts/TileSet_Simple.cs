using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Simple")]
public class TileSet_Simple : TileSetBase
{
    [SerializeField] TileSet tileSet;

    public override TilesPossibles[] Tiles(Peça peça = null) => tileSet.Tiles;
    public override Connexio[] ConnexionsNules(Peça peça = null) => tileSet.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Peça peça = null) => tileSet.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Peça peça = null) => tileSet.ConnexionsPossibles;

    protected void OnValidate()
    {
        tileSet.Setup();
    }

}
