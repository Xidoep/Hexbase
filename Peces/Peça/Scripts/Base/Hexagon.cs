using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[SelectionBase]
public abstract class Hexagon : MonoBehaviour
{
    public virtual void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        veins = grid.Veins;
        veinsPeça = grid.VeinsPeça;
        this.coordenades = coordenades;
    }

    [SerializeField] Transform parent;

    Vector2Int coordenades;

    System.Func<Vector2Int, List<Hexagon>> veins;
    System.Func<Vector2Int, List<Peça>> veinsPeça;

    protected System.Action<Hexagon, bool> mostrarInformacio;
    protected System.Action<Hexagon> amagarInformacio;



    //ABSTRACT
    public abstract bool EsPeça { get; }
    public abstract void OnPointerEnter();
    public abstract void OnPointerExit();
    public virtual void OnPointerDown() { }
    public virtual void OnPointerUp() { }



    //PROPIETATS
    public Transform Parent => parent;
    public Vector2Int Coordenades => coordenades;
    public List<Hexagon> Veins => veins.Invoke(coordenades);
    public List<Peça> VeinsPeça => veinsPeça.Invoke(coordenades);
    public System.Action<Peça, bool> MostrarInformacio => mostrarInformacio;
    public System.Action<Peça> AmagarInformacio => amagarInformacio;







}
