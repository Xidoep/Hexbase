using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC Regles/Riu seguit")]
public class WfcRegla_RiuSeguit : WfcRegla
{
    [SerializeField] Subestat riu;

    [SerializeField] List<Possibilitat> tilesAcceptats;
    [Tooltip("Es la quantitat de Tiles que pot deixar sense cohincidir")][SerializeField] [Range(0,5)] int minimsEncerts;

    //Intern
    bool complert;
    int encerts;
    int comprovats;
    List<TilePotencial> tiles;

    public override Subestat Subestat => riu;



    protected override bool Comprovacio(Peça peça)
    {
        complert = true;
        encerts = 0;
        comprovats = 0;
        tiles = new List<TilePotencial>(peça.Tiles);
        Debug.Log("hola");
        for (int i = Random.Range(0, tiles.Count); tiles.Count > 0; i = Random.Range(0,tiles.Count))
        {
            if (tiles[i].Veins[0] == null)
            {
                tiles.RemoveAt(i);
                Debug.Log($"Tile[{i}] no te vei");
                continue;
            }

            if (!tiles[i].Veins[0].Peça.Subestat.Aquatic)
            {
                Debug.Log($"Tile[{i}] no te vei aquatic");
                tiles.RemoveAt(i);
                continue;
            }

            for (int p = 0; p < tilesAcceptats.Count; p++)
            {
                Debug.Log($"Tile[{i}] es igual a {tilesAcceptats[p].Tile.name}???");
                if (tiles[i].PossibilitatsVirtuals.Get(0).EqualsTo(tilesAcceptats[p]))
                {
                    Debug.Log($"Tile[{i}] ES UN DELS PERMESSOS!");
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
        Debug.Log($"Complert = {complert}");

        return complert;
    }
}
