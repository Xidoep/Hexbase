using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Estat : ScriptableObject, IProcessable
{
    public void SetupCreacio(TipusEnum tipus, bool aquatic, Producte producte)
    {
        this.tipus = tipus;
        this.aquatic = aquatic;
        this.producte = producte;
        informacions = new List<Informacio>();
        SetupInformacio();
    }
    public enum TipusEnum 
    {
        [LabelText(" ", SdfIconType.DashLg)] Normal,
        [LabelText(" ", SdfIconType.HouseFill)] Casa,
        [LabelText(" ", SdfIconType.Shop)] Productor,
        [LabelText(" ", SdfIconType.MinecartLoaded)] Extraccio
    }
    public virtual Estat Setup(Peça peça) 
    {
        peça.ResetCases();

        peça.gameObject.name = $"{this.name.ToUpper()}({peça.Coordenades})";
        
        peça.processador.NovaRecepta(Receptes);

        switch (tipus)
        {
            case TipusEnum.Normal:
                produccio.RemoveProductor(peça);
                break;
            case TipusEnum.Casa:
                produccio.RemoveProductor(peça);
                if (!peça.TeCasa)
                    repoblar.AfegirLaPrimeraCasa(peça);
                break;
            case TipusEnum.Productor:
                produccio.AddProductor(peça);
                break;
            case TipusEnum.Extraccio:
                produccio.RemoveProductor(peça);
                break;
            default:
                break;
        }


        return this;
    }

    [TabGroup("Propietats"), EnumToggleButtons ,SerializeField] 
    TipusEnum tipus;
    [TabGroup("Propietats"),SerializeField] 
    bool caminable;
    [TabGroup("Propietats"),SerializeField] 
    bool aquatic;
    [TabGroup("Propietats"),ShowIf("@this.tipus == TipusEnum.Productor || this.tipus == TipusEnum.Extraccio"),SerializeField] 
    Producte producte;

    //[Apartat("CONDICIONS")]
    [TabGroup("Condicions")]
    [SerializeField] Recepta[] receptes;
    //[SerializeField] Condicio[] condicions;
    [Space(10)]
    [TabGroup("Condicions")]
    [SerializeField] DetallScriptable[] detallsScriptables;

    //[Apartat("TILES")]
    [TabGroup("Tiles"), SerializeScriptableObject, SerializeField, InlineEditor] 
    TileSetBase tileset;

    //[Apartat("INFORMACIO")]
    [TabGroup("Informacio")]
    [SerializeScriptableObject] [SerializeField] List<Informacio> informacions;

    [HideInInspector] [SerializeField] Produccio produccio;
    [HideInInspector] [SerializeField] Repoblar repoblar;

    public TipusEnum Tipus => tipus;
    public bool Caminable => caminable;
    public bool Aquatic => aquatic;
    public Producte Producte => producte;


    public Recepta[] Receptes => receptes;
    public DetallScriptable[] Detalls => detallsScriptables;



    public bool TeConnexionsNules(Peça peça) => tileset.ConnexionsNules(peça).Length > 0;
    public virtual Connexio[] ConnexionsNules(Peça peça) => tileset.ConnexionsNules(peça);
    public ConnexioEspesifica[] ConnexionsEspesifiques(Peça peça) => tileset.ConnexionsEspesifiques(peça);
    public Connexio[] ConnexionsPossibles(Peça peça) => tileset.ConnexioinsPossibles(peça);
    public TileSetBase SetTileset { set => tileset = value; }


    public void AddRecepta(Recepta recepta)
    {
        List<Recepta> tmp = new List<Recepta>(receptes);

        if (tmp.Contains(recepta))
            return;

        tmp.Add(recepta);
        receptes = tmp.ToArray();
    }
    public void RemoveRecepta(Recepta recepta)
    {
        List<Recepta> tmp = new List<Recepta>(receptes);
        tmp.Remove(recepta);
        receptes = tmp.ToArray();
    }


    public void InformacioMostrar(Hexagon peça) 
    {
        for (int i = 0; i < informacions.Count; i++)
        {
            informacions[i].Mostrar(peça);
        }
    }
    public void InformacioAmagar(Hexagon peça) 
    {
        if (informacions.Count == 0)
            return;

        for (int i = 0; i < informacions.Count; i++)
        {
            informacions[i].Amagar(peça);
        }
    } 





    public Possibilitats Possibilitats(Peça peça)
    {
        Possibilitats ps = new Possibilitats(new List<Possibilitat>());
        TilesPossibles[] tiles = tileset.Tiles(peça);
        for (int i = 0; i < tiles.Length; i++)
        {
            //if (!tiles[i].tile.ConnexionsIguals)
            //{
                ps.Add(tiles[i].tile, 0, tiles[i].pes);
                ps.Add(tiles[i].tile, 1, tiles[i].pes);
                ps.Add(tiles[i].tile, 2, tiles[i].pes);
            //}
            //else ps.Add(tiles[i].tile, 0, tiles[i].pes * 3);
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
        if (repoblar == null) repoblar = XS_Editor.LoadAssetAtPath<Repoblar>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Repoblar.asset");

        if (tileset == null) tileset = XS_Editor.LoadAssetAtPath<TileSetBase>($"Assets/XidoStudio/Hexbase/Peces/Estats/{name}/Tiles/{name}.asset");

        if (tipus == TipusEnum.Normal)
            return;
        SetupInformacio();

    }

    void SetupInformacio()
    {
        Informacio casa = XS_Editor.LoadAssetAtPath<Informacio>("Assets/XidoStudio/Hexbase/Peces/Informacio/Casa.asset");
        Informacio connexio = XS_Editor.LoadAssetAtPath<Informacio>("Assets/XidoStudio/Hexbase/Peces/Informacio/Connexio.asset");
        Informacio extraccio = XS_Editor.LoadAssetAtPath<Informacio>("Assets/XidoStudio/Hexbase/Peces/Informacio/Extraccio.asset");
        Informacio grup = XS_Editor.LoadAssetAtPath<Informacio>("Assets/XidoStudio/Hexbase/Peces/Informacio/Grup.asset");
        Informacio productor = XS_Editor.LoadAssetAtPath<Informacio>("Assets/XidoStudio/Hexbase/Peces/Informacio/Productor.asset");

        switch (tipus)
        {
            case TipusEnum.Casa:
                if (!informacions.Contains(casa) || !informacions.Contains(grup))
                {
                    informacions = new List<Informacio>();
                    informacions.Add(casa);
                    informacions.Add(grup);
                }
                break;
            case TipusEnum.Productor:
                if (!informacions.Contains(grup) || !informacions.Contains(connexio) || !informacions.Contains(productor))
                {
                    informacions = new List<Informacio>();
                    informacions.Add(grup);
                    informacions.Add(connexio);
                    informacions.Add(productor);
                }
                break;
            case TipusEnum.Extraccio:
                if (!informacions.Contains(connexio) || !informacions.Contains(extraccio))
                {
                    informacions = new List<Informacio>();
                    informacions.Add(connexio);
                    informacions.Add(extraccio);
                }
                break;
        }
    }
}