using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;


public abstract class TileSetBase : ScriptableObject
{
    public abstract void Setup();

    public abstract TilesPossibles[] Tiles(Pe�a pe�a = null);
    public abstract Connexio[] ConnexionsNules(Pe�a pe�a = null);
    public abstract ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a = null);
    public abstract Connexio[] ConnexioinsPossibles(Pe�a pe�a = null);
    public bool TeConnexionsNules(Pe�a pe�a) => ConnexionsNules(pe�a).Length > 0;

    [System.Serializable]
    public class TilesPossibles
    {
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
}
