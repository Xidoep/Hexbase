using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Subestat : ScriptableObject, IProcessable
{
    public enum Tipus { Normal, Casa, Productor}
    public virtual Subestat Setup(Pe�a pe�a) 
    {
        pe�a.ResetCases();

        pe�a.gameObject.name = $"{this.name.ToUpper()}({pe�a.Coordenades})";

        switch (tipus)
        {
            case Tipus.Normal:
                produccio.RemoveProductor(pe�a);
                break;
            case Tipus.Casa:
                produccio.RemoveProductor(pe�a);
                if (!pe�a.TeCasa)
                    repoblar.AfegirLaPrimeraCasa(pe�a);
                break;
            case Tipus.Productor:
                produccio.AddProductor(pe�a);
                break;
            default:
                break;
        }

        pe�a.processador.NovesReceptes(Receptes);

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
    [SerializeField] Repoblar repoblar;

    public bool EsProductor => tipus == Tipus.Productor;
    public bool Caminable => caminable;
    public bool Aquatic => aquatic;
    //public virtual bool EsProducte => false;



    public Recepta[] Receptes => receptes;
    //public Condicio[] Condicions => condicions;
    public DetallScriptable[] Detalls => detallsScriptables;



    public bool TeConnexionsNules(Pe�a pe�a) => tileset.ConnexionsNules(pe�a).Length > 0;
    public virtual Connexio[] ConnexionsNules(Pe�a pe�a) => tileset.ConnexionsNules(pe�a);
    public TileSetBase.ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a) => tileset.ConnexionsEspesifica(pe�a);
    public Connexio[] ConnexionsPossibles(Pe�a pe�a) => tileset.ConnexioinsPossibles(pe�a);
    














    public void InformacioMostrar(Hexagon pe�a, bool proveides) 
    {
        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Mostrar(pe�a, proveides);
        }

        
    }
    public void InformacioAmagar(Hexagon pe�a) 
    {
        if (informacions.Length == 0)
        {

            return;
        }

        for (int i = 0; i < informacions.Length; i++)
        {
            informacions[i].Amagar(pe�a);
        }
    } 





    public Possibilitats Possibilitats(Pe�a pe�a)
    {
        Possibilitats ps = new Possibilitats(new List<Possibilitat>());
        TileSetBase.TilesPossibles[] tiles = tileset.Tiles(pe�a);
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

    public void Processar(Pe�a pe�a)
    {
        Debug.Log($"PROCESSAR SUBESTAT {this.name}");
        pe�a.CanviarSubestat(this);
    }








    void OnValidate()
    {
        if (produccio == null) produccio = XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
        if (repoblar == null) repoblar = XS_Editor.LoadAssetAtPath<Repoblar>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Repoblar.asset");
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