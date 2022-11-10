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
    [SerializeField] PoolPeces pool;
    [SerializeField] int pecesInicials = 20;
    //[Space(10)]
    //[SerializeField] SaveHex save;

    public override void Actualitzar()
    {
        PosarPrimeraPeça();
    }

    public void PosarPrimeraPeça()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearPeça(inicial, grid.Centre);
        pool.Inicialize(pecesInicials);

        colocar.Iniciar();
    }

    public override void Finalitzar()
    {
        OnFinish_Invocar();


    }
}
