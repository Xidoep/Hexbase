using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//**********************************************
//FALTA: treute estat i subestat d'aqui i passarlo a la pe�a.
//**********************************************

/// <summary>
/// La classe base de les peces del Grid
/// </summary>
[SelectionBase]
public abstract class Hexagon : MonoBehaviour
{
    public virtual void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat = null)
    {
        veins = grid.Veins;
        veinsPe�a = grid.VeinsPe�a;
        this.coordenades = coordenades;
    }

    //VARIABLES 
    [SerializeField] Transform parent;
    [SerializeField] protected AnimacioPerCodi animacioPerCodi;

    Vector2Int coordenades;

    System.Func<Vector2Int, Hexagon[]> veins;
    System.Func<Vector2Int, Pe�a[]> veinsPe�a;



    public abstract bool EsPe�a { get; }
    public Vector2Int Coordenades => coordenades;

    public Transform Parent => parent;

    public Hexagon[] Veins => veins.Invoke(coordenades);
    public Pe�a[] VeinsPe�a => veinsPe�a.Invoke(coordenades);



}
