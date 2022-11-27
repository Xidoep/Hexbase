using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC Regles/Tiles MinMax")]
public class WfcRegla_TilesMinMax : WfcRegla
{
    [SerializeField] Subestat objectiu;

    [SerializeField] List<Tile> tilesAcceptats;
    [Tooltip("Es la quantitat de Tiles que pot deixar sense cohincidir")] [SerializeField] [Range(0, 6)] int minim;
    [Tooltip("Es la quantitat de Tiles que pot deixar sense cohincidir")] [SerializeField] [Range(0, 6)] int maxim;
    [SerializeField] bool permetreVeins;
    //Intern
    bool complert;
    int encerts;
    List<TilePotencial> tiles;
    public override Subestat Subestat => objectiu;

    protected override bool Comprovacio(Peça peça)
    {
        complert = false;
        encerts = 0;

        tiles = new List<TilePotencial>(peça.Tiles);
        Debug.Log("hola");
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tilesAcceptats.Contains(tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                encerts++;
        }

        if (permetreVeins)
        {
            if (encerts == 0)
            {
                List<TilePotencial> veins = new List<TilePotencial>();
                for (int i = 0; i < tiles.Count; i++)
                {
                    if(tiles[i].Veins[0] != null)
                        veins.Add(tiles[i].Veins[0]);
                }

                for (int i = 0; i < veins.Count; i++)
                {
                    if (tilesAcceptats.Contains(veins[i].PossibilitatsVirtuals.Get(0).Tile))
                        encerts++;
                }
            }
        }
       
        

        if (encerts == Mathf.Clamp(encerts, minim, maxim)) complert = true;
        //if (encerts != comprovats && encerts < m<axim) complert = false;
        Debug.Log($"Complert = {complert}");

        return complert;
    }
}
