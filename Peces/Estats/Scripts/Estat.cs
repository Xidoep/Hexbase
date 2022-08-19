using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Basic")]
public class Estat : ScriptableObject
{
    [Apartat("TILES")]
    //[SerializeField] Tile[] tilesInicials;
    //[SerializeField] Tile[] tilesPossibles;
    [SerializeField] TilesPossibles[] tiles;
    [Tooltip("S'emplena automaticament")][SerializeField] Connexio[] connexionsPossibles;
    
    [Apartat("SUBESTAT")]
    [SerializeField] Subestat inicial;
    [SerializeField] Condicio[] condicions;

    [Apartat("Visualitzacio")]
    [SerializeField] GameObject prefab;

    //PROPIETATS
    //public Tile[] Possibilitats() => tilesPossibles;
    public Possibilitats Possibilitats() => Possibilitats(tiles);
    public Possibilitats Possibilitats(TilesPossibles[] tiles)
    {
        Possibilitats ps = new Possibilitats(new List<Possibilitat>());
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].tile.ConnexionsIguals)
            {
                ps.Add(tiles[i].tile, 0, tiles[i].pes);
                ps.Add(tiles[i].tile, 1, tiles[i].pes);
                ps.Add(tiles[i].tile, 2, tiles[i].pes);
            }
            else ps.Add(tiles[i].tile, 0, tiles[i].pes * 3);
        }
        return ps;
    }
    public Connexio[] ConnexionsPossibles => connexionsPossibles;
    public Subestat SubestatInicial => inicial;
    public GameObject Prefag => prefab;

    //public virtual bool EsCasa => false;


    //public virtual void OnCreate(Peça peça) { }


    //Els tiles amb que comença la peça quan es crea
    /*public virtual void TilesInicials(TilePotencial[] tiles) 
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Escollir(WFC_Possibilitats.RandomTiles(tilesInicials), 0);
            //tiles[i].Escollir(new WaveFunctionColapse.Possibilitats(tilesPossibles[0], 0), 0);
        }
    }*/


    //Que retorna si troba un vei null. En alguns estat puc voler que sigui una altre cosa.
    public virtual Connexio[] VeiNull(TilePotencial tile) => null;




    //Retorna les condicions de canvi de l'estat mes es subestat si es passa.
    public Condicio[] Condicions(Subestat subestat = null)
    {
        List<Condicio> _tmp = new List<Condicio>(condicions);
        if (subestat != null) _tmp.AddRange(subestat.Condicions);
        return _tmp.ToArray();
    }




    protected bool EsVeiNull(TilePotencial tile) => tile.Veins[0] == null;







    void OnValidate()
    {
        List<Tile> tmpTiles = new List<Tile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            tmpTiles.Add(tiles[i].tile);
        }
        //tilesPossibles = tmpTiles.ToArray();

        List<Connexio> tmpConnexions = new List<Connexio>();
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tmpConnexions.Contains(tiles[i].tile.Exterior(0))) tmpConnexions.Add(tiles[i].tile.Exterior(0));
            if (!tmpConnexions.Contains(tiles[i].tile.Esquerra(0))) tmpConnexions.Add(tiles[i].tile.Esquerra(0));
            if (!tmpConnexions.Contains(tiles[i].tile.Dreta(0))) tmpConnexions.Add(tiles[i].tile.Dreta(0));
        }

        connexionsPossibles = tmpConnexions.ToArray();
    }


    [System.Serializable]
    public class TilesPossibles
    {
        public Tile tile;
        [Range(1,100)]public int pes;
    }
}

