using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat Riu")]
public class Subestat_Riu : Subestat
{

    public override Subestat Setup(Peça peça)
    {
        this.peça = peça;

        return base.Setup(peça);
    }
    Peça peça;

    /*public override Connexio[] ConnexionsNules
    {
        get
        {
            if (peça.VeinsPeça.Count < 2)
                return ConnexionsPossibles;

            return ConnexionsNules;
        }
    }*/

    private void OnValidate()
    {
        List<Connexio> tmpConnexions = new List<Connexio>();
        for (int i = 0; i < Tiles.Length; i++)
        {
            if (!tmpConnexions.Contains(Tiles[i].tile.Exterior(0))) tmpConnexions.Add(Tiles[i].tile.Exterior(0));
            if (!tmpConnexions.Contains(Tiles[i].tile.Esquerra(0))) tmpConnexions.Add(Tiles[i].tile.Esquerra(0));
            if (!tmpConnexions.Contains(Tiles[i].tile.Dreta(0))) tmpConnexions.Add(Tiles[i].tile.Dreta(0));
        }

        connexionsPossibles = tmpConnexions.ToArray();
    }
}
