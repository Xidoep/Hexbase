using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Subestat : ScriptableObject, IProcessable
{
    public enum TipusEnum { Normal, Casa, Productor, Extraccio}
    public virtual Subestat Setup(Pe�a pe�a) 
    {
        pe�a.ResetCases();

        pe�a.gameObject.name = $"{this.name.ToUpper()}({pe�a.Coordenades})";
        
        pe�a.processador.NovaRecepta(Receptes);

        switch (tipus)
        {
            case TipusEnum.Normal:
                produccio.RemoveProductor(pe�a);
                break;
            case TipusEnum.Casa:
                produccio.RemoveProductor(pe�a);
                if (!pe�a.TeCasa)
                    repoblar.AfegirLaPrimeraCasa(pe�a);
                break;
            case TipusEnum.Productor:
                produccio.AddProductor(pe�a);
                break;
            case TipusEnum.Extraccio:
                produccio.RemoveProductor(pe�a);
                break;
            default:
                break;
        }


        return this;
    }

    [SerializeField] TipusEnum tipus;
    [SerializeField] bool caminable;
    [SerializeField] bool aquatic;
    [SerializeField] Producte producte;

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

    public TipusEnum Tipus => tipus;
    public bool Caminable => caminable;
    public bool Aquatic => aquatic;
    public Producte Producte => producte;


    public Recepta[] Receptes => receptes;
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
            return;

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

}