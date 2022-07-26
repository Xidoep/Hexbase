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
    [SerializeField] Estat inicial; //Posar aixo en un scriptable que controli la pe�a que s'ha seleccionat. "Seleccio" o algo aixi
    [SerializeField] PoolPeces pool;

    [Linia]
    [Header("SAVE")]
    [Nota("Si hi ha un archiu de guardat, el carregar�. Si no fer� un inici normal.")]
    [SerializeField] SaveHex save;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();
        
        //AIXO NO HAURA DE FUNCIONAR AIXI. HI HAUR� VAROS SAVES SLOTS I EN TRIARAS UN.
        if (!save)
        {
            grid.CrearPe�a(inicial, grid.Centre);
            pool.Inicialize(20);
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
