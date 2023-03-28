using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedPeça
{
    public SavedPeça(Peça peça, Grups grups)
    {
        coordenada = peça.Coordenades;
        estat = peça.EstatNom;
        subestat = peça.SubestatNom;
        extraccio = peça.GetExtraccio != null ? peça.GetExtraccio.Coordenades : GridExtensions.CoordenadaNula;
        grup = grups.GrupByPeça(grups.Grup, peça);

        if (peça.TeCasa)
        {
            necessitats = new SavedNecessitat[] { new SavedNecessitat(peça.Casa.Necessitats) };
            //casa = new SavedCasa[] { new SavedCasa(peça.Casa.Necessitats) };
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

    [SerializeField] string subestat;
    [SerializeField] string estat;
    [SerializeField] Vector2Int coordenada;
    //[SerializeField] Estat esta;
    //[SerializeField] Subestat subestat;

    [SerializeField] Vector2Int extraccio;
    [SerializeField] Grup grup;
    //[SerializeField] SavedCasa[] casa;
    [SerializeField] SavedNecessitat[] necessitats;
    [SerializeField] SavedTile[] tiles;

    public Vector2Int Coordenada => coordenada;

    public Peça Load(Grid grid, Grups grups, System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab, System.Func<string, Producte> producteNomToPrefab, System.Func<string, Tile> tileNomToPrefab, System.Action<Peça> animacio)
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

        //CREAR GRUP si cal
        grups.CrearGrups_FromLoad(grup, peça);

        //CASES
        /*if (casa != null)
        {
            peça.CrearCasa(casa[0].Load(producteNomToPrefab));
        }*/
        //NECESSITATS
        if(necessitats != null && necessitats.Length > 0)
        {
            peça.CrearCasa(new Casa(necessitats[0].Load(producteNomToPrefab)));
        }

        grid.Set(peça);
        //VEINS
        peça.AssignarVeinsTiles(peça.Tiles);

        if (extraccio != -Vector2Int.one) peça.SetExtraccio = extraccio;
        //peça.CrearDetalls();

        //peça.Detalls();
        Debug.Log(peça.name);
        animacio?.Invoke(peça);

        return peça;
    }
}
