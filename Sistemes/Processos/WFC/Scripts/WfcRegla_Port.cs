using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC Regles/Ports entre 1 i 2")]
public class WfcRegla_Port : WfcRegla
{
    [SerializeField] Subestat port;

    [SerializeField] List<Tile> tilesAmbPort;

    //INTERN
    int ports;
    bool portSol;

    public override Subestat Subestat => port;


    protected override bool Comprovacio(Pe�a pe�a)
    {
        ports = 0;
        portSol = true;
        for (int i = 0; i < pe�a.Tiles.Length; i++)
        {
            if (tilesAmbPort.Contains(pe�a.Tiles[i].PossibilitatsVirtuals.Tile(0)))
            {
                ports++;
                for (int v = 0; v < pe�a.Tiles[i].Veins.Length; v++)
                {
                    if (pe�a.Tiles[i].Veins[v] == null)
                        continue;

                    if (tilesAmbPort.Contains(pe�a.Tiles[i].Veins[v].PossibilitatsVirtuals.Tile(0)))
                        return false;
                }
            }
        }

        return ports == Mathf.Clamp(ports, 1, 2);
    }

}
