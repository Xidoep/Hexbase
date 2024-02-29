using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PeçaGuardada
{
    public PeçaGuardada(Peça peça, Grup grup)
    {
        coordenada = peça.Coordenades;
        Save(peça, grup);
    }

    [SerializeField] Vector2Int coordenada;
    [SerializeField] string subestat;
    [SerializeField] string estat;
    //[SerializeField] Estat esta;
    //[SerializeField] Subestat subestat;

    [SerializeField] Vector2Int connexio;
    [SerializeField] ProductesGuardats productes;
    [SerializeField] string grup;
    //[SerializeField] Grup grup;
    //[SerializeField] SavedCasa[] casa;
    [SerializeField] string[] receptes;
    [SerializeField] string[] receptesCasa;
    [SerializeField] string[] necessitats;
    [SerializeField] SavedTile[] tiles;

    public Vector2Int Coordenada => coordenada;

    public void Save(Peça peça, Grup grup)
    {
        estat = peça.EstatNom;
        subestat = peça.SubestatNom;
        connexio = peça.Connexio != null ? peça.Connexio.Coordenades : GridExtensions.CoordenadaNula;
        productes = new ProductesGuardats(peça.ProductesExtrets);
        this.grup = grup.Id;

        receptes = new string[peça.processador.Receptes.Count];
        for (int i = 0; i < peça.processador.Receptes.Count; i++)
        {
            receptes[i] = peça.processador.Receptes[i].name;
        }

        //grup = grups.GrupByPeça(grups.GetGrups, peça);
        if (peça.TeCasa)
        {
            receptesCasa = new string[peça.Cases[0].Receptes.Count];
            for (int i = 0; i < peça.Cases[0].Receptes.Count; i++)
            {
                receptesCasa[i] = peça.Cases[0].Receptes[i].name;
            }

            necessitats = new string[peça.Cases[0].Necessitats.Count];
            for (int i = 0; i < peça.Cases[0].Necessitats.Count; i++)
            {
                necessitats[i] = peça.Cases[0].Necessitats[i].name;
            }
        }

        tiles = new SavedTile[]
        {
            peça.Tiles[0].Save,
            peça.Tiles[1].Save,
            peça.Tiles[2].Save,
            peça.Tiles[3].Save,
            peça.Tiles[4].Save,
            peça.Tiles[5].Save
        };
    }

    public Peça Load(Grid grid, System.Func<string, EstatColocable> estatNomToPrefab, System.Func<string, Estat> subestatNomToPrefab, System.Func<string, Producte> producteNomToPrefab, System.Func<string, Tile> tileNomToPrefab, System.Action<Peça> animacio)
    {
        //BASE
        GameObject tmp = MonoBehaviour.Instantiate(grid.Prefab_Peça, grid.transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(coordenada);

        //PEÇA
        Peça peça = tmp.GetComponent<Peça>();
        peça.Setup(grid, coordenada, estatNomToPrefab.Invoke(estat), subestatNomToPrefab.Invoke(subestat));
        peça.name = $"{peça.EstatNom}({peça.Coordenades})";

        //CREAR TILES
        peça.CrearTilesPotencials();
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Load(peça, tileNomToPrefab);
        }
        peça.CrearTilesFisics();

        //CREAR GRUP (si cal)
        //grups.CrearGrups_FromLoad(grup, peça);

        //CASES
        /*if (casa != null)
        {
            peça.CrearCasa(casa[0].Load(producteNomToPrefab));
        }*/
        //NECESSITATS



        if(necessitats != null && necessitats.Length > 0)
        {
            peça.CrearCasa(new Casa(peça, necessitats, receptesCasa));
        }
        if(receptes != null && receptes.Length > 0)
        {
            peça.processador.NetejarReceptes();
            for (int i = 0; i < receptes.Length; i++)
            {
                peça.processador.AfegirRecepta(Referencies.Instance.GetRecepta(receptes[i]));
            }
        }

        grid.Set(peça);
        //VEINS
        peça.AssignarVeinsTiles(peça.Tiles);

        List<ProducteExtret> producteGuardats = new List<ProducteExtret>();
        for (int i = 0; i < productes.productes.Length; i++)
        {
            producteGuardats.Add(new ProducteExtret(Referencies.Instance.GetProducte(productes.productes[i]), productes.gastats[i]));
        }
        peça.SetProductesExtrets = producteGuardats.ToArray();

        peça.ConnexioCoordenada = connexio;
        //if (extraccio != -Vector2Int.one * 10000) peça.SetExtraccio = extraccio;
        //peça.CrearDetalls();

        //peça.Detalls();
        Debug.Log(peça.name);
        animacio?.Invoke(peça);

        return peça;
    }

   


}

[System.Serializable]
public struct GrupGuardat
{
    public GrupGuardat(Grup grup)
    {
        id = grup.Id;
        poble = grup.EsPoble;

        peces = new Vector2Int[grup.Peces.Count];
        for (int i = 0; i < grup.Peces.Count; i++)
        {
            peces[i] = grup.Peces[i].Coordenades;
        }

        veines = new Vector2Int[grup.Veins.Count];
        for (int i = 0; i < grup.Veins.Count; i++)
        {
            veines[i] = grup.Veins[i].Coordenades;
        }

        if (grup.ConnexionsId != null)
        {
            connexionsId = new string[grup.ConnexionsId.Count];
            for (int i = 0; i < grup.ConnexionsId.Count; i++)
            {
                connexionsId[i] = grup.ConnexionsId[i];
            }
        }
        else connexionsId = new string[0];

        if(grup.Cases != null)
        {
            cases = new Vector2Int[grup.Cases.Count];
            for (int i = 0; i < grup.Cases.Count; i++)
            {
                cases[i] = grup.Cases[i].Coordenades;
            }
        }
        else cases = new Vector2Int[0];

        if (grup.Camins != null)
        {
            camins = new Vector2Int[grup.Camins.Count];
            for (int i = 0; i < grup.Camins.Count; i++)
            {
                camins[i] = grup.Camins[i].Coordenades;
            }
        }
        else camins = new Vector2Int[0];

        if(grup.Ports != null)
        {
            ports = new Vector2Int[grup.Ports.Count];
            for (int i = 0; i < grup.Ports.Count; i++)
            {
                ports[i] = grup.Ports[i].Coordenades;
            }
        }
        else ports = new Vector2Int[0];
    }

    public string id;
    public bool poble;
    public Vector2Int[] peces;
    public Vector2Int[] veines;
    public string[] connexionsId;
    public Vector2Int[] cases;
    public Vector2Int[] camins;
    public Vector2Int[] ports;
}


[System.Serializable]
public struct ProductesGuardats
{
    public ProductesGuardats(ProducteExtret[] productes)
    {
        this.productes = new string[productes.Length];
        this.gastats = new bool[productes.Length];
        for (int i = 0; i < productes.Length; i++)
        {
            this.productes[i] = productes[i].producte.name;
            this.gastats[i] = productes[i].gastat;
        }
    }
    public string[] productes;
    public bool[] gastats;
}