using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Subestat : ScriptableObject, IProcessable
{
    public enum Tipus { Normal, Productor}
    public virtual Subestat Setup(Peça peça) 
    {
        peça.ResetCases();

        peça.gameObject.name = $"{this.name.ToUpper()}({peça.Coordenades})";

        if (tipus == Tipus.Productor)
            produccio.AddProductor(peça);
        else produccio.RemoveProductor(peça);

        peça.processador.NovesReceptes(Receptes);

        return this;
    }

    [SerializeField] Tipus tipus;
    [SerializeField] bool caminable;
    [SerializeField] bool aquatic;

    [Apartat("CONDICIONS")]
    [SerializeField] Recepta[] receptes;
    //[SerializeField] Condicio[] condicions;
    [SerializeField] DetallScriptable[] detallsScriptables;

    [Apartat("TILES")]
    [SerializeScriptableObject] [SerializeField] TileSetBase tileset;

    [Apartat("INFORMACIO")]
    [SerializeScriptableObject] [SerializeField] Informacio[] informacions;

    [Apartat("REFERENCIES AUTO-CONFIGURABLES")]
    [SerializeField] Produccio produccio;


    public bool EsProductor => tipus == Tipus.Productor;
    public bool Caminable => caminable;
    public bool Aquatic => aquatic;
    //public virtual bool EsProducte => false;



    public Recepta[] Receptes => receptes;
    //public Condicio[] Condicions => condicions;
    public DetallScriptable[] Detalls => detallsScriptables;



    public bool TeConnexionsNules(Peça peça) => tileset.ConnexionsNules(peça).Length > 0;
    public virtual Connexio[] ConnexionsNules(Peça peça) => tileset.ConnexionsNules(peça);
    public TileSetBase.ConnexioEspesifica ConnexionsEspesifica(Peça peça) => tileset.ConnexionsEspesifica(peça);
    public Connexio[] ConnexionsPossibles(Peça peça) => tileset.ConnexioinsPossibles(peça);
    














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

    public void Processar(Peça peça)
    {
        Debug.Log($"PROCESSAR SUBESTAT {this.name}");
        peça.CanviarSubestat(this);
    }








    void OnValidate()
    {
        if (produccio == null) produccio = XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
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