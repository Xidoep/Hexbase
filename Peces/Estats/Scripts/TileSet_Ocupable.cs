using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Ocupable")]
public class TileSet_Ocupable : TileSetBase
{
    public override void Setup()
    {
        lliure = new TileSet().Setup();
        ocupat = new TileSet().Setup();
    }

    [SerializeField] TileSet lliure;
    [SerializeField] TileSet ocupat;


    public override TilesPossibles[] Tiles(Peça peça = null) => peça.EstaConnectat ? ocupat.Tiles : lliure.Tiles;
    public override Connexio[] ConnexionsNules(Peça peça = null) => peça.EstaConnectat ? ocupat.ConnexionsNules : lliure.ConnexionsNules;
    public override ConnexioEspesifica[] ConnexionsEspesifiques(Peça peça = null) => peça.EstaConnectat ? ocupat.ConnexionsEspesifiques : lliure.ConnexionsEspesifiques;
    public override Connexio[] ConnexioinsPossibles(Peça peça = null) => peça.EstaConnectat ? ocupat.ConnexionsPossibles : lliure.ConnexionsPossibles;


    public TileSet TileSetLliure => lliure;
    public TileSet TileSetOcupat => ocupat;


    protected void OnValidate()
    {
        lliure.SetConnexionsPossibles();
        ocupat.SetConnexionsPossibles();
    }

    
}
