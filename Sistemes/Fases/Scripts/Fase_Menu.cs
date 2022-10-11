using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
    Grid grid;

    [SerializeField] Grups grups;
    [SerializeField] Fase colocar;
    [Space(10)]
    [SerializeField] SaveHex save;
    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        

        if (!save.TePeces)
        {
            grid.CrearBoto(grid.Centre);
        }
        else
        {
            save.Load(grups, colocar);
        }
    }


    public override void Finalitzar()
    {
        OnFinish_Invocar();
    }
}
