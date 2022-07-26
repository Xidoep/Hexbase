using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPeça = false;

    Grid grid;

    [Linia]
    [SerializeField] PoolPeces poolPeces;
    [Linia]
    [Nota("If not null, will take this insead of from pool")]
    [SerializeField] Estat debug;


    public Estat PeçaSeleccionada { set => debug = value; }
    public static bool PermesColoarPeça => permesColoarPeça;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la peça inicial agafantla del pool de peces.
        permesColoarPeça = true;

        //poolPeces.Get
    }


    public void CrearPeça(Vector2Int coordenada)
    {
        grid.CrearPeça(debug == null ? poolPeces.Get : debug, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        permesColoarPeça = false;
    }
}
