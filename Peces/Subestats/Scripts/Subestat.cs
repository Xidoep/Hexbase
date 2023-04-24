using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Subestat : ScriptableObject
{
    public virtual Subestat Setup(Peça peça) 
    {
        peça.ResetCases();

        peça.gameObject.name = $"{this.name.ToUpper()}({peça.Coordenades})";

        peça.processador.NovesReceptes(receptas, peça.CanviarSubestat);

        return this;
    } 

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

    public Recepta[] receptas;



    [Apartat("TILES")]
    [SerializeScriptableObject] [SerializeField] TileSetBase tileset;

    public bool TeConnexionsNules(Peça peça) => tileset.ConnexionsNules(peça).Length > 0;
    public virtual Connexio[] ConnexionsNules(Peça peça) => tileset.ConnexionsNules(peça);
    public TileSetBase.ConnexioEspesifica ConnexionsEspesifica(Peça peça) => tileset.ConnexionsEspesifica(peça);
    public Connexio[] ConnexionsPossibles(Peça peça) => tileset.ConnexioinsPossibles(peça);
    





    [Apartat("PRODUCTE")]
    [SerializeField] Producte producte;
    [SerializeField] EstrategiaDeProduccio estrategia;

    public Producte Producte => producte;
    public Producte[] Produccio() => estrategia.Produir(producte);
    public EstrategiaDeProduccio Estrategia => estrategia;





    [Apartat("INFORMACIO")]
    [SerializeField] Informacio[] informacions;









    public void InformacioMostrar(Hexagon peça, bool proveides) 
    {
        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Mostrar(peça, proveides);
        }

        
    }
    public void InformacioAmagar(Hexagon peça) 
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





    public Possibilitats Possibilitats(Peça peça)
    {
        Possibilitats ps = new Possibilitats(new List<Possibilitat>());
        TileSetBase.TilesPossibles[] tiles = tileset.Tiles(peça);
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



    /*[System.Serializable]
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
    }*/
}