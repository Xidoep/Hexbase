using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Possibilitats
{
    public Possibilitats(Possibilitats possibilitats)
    {
        this.possibilitats = possibilitats.possibilitats;
    }
    public Possibilitats(List<Possibilitat> possibilitats)
    {
        this.possibilitats = possibilitats;
    }
    public Possibilitats(Tile tile, int orientacio, int pes)
    {
        possibilitats = new List<Possibilitat>() { new Possibilitat(tile, orientacio, pes) };
    }

    [SerializeField] List<Possibilitat> possibilitats;

    //INTERN

    //FUNCIONS
    public int Count => possibilitats != null ? possibilitats.Count : 0;
    public Possibilitat Get(int index) => possibilitats[index];
    public void Remove(Possibilitat possibilitat) => possibilitats.Remove(possibilitat);
    public Tile Tile(int index) => possibilitats[index].Tile;
    public int Orietacio(int index) => possibilitats[index].Orientacio;
    public void Add(Tile tile, int orientacio, int pes) => possibilitats.Add(new Possibilitat(tile, orientacio, pes));
    public void Add(Possibilitat possibilitat) 
    {
        if (possibilitats == null) possibilitats = new List<Possibilitat>();
        possibilitats.Add(possibilitat);
    } 
    public bool Contains(Possibilitat possibilitat)
    {
        if (possibilitats == null)
            return false;

        bool trobat = false;
        for (int i = 0; i < possibilitats.Count; i++)
        {
            if (possibilitats[i].EqualsTo(possibilitat))
            {
                trobat = true;
                break;
            }
        }
        return trobat;
    }
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


    public int GetPes(int index) => pes + Mathf.RoundToInt(Mathf.PerlinNoise(
        index * 0.4f + Mathf.Sin(Time.time),
        index * 0.4f + Mathf.Cos(Time.time)));
    public int GetPes(int index, int colisions) => pes + Mathf.RoundToInt(Mathf.PerlinNoise(
        (index + colisions) * 0.4f + Mathf.Sin(Time.time), 
        (index + colisions) * 0.4f + Mathf.Sin(Time.time))
        * colisions - (colisions / 2f));
    
    
    
    /*
    public int GetPes(int index, int colisions) => pes + Mathf.RoundToInt(Mathf.PerlinNoise(
        index * 0.3f + Mathf.Sin(Time.time),
        index * 0.3f + Mathf.Sin(Time.time))
        * 2 - 1);
    */
    public bool EqualsTo(Possibilitat possibilitat) => possibilitat.tile == tile && possibilitat.orientacio == orientacio;

}