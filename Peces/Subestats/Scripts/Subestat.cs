using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Subestat : ScriptableObject
{
    public virtual Subestat Setup(Peça peça) => this;

    [SerializeField] int punts;
    [Linia]
    [Tooltip("Que s'ha de crear un cami cap a ell")]
    [SerializeField] bool caminable;
    [SerializeField] bool aquatic;
    public bool Caminable => caminable;
    public bool Aquatic => aquatic;

    public virtual bool EsProducte => false;



    [Apartat("CONDICIONS")]
    [SerializeField] Condicio[] condicions;
    [SerializeField] DetallScriptable[] detallsScriptables;

    public Condicio[] Condicions => condicions;
    public DetallScriptable[] Detalls => detallsScriptables;





    [Apartat("TILES")]
    [SerializeField] TilesPossibles[] tiles;
    [SerializeField] Connexio[] connexionsNules;
    [SerializeField] ConnexioEspesifica connexioEspesifica;

    protected TilesPossibles[] Tiles => tiles;
    public bool TeConnexionsNules => connexionsNules.Length > 0;
    public virtual Connexio[] ConnexionsNules => connexionsNules;
    public ConnexioEspesifica ConnexionsEspesifica => connexioEspesifica;





    [Apartat("PRODUCTE")]
    [SerializeField] Producte producte;
    [SerializeField] EstrategiaDeProduccio estrategia;

    public Producte Producte => producte;
    public Producte[] Produccio() => estrategia.Produir(producte);
    public EstrategiaDeProduccio Estrategia => estrategia;





    [Apartat("INFORMACIO")]
    [SerializeField] Informacio[] informacions;





    [Tooltip("S'emplena automaticament")] 
    [SerializeField] protected Connexio[] connexionsPossibles;
    public Connexio[] ConnexionsPossibles => connexionsPossibles;





    public void InformacioMostrar(Peça peça, bool proveides) 
    {
        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Mostrar(peça, proveides);
        }

        
    }
    public void InformacioAmagar(Peça peça) 
    {
        if (informacions.Length == 0)
        {

            return;
        }

        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Amagar(peça);
        }
    } 





    public Possibilitats Possibilitats()
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






    protected void OnValidate()
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



    [System.Serializable]
    public class TilesPossibles
    {
        public Tile tile;
        [Range(1, 10)] public int pes;
    }

    [System.Serializable]
    public class ConnexioEspesifica
    {
        public List<Subestat> subestats;
        public Connexio[] connexions;
    }
}