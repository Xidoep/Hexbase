using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPe�a = false;

    Grid grid;

    [Linia]
    [SerializeField] PoolPeces poolPeces;
    [Linia]
    [Nota("If not null, will take this insead of from pool")]
    [SerializeField] Estat debug;


    public Estat Pe�aSeleccionada { set => debug = value; }
    public static bool PermesColoarPe�a => permesColoarPe�a;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la pe�a inicial agafantla del pool de peces.
        permesColoarPe�a = true;

        //poolPeces.Get
    }


    public void CrearPe�a(Vector2Int coordenada)
    {
        grid.CrearPe�a(debug == null ? poolPeces.Get : debug, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        permesColoarPe�a = false;
    }
}
