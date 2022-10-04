using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat
{
    [Apartat("RECURSOS")]
    [SerializeField] Producte producte;
    [SerializeField] EstrategiaDeProduccio estrategia;

    public override Producte[] Produccio() => estrategia.Produir(producte);

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
