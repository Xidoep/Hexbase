using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedTile
{
    public SavedTile(Tile tile, int orientacio, int orientacioFisica)
    {
        this.tile = tile.name;
        this.orientacio = orientacio;
        this.orientacioFisica = orientacioFisica;
    }

    [SerializeField] string tile;
    [SerializeField] int orientacio;
    [SerializeField] int orientacioFisica;


    public void Load(Peça peça, System.Func<string, Tile> tileNomToPrefab)
    {
        peça.Tiles[orientacio] = new TilePotencial(peça, orientacio);
        peça.Tiles[orientacio].Escollir(tileNomToPrefab.Invoke(tile), orientacioFisica);
    }
}
