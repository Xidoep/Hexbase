using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



/// <summary>
/// La classe base de les peces del Grid
/// </summary>
[SelectionBase]
public abstract class Hexagon : MonoBehaviour
{
    public virtual void Setup(Grid grid, Vector2Int coordenades, EstatPeça estat)
    {
        this.grid = grid;
        veins = grid.Veins;
        crearPeça = grid.CrearPeça;
        this.estat = estat;
        this.coordenades = coordenades;
        //possibilitats = estat.Possibilitats;
    }

    //VARIABLES 
    [SerializeField] Transform parent;
    [SerializeField] protected AnimacioPerCodi animacioPerCodi;

    [SerializeField] EstatPeça estat;
    [SerializeField] protected Subestat subestat;

    Vector2Int coordenades;
    Grid grid;
    protected System.Func<Vector2Int, Hexagon[]> veins;
    System.Action<Vector2Int> crearPeça;



    public abstract bool EsPeça { get; }
    public Vector2Int Coordenades => coordenades;
    public EstatPeça Estat => estat;
    public string EstatName => estat.name;
    public bool EstatNull => estat == null;
    public Transform Parent => parent;
    public bool EstatIgualA(EstatPeça altreEstat) => estat == altreEstat;
    public Grid Grid => grid;
    public Hexagon[] Veins => veins.Invoke(coordenades);

    public Subestat Subestat => subestat;
    public virtual void CanviarSubestat(Subestat subestat) => this.subestat = subestat;
    //FUNCIONS

    public void CrearPeça() => crearPeça.Invoke(coordenades);



    public abstract void Iniciar();
    public abstract void Actualitzar();
}
