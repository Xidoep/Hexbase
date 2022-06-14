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
    public void Setup(Grid grid, Vector2Int coordenades, EstatPe�a estat)
    {
        veins = grid.Veins;
        crearPe�a = grid.CrearPe�a;
        this.estat = estat;
        this.coordenades = coordenades;
        //possibilitats = estat.Possibilitats;
    }

    //VARIABLES 
    [SerializeField] Transform parent;
    [SerializeField] protected AnimacioPerCodi animacioPerCodi;

    [SerializeField] protected EstatPe�a estat;
    [SerializeField] public bool acabadaDeCrear => WaveFunctionColapse.UltimaPe�aCreada == this;

    Vector2Int coordenades;
    protected System.Func<Vector2Int, Hexagon[]> veins;
    System.Action<Vector2Int> crearPe�a;

    public abstract bool EsPe�a { get; }
    public Vector2Int Coordenades => coordenades;
    public EstatPe�a Estat => estat;
    public string EstatName => estat.name;
    public bool EstatNull => estat == null;
    public Transform Parent => parent;
    public bool EstatIgualA(EstatPe�a altreEstat) => estat == altreEstat;

    //FUNCIONS
    public Hexagon[] Veins => veins.Invoke(coordenades);

    public void CrearPe�a() => crearPe�a.Invoke(coordenades);

    public abstract void Iniciar();
    public abstract void Actualitzar();
}
