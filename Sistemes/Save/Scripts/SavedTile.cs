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

    public void Load(Pe�a pe�a)
    {
        pe�a.Tiles[orientacio] = new TilePotencial(pe�a, orientacio);
        pe�a.Tiles[orientacio].Escollir(tile, orientacioFisica);
    }
}
