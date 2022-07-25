using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Iniciar")]
public class Fase_Iniciar : Fase
{
    Grid grid;

    [SerializeField] Fase_Colocar colocar;
    
    [Linia]
    [Header("INICIAL")]
    [SerializeField] Estat inicial; //Posar aixo en un scriptable que controli la peça que s'ha seleccionat. "Seleccio" o algo aixi

    [Linia]
    [Header("SAVE")]
    [SerializeField] SaveHex save;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();
        
        //AIXO NO HAURA DE FUNCIONAR AIXI. HI HAURÀ VAROS SAVES SLOTS I EN TRIARAS UN.
        if (!save)
        {
            grid.CrearPeça(inicial, grid.Centre);
        }
        else
        {
            save.Load();
        }
        

        colocar.Iniciar();
    }


    public override void Finalitzar()
    {
        onFinish?.Invoke();


    }
}
