using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Ocupable")]
public class TileSet_Ocupable : TileSetBase
{
    [SerializeField] TileSet lliure;
    [SerializeField] TileSet ocupat;


    public override TilesPossibles[] Tiles(Peça peça = null) => peça.Connectat ? ocupat.Tiles : lliure.Tiles;
    public override Connexio[] ConnexionsNules(Peça peça = null) => peça.Connectat ? ocupat.ConnexionsNules : lliure.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Peça peça = null) => peça.Connectat ? ocupat.ConnexioEspesifica : lliure.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Peça peça = null) => peça.Connectat ? ocupat.ConnexionsPossibles : lliure.ConnexionsPossibles;

    protected void OnValidate()
    {
        lliure.Setup();
        ocupat.Setup();
    }

    
}
