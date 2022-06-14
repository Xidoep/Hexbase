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
    public void Setup(Grid grid, Vector2Int coordenades, EstatPeça estat)
    {
        veins = grid.Veins;
        crearPeça = grid.CrearPeça;
        this.estat = estat;
        this.coordenades = coordenades;
        //possibilitats = estat.Possibilitats;
    }

    //VARIABLES 
    [SerializeField] Transform parent;
    [SerializeField] protected AnimacioPerCodi animacioPerCodi;

    [SerializeField] protected EstatPeça estat;
    [SerializeField] public bool acabadaDeCrear => WaveFunctionColapse.UltimaPeçaCreada == this;

    Vector2Int coordenades;
    protected System.Func<Vector2Int, Hexagon[]> veins;
    System.Action<Vector2Int> crearPeça;

    public abstract bool EsPeça { get; }
    public Vector2Int Coordenades => coordenades;
    public EstatPeça Estat => estat;
    public string EstatName => estat.name;
    public bool EstatNull => estat == null;
    public Transform Parent => parent;
    public bool EstatIgualA(EstatPeça altreEstat) => estat == altreEstat;

    //FUNCIONS
    public Hexagon[] Veins => veins.Invoke(coordenades);

    public void CrearPeça() => crearPeça.Invoke(coordenades);

    public abstract void Iniciar();
    public abstract void Actualitzar();
}
