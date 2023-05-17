using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC Regles/Riu seguit")]
public class WfcRegla_RiuSeguit : WfcRegla
{
    [SerializeField] Subestat objectiu;

    [SerializeField] bool aquatic;
    [SerializeField] List<Possibilitat> tilesAcceptats;
    [SerializeField] List<Possibilitat> tilesConnexio;


    [Tooltip("Es la quantitat de Tiles que pot deixar sense cohincidir")]
    [SerializeField] [Range(0,5)] int minimsEncerts;
    //Intern
    bool complert;
    bool acceptat;
    int encerts;
    int comprovats;
    int ir;
    List<TilePotencial> tiles;
    public override Subestat Subestat => objectiu;
    bool totConnectat;

    protected override bool Comprovacio(Peça peça)
    {
        totConnectat = true;
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (peça.Tiles[i].Veins[0] == null || !peça.Tiles[i].Veins[0].Peça.EsAquatic)
                continue;

            acceptat = false;
            for (int p = 0; p < tilesConnexio.Count; p++)
            {
                if (peça.Tiles[i].PossibilitatsVirtuals.Get(0).EqualsTo(tilesConnexio[p]))
                {
                    Debugar.Log($"Tile[{i}] ES UN DELS PERMESSOS!");
                    acceptat = true;
                    break;
                }
            }
            if (!acceptat)
            {
                totConnectat = false;
                break;
            }
        }

        return totConnectat;


        complert = true;
        encerts = 0;
        comprovats = 0;
        tiles = new List<TilePotencial>(peça.Tiles);
        Debugar.Log("hola");
        for (int i = Random.Range(0, tiles.Count); tiles.Count > 0; i = Random.Range(0,tiles.Count))
        {
            if (tiles[i].Veins[0] == null)
            {
                tiles.RemoveAt(i);
                Debugar.Log($"Tile[{i}] no te vei");
                continue;
            }

            if (!tiles[i].Veins[0].Peça.EsAquatic)
            {
                Debugar.Log($"Tile[{i}] no te vei aquatic");
                tiles.RemoveAt(i);
                continue;
            }

            for (int p = 0; p < tilesAcceptats.Count; p++)
            {
                Debugar.Log($"Tile[{i}] es igual a {tilesAcceptats[p].Tile.name}???");
                acceptat = false;
                if (tiles[i].PossibilitatsVirtuals.Get(0).EqualsTo(tilesAcceptats[p]))
                {
                    Debugar.Log($"Tile[{i}] ES UN DELS PERMESSOS!");
                    acceptat = true;
                    encerts++;
                    break;
                }
            }

            tiles.RemoveAt(i);

            comprovats++;
        }
        /*if (encerts == comprovats) complert = true;
        else
        {
            if (encerts >= minimsEncerts) complert = true;
            else complert = false;
        }*/

        if (encerts != comprovats && encerts < minimsEncerts) complert = false;
        Debugar.Log($"Complert = {complert}");

        return complert;
    }
}
