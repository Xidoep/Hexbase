using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Possibilitats
{
    public Possibilitats(List<Possibilitat> possibilitats)
    {
        this.possibilitats = possibilitats;
        //contains = false;
        //index = 0;
    }
    public Possibilitats(Tile tile, int orientacio, int pes)
    {
        possibilitats = new List<Possibilitat>() { new Possibilitat(tile, orientacio, pes) };
        //contains = false;
        //index = 0;
    }
 //   public Possibilitats(Tile tile, int orientacio, int rMin, int rMax)
  //  {
   //     possibilitats = new List<Possibilitat>() { new Possibilitat(tile, orientacio, rMin, rMax) };
        //contains = false;
        //index = 0;
  //  }

    /// <summary>
    /// Utilitzat principalment per quan es tria el PREFERIT. I es donen dos mes opcios, perque es torni a comprovar la eleccio i no es resolgui al tenir un sol resultat.
    /// </summary>
    //public Possibilitats(Tile tile1, Tile tile2) //NOT USED
   // {
   //     possibilitats = new List<Possibilitat>();
        //possibilitats.Add(new Possibilitat(tile1, 0));
        //possibilitats.Add(new Possibilitat(tile2, 0));
        //contains = false;
        //index = 0;
   // }
    //public Possibilitats(Tile tile1) //NOT USED
    //{
    //    possibilitats = new List<Possibilitat>();
        /*possibilitats.Add(new Possibilitat(tile1, 0));
        possibilitats.Add(new Possibilitat(tile1, 1));
        possibilitats.Add(new Possibilitat(tile1, 2));*/
        //contains = false;
        //index = 0;
    //}

    [SerializeField] List<Possibilitat> possibilitats;

    //INTERN
    //bool contains;
    //int index;


    //FUNCIONS
    public int Count => possibilitats.Count;
    public Possibilitat Get(int index) => possibilitats[index];
    public void Remove(Possibilitat possibilitat) => possibilitats.Remove(possibilitat);
    public Tile Tile(int index) => possibilitats[index].Tile;
    public int Orietacio(int index) => possibilitats[index].Orientacio;
    public void Add(Tile tile, int orientacio, int pes) => possibilitats.Add(new Possibilitat(tile, orientacio, pes));
    /*public void Add(Tile tile, int orientacio, int rMin, int rMax) => possibilitats.Add(new Possibilitat(tile, orientacio, rMin, rMax));
    public void Replace(int index, int orientacio, int rMin, int rMax) => possibilitats[index] = new Possibilitat(Tile(index), orientacio, rMin, rMax);
    public bool Contains(Tile tile)
    {
        contains = false;
        for (int i = 0; i < possibilitats.Count; i++)
        {
            if (possibilitats[i].EsIgual(tile)) contains = true;
        }
        return contains;
    }
    public int IndexOf(Tile tile)
    {
        index = 0;
        for (int i = 0; i < possibilitats.Count; i++)
        {
            if (possibilitats[i].EsIgual(tile))
            {
                index = i;
                break;
            }
        }
        return index;
    }
    public void SetRandomRange(int index, int rMin, int rMax) => possibilitats[index].Random(new Vector2Int(rMin, rMax));

    public int Random(int random)
    {
        int index = -1;
        Debug.LogError($"GIVEN RANDOM NUMBER = {random}");
        for (int i = 0; i < possibilitats.Count; i++)
        {
            Debug.LogError($"P{i} RANDOM {possibilitats[i].random}");
            if (possibilitats[i].InRandomRange(random)) index = i;
        }
        Debug.LogError($"RANDOM = {index}");
        return index;
    }
    public Tile[] ToArray()
    {
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < possibilitats.Count; i++)
        {
            tiles.Add(possibilitats[i].Tile);
        }
        return tiles.ToArray();
    }*/
}
[System.Serializable]
public struct Possibilitat
{

    public Possibilitat(Tile tile, int orientacio, int pes)
    {
        this.tile = tile;
        this.orientacio = orientacio;
        this.pes = pes;
        //random = new Vector2Int();
    }
    /*public Possibilitat(Tile tile, int orientacio, int rMin, int rMax)
    {
        this.tile = tile;
        this.orientacio = orientacio;
        this.pes = rMax - rMin;
        //random = new Vector2Int(rMin, rMax);
    }*/
    [SerializeField] Tile tile;
    [Range(0,2)][SerializeField] int orientacio;
    int pes;

    public Tile Tile => tile;
    public int Orientacio => orientacio;
    public int Pes => pes;


    //public void Random(Vector2Int random) => this.random = random;

    //public bool EsIgual(Tile tile) => this.tile == tile;
    //public bool InRandomRange(int r) => r == Mathf.Clamp(r, random.x, random.y);
    public bool EqualsTo(Possibilitat possibilitat) => possibilitat.tile == tile && (possibilitat.orientacio == orientacio);
    //public bool EqualsTo(Tile tile, int orientacio) => tile == this.tile && orientacio == this.orientacio;
}







