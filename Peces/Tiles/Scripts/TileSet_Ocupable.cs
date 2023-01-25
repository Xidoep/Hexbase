using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Ocupable")]
public class TileSet_Ocupable : TileSetBase
{
    [SerializeField] TileSet lliure;
    [SerializeField] TileSet ocupat;


    public override TilesPossibles[] Tiles(Pe�a pe�a = null) => pe�a.Ocupat ? ocupat.Tiles : lliure.Tiles;
    public override Connexio[] ConnexionsNules(Pe�a pe�a = null) => pe�a.Ocupat ? ocupat.ConnexionsNules : lliure.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a = null) => pe�a.Ocupat ? ocupat.ConnexioEspesifica : lliure.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Pe�a pe�a = null) => pe�a.Ocupat ? ocupat.ConnexionsPossibles : lliure.ConnexionsPossibles;

    protected void OnValidate()
    {
        lliure.Setup();
        ocupat.Setup();
    }

    
}
