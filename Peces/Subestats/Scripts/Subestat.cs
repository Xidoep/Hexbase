using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat")]
public class Subestat : ScriptableObject
{
    public virtual Subestat Setup(Peça peça) => this;


    [Header("PUNTS")]
    [SerializeField] int punts;

    [Linia]
    [SerializeField] Condicio[] condicions;

    [Linia]
    [SerializeField] DetallScriptable[] detallsScriptables;

    [Linia]
    [SerializeField] bool caminable;
    [SerializeField] bool aquatic;

    [Linia]
    [SerializeField] Estat.TilesPossibles[] tilesAlternatius;


    public Condicio[] Condicions => condicions;
    public DetallScriptable[] Detalls => detallsScriptables;
    public bool Caminable => caminable;
    public bool Aquatic => aquatic;
    public bool TilesPropis => tilesAlternatius.Length > 0;
    public Estat.TilesPossibles[] TilesAlternatius => tilesAlternatius;

    public virtual Producte[] Produccio() => null;
    public int Punts => punts;

    //public virtual Subestat Get(Peça peça) => this;





    /*private void OnValidate()
    {
        if (recurs == null)
            return;

        produccio = (Produccio)XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
    }*/
}