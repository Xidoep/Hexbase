using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Ocupable")]
public class TileSet_Ocupable : TileSetBase
{
    public override void Setup()
    {
        lliure = new TileSet();
        ocupat = new TileSet();
    }

    [SerializeField] TileSet lliure;
    [SerializeField] TileSet ocupat;


    public override TilesPossibles[] Tiles(Pe�a pe�a = null) => pe�a.EstaConnectat ? ocupat.Tiles : lliure.Tiles;
    public override Connexio[] ConnexionsNules(Pe�a pe�a = null) => pe�a.EstaConnectat ? ocupat.ConnexionsNules : lliure.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a = null) => pe�a.EstaConnectat ? ocupat.ConnexioEspesifica : lliure.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Pe�a pe�a = null) => pe�a.EstaConnectat ? ocupat.ConnexionsPossibles : lliure.ConnexionsPossibles;


    public TileSet GetTileSetLliure => lliure;
    public TileSet GetTileSetOcupat => ocupat;


    protected void OnValidate()
    {
        lliure.Setup();
        ocupat.Setup();
    }

    
}
