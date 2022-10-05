using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
    Grid grid;
    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        grid.CrearBoto(grid.Centre);

    }


    public override void Finalitzar()
    {
        onFinish?.Invoke();


    }
}
