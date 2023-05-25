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


    public override TilesPossibles[] Tiles(Peça peça = null) => peça.EstaConnectat ? ocupat.Tiles : lliure.Tiles;
    public override Connexio[] ConnexionsNules(Peça peça = null) => peça.EstaConnectat ? ocupat.ConnexionsNules : lliure.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Peça peça = null) => peça.EstaConnectat ? ocupat.ConnexioEspesifica : lliure.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Peça peça = null) => peça.EstaConnectat ? ocupat.ConnexionsPossibles : lliure.ConnexionsPossibles;


    public TileSet GetTileSetLliure => lliure;
    public TileSet GetTileSetOcupat => ocupat;


    protected void OnValidate()
    {
        lliure.Setup();
        ocupat.Setup();
    }

    
}
