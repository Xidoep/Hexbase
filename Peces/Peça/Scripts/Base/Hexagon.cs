using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    //protected const string SELECCIONAT_ID = "_Seleccionat";
    public virtual void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        veins = grid.Veins;
        veinsPe�a = grid.VeinsPe�a;
        this.coordenades = coordenades;
    }

    //VARIABLES 
    [SerializeField] Transform parent;

    //[SerializeField] protected AnimacioPerCodi animacioPerCodi;
    //[SerializeField] public AnimacioPerCodi animacio;

    Vector2Int coordenades;

    System.Func<Vector2Int, List<Hexagon>> veins;
    System.Func<Vector2Int, List<Pe�a>> veinsPe�a;

    protected System.Action<Hexagon, bool> mostrarInformacio;
    protected System.Action<Hexagon> amagarInformacio;

    public System.Action<Pe�a, bool> MostrarInformacio => mostrarInformacio;
    public System.Action<Pe�a> AmagarInformacio => amagarInformacio;


    public abstract bool EsPe�a { get; }
    public Vector2Int Coordenades => coordenades;

    public Transform Parent => parent;

    public List<Hexagon> Veins => veins.Invoke(coordenades);
    public List<Pe�a> VeinsPe�a => veinsPe�a.Invoke(coordenades);



}
