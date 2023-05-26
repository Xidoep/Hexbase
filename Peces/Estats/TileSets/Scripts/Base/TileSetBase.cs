using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;


public abstract class TileSetBase : ScriptableObject
{
    public abstract void Setup();

    public abstract TilesPossibles[] Tiles(Peça peça = null);
    public abstract Connexio[] ConnexionsNules(Peça peça = null);
    public abstract ConnexioEspesifica ConnexionsEspesifica(Peça peça = null);
    public abstract Connexio[] ConnexioinsPossibles(Peça peça = null);
    public bool TeConnexionsNules(Peça peça) => ConnexionsNules(peça).Length > 0;

    


}

[System.Serializable]
public class TileSet
{
    [SerializeField] TilesPossibles[] tiles;
    [SerializeField] Connexio[] connexionsNules;
    [SerializeField] ConnexioEspesifica connexioEspesifica;

    [Space(20)]
    [Nota("S'emplena automaticament")]
    [SerializeField] Connexio[] connexionsPossibles;

    public TilesPossibles[] Tiles => tiles;
    public Connexio[] ConnexionsNules { get => connexionsNules; set => connexionsNules = value; }
    public ConnexioEspesifica ConnexioEspesifica { get => connexioEspesifica; set => connexioEspesifica = value; }
    public Connexio[] ConnexionsPossibles => connexionsPossibles;

    public void AddTile(Tile tile, int pes)
    {
        List<TilesPossibles> tiles = new List<TilesPossibles>(this.tiles);
        tiles.Add(new TilesPossibles(tile, pes));
        this.tiles = tiles.ToArray();
    }
    public void NetejarTiles() => tiles = new TilesPossibles[0];

    public void Setup()
    {
        List<Connexio> tmpConnexions = new List<Connexio>();
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tmpConnexions.Contains(tiles[i].tile.Exterior(0))) tmpConnexions.Add(tiles[i].tile.Exterior(0));
            if (!tmpConnexions.Contains(tiles[i].tile.Esquerra(0))) tmpConnexions.Add(tiles[i].tile.Esquerra(0));
            if (!tmpConnexions.Contains(tiles[i].tile.Dreta(0))) tmpConnexions.Add(tiles[i].tile.Dreta(0));
        }

        connexionsPossibles = tmpConnexions.ToArray();
    }

   
}

[System.Serializable]
public struct TilesPossibles
{
    public TilesPossibles(Tile tile, int pes)
    {
        this.tile = tile;
        this.pes = pes;
    }

    public Tile tile;
    [Range(1, 10)] public int pes;
}

[System.Serializable]
public class ConnexioEspesifica
{
    public ConnexioEspesifica(List<Estat> estats, List<Connexio> connexions)
    {
        this.subestats = estats;
        this.connexions = connexions.ToArray();
    }
    public List<Estat> subestats;
    public Connexio[] connexions;
}