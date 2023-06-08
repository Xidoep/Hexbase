using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Simple")]
public class TileSet_Simple : TileSetBase
{
    public override void Setup()
    {
        tileSet = new TileSet().Setup();
    }

    [SerializeField] TileSet tileSet;


    public override TilesPossibles[] Tiles(Pe�a pe�a = null) => tileSet.Tiles;
    public override Connexio[] ConnexionsNules(Pe�a pe�a = null) => tileSet.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a = null) => tileSet.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Pe�a pe�a = null) => tileSet.ConnexionsPossibles;


    public TileSet TileSet => tileSet;

    protected void OnValidate()
    {
        tileSet.SetConnexionsPossibles();
    }

}
