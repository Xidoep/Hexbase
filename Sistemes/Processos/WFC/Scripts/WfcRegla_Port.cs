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


    protected override bool Comprovacio(Peça peça)
    {
        ports = 0;
        portSol = true;
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (tilesAmbPort.Contains(peça.Tiles[i].PossibilitatsVirtuals.Tile(0)))
            {
                ports++;
                for (int v = 0; v < peça.Tiles[i].Veins.Length; v++)
                {
                    if (peça.Tiles[i].Veins[v] == null)
                        continue;

                    if (tilesAmbPort.Contains(peça.Tiles[i].Veins[v].PossibilitatsVirtuals.Tile(0)))
                        return false;
                }
            }
        }

        return ports == Mathf.Clamp(ports, 1, 2);
    }

}
