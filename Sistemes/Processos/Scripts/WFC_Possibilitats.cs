using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Possibilitats
{
    public Possibilitats(List<Possibilitat> possibilitats)
    {
        this.possibilitats = possibilitats;
    }
    public Possibilitats(Tile tile, int orientacio, int pes)
    {
        possibilitats = new List<Possibilitat>() { new Possibilitat(tile, orientacio, pes) };
    }

    [SerializeField] List<Possibilitat> possibilitats;

    //FUNCIONS
    public int Count => possibilitats.Count;
    public Possibilitat Get(int index) => possibilitats[index];
    public void Remove(Possibilitat possibilitat) => possibilitats.Remove(possibilitat);
    public Tile Tile(int index) => possibilitats[index].Tile;
    public int Orietacio(int index) => possibilitats[index].Orientacio;
    public void Add(Tile tile, int orientacio, int pes) => possibilitats.Add(new Possibilitat(tile, orientacio, pes));
}
[System.Serializable]
public struct Possibilitat
{

    public Possibilitat(Tile tile, int orientacio, int pes)
    {
        this.tile = tile;
        this.orientacio = orientacio;
        this.pes = pes;
    }

    [SerializeField] Tile tile;
    [Range(0,2)][SerializeField] int orientacio;
    int pes;

    public Tile Tile => tile;
    public int Orientacio => orientacio;
    public int Pes => pes;

    public bool EqualsTo(Possibilitat possibilitat) => possibilitat.tile == tile && possibilitat.orientacio == orientacio;

}







