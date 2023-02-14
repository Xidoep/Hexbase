using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedTile
{
    public SavedTile(Tile tile, int orientacio, int orientacioFisica)
    {
        this.tile = tile;
        this.orientacio = orientacio;
        this.orientacioFisica = orientacioFisica;
    }

    [SerializeField] Tile tile;
    [SerializeField] int orientacio;
    [SerializeField] int orientacioFisica;

    public void Load(Peça peça)
    {
        peça.Tiles[orientacio] = new TilePotencial(peça, orientacio);
        peça.Tiles[orientacio].Escollir(tile, orientacioFisica);
    }
}
