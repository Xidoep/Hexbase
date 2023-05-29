using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;


public abstract class TileSetBase : ScriptableObject
{
    public abstract void Setup();

    public abstract TilesPossibles[] Tiles(Pe�a pe�a = null);
    public abstract Connexio[] ConnexionsNules(Pe�a pe�a = null);
    public abstract ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a = null);
    public abstract Connexio[] ConnexioinsPossibles(Pe�a pe�a = null);
    public bool TeConnexionsNules(Pe�a pe�a) => ConnexionsNules(pe�a).Length > 0;

    


}

[System.Serializable]
public class TileSet
{
    public TileSet Setup()
    {
        tiles = new TilesPossibles[0];
        connexionsNules = new Connexio[0];
        connexioEspesifica = new ConnexioEspesifica(new List<Estat>(), new List<Connexio>());
        return this;
    }

    [TableList(AlwaysExpanded = true, DrawScrollView = false), BoxGroup("Tiles", centerLabel: true)]
    [SerializeField] TilesPossibles[] tiles;

    [BoxGroup("Connexions", centerLabel: true), BoxGroup("Connexions/Nules", centerLabel: true), AssetSelector(Paths = "Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats")]
    [SerializeField] Connexio[] connexionsNules;

    [BoxGroup("Connexions/Especifiques", centerLabel: true)]
    [SerializeField] ConnexioEspesifica connexioEspesifica;

    [Space(20)]
    [ReadOnly]
    [SerializeField] Connexio[] connexionsPossibles;

    public TilesPossibles[] Tiles => tiles;
    public Connexio[] ConnexionsNules { get => connexionsNules; set => connexionsNules = value; }
    public ConnexioEspesifica ConnexioEspesifica { get => connexioEspesifica; set => connexioEspesifica = value; }
    public Connexio[] ConnexionsPossibles => connexionsPossibles;

    public void AddTile(Tile tile, int pes)
    {
        List<TilesPossibles> tiles = new List<TilesPossibles>(this.tiles);
        tiles.Add(new TilesPossibles(tile, pes));
        if(tiles.Count > 1)
        {
            for (int i = 0; i < tiles.Count; i++)
            {

            }
        }

        this.tiles = tiles.ToArray();
    }
    public void NetejarTiles() => tiles = new TilesPossibles[0];

    public void SetConnexionsPossibles()
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

    //[PreviewField(Height = 20)]
    [TableColumnWidth(40)]
    public Tile tile;

    [TableColumnWidth(200, Resizable = false)]
    [Range(0,10)]public int pes;
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