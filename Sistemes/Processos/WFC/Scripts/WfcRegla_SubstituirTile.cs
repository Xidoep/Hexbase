using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC Regles/SubstituirTile")]
public class WfcRegla_SubstituirTile : WfcRegla
{
    [SerializeField] Subestat objectiu;

    [Linia]
    [SerializeField] Substitucio[] substitucions;
    [Space(10)]
    [SerializeField] bool evitarVeinsIguals;

    //INTERN
    bool substituit;
    List<TilePotencial> tiles;



    public override Subestat Subestat => objectiu;

    protected override bool Comprovacio(Peça peça)
    {
        substituit = false;

        tiles = new List<TilePotencial>(peça.Tiles);
        for (int t = 0; t < tiles.Count; t++)
        {
            for (int s = 0; s < substitucions.Length; s++)
            {
                if (tiles[t].PossibilitatsVirtuals.Get(0).Tile == substitucions[s].substitucio)
                {
                    substituit = true;
                    break;
                }
            }
            if (substituit)
                break;
        }

        if (substituit)
            return true;



        for (int t = 0; t < tiles.Count; t++)
        {
            for (int s = 0; s < substitucions.Length; s++)
            {
                if(TileIgualABuscat(t, s))
                {
                    if (evitarVeinsIguals)
                    {
                        for (int v = 0; v < tiles[t].Veins.Length; v++)
                        {
                            if (TileVeiIgualASubstitucio(t, v, s))
                                continue;
                        }
                    }

                    Substituir(t, s);

                    substituit = true;
                    break;
                }
            }
            if (substituit)
                break;

        }

        return substituit;
    }


    bool TileIgualABuscat(int t, int s) => tiles[t].PossibilitatsVirtuals.Get(0).Tile == substitucions[s].buscat;
    bool TileVeiIgualASubstitucio(int t, int v, int s) => tiles[t].Veins[v] != null && tiles[t].Veins[v].PossibilitatsVirtuals.Get(0).Tile == substitucions[s].substitucio;
    void Substituir(int t, int s) => tiles[t].Escollir(substitucions[s].substitucio, tiles[t].PossibilitatsVirtuals.Get(0).Orientacio);


    [System.Serializable] protected class Substitucio
    {
        public Tile buscat;
        public Tile substitucio;
        
    }
}
