using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//**********************************************
//FALTA: treute estat i subestat d'aqui i passarlo a la peça.
//**********************************************

/// <summary>
/// La classe base de les peces del Grid
/// </summary>
[SelectionBase]
public abstract class Hexagon : MonoBehaviour
{
    //protected const string SELECCIONAT_ID = "_Seleccionat";
    public virtual void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        veins = grid.Veins;
        veinsPeça = grid.VeinsPeça;
        this.coordenades = coordenades;
        buidar = grid.Buidar;
    }

    //VARIABLES 
    [SerializeField] Transform parent;
    //[SerializeField] protected AnimacioPerCodi animacioPerCodi;
    [SerializeField] public Animacio_Scriptable animacio;

    Vector2Int coordenades;

    System.Action<Vector2Int> buidar;
    System.Func<Vector2Int, List<Hexagon>> veins;
    System.Func<Vector2Int, List<Peça>> veinsPeça;



    public abstract bool EsPeça { get; }
    public Vector2Int Coordenades => coordenades;

    public Transform Parent => parent;

    public List<Hexagon> Veins => veins.Invoke(coordenades);
    public List<Peça> VeinsPeça => veinsPeça.Invoke(coordenades);


    protected void Buidar() => buidar.Invoke(coordenades);


}
