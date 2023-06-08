using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;


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
    public TileSet Setup()
    {
        tiles = new TilesPossibles[0];
        connexionsNules = new Connexio[0];
        connexioEspesifica = new ConnexioEspesifica(new List<Estat>(), new List<Connexio>());
        return this;
    }

    [PropertyOrder(1), TableList(ShowPaging = true), BoxGroup("Tiles", centerLabel: true), SerializeField] 
    TilesPossibles[] tiles;

    [PropertyOrder(3), BoxGroup("Connexions", centerLabel: true), BoxGroup("Connexions/Nules", centerLabel: true), AssetSelector(Paths = "Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats")]
    [SerializeField] Connexio[] connexionsNules;

    [PropertyOrder(4), BoxGroup("Connexions/Especifiques", centerLabel: true)]
    [SerializeField] ConnexioEspesifica connexioEspesifica;


    [Space(20)]
    [ReadOnly]
    [PropertyOrder(5), SerializeField] Connexio[] connexionsPossibles;

    public TilesPossibles[] Tiles { get => tiles; set => tiles = value; }
    public Connexio[] ConnexionsNules { get => connexionsNules; set => connexionsNules = value; }
    public ConnexioEspesifica ConnexioEspesifica { get => connexioEspesifica; set => connexioEspesifica = value; }
    public Connexio[] ConnexionsPossibles => connexionsPossibles;

    //INTERN
    bool tTrobat;


    public bool Contains(Tile tile)
    {
        tTrobat = false;
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].tile.Equals(tile))
            {
                tTrobat = true;
                break;
            }
        }
        return tTrobat;
    }

    public void AddTile(Tile tile, int pes)
    {
        //if (this.tiles == null) this.tiles = new TilesPossibles[0];

        List<TilesPossibles> tiles = new List<TilesPossibles>(this.tiles);
        //tiles.Add(new TilesPossibles(tile, pes));
        if(tiles.Count > 1)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].pes < pes || i == tiles.Count - 1)
                {
                    tiles.Insert(i, new TilesPossibles(tile, pes));
                    break;
                }
            }
        }
        else
        {
            tiles.Add(new TilesPossibles(tile, pes));
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

    [PropertyOrder(2), Button("Ordenar")]
    void Organitzar()
    {
        List<TilesPossibles> tiles = new List<TilesPossibles>(this.tiles);
        tiles.Sort(CompararPes);

        this.tiles = tiles.ToArray();
    }
    
    int CompararPes(TilesPossibles x, TilesPossibles y) => y.pes - x.pes;
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

    public Tile tile;

    [TableColumnWidth(200, Resizable = false)]
    [Range(0,10)]public int pes;


    [TableColumnWidth(56, resizable: false),ShowInInspector,PreviewField,PropertyOrder(-1)]
    Object Prefab => tile?.Prefab;

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