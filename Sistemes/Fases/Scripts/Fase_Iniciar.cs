using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iniciat pel boto Inicial, Crea la primera peça del grid.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Iniciar")]
public class Fase_Iniciar : Fase
{
    [SerializeField] Fase colocar;
    [SerializeField] Modes modes;
    [SerializeField] Estat inicial;

    Grid grid;
    bool gridNet = true;

    

    public override void FaseStart()
    {
        Debug.Log("Fase_Iniciar > Actualitzar");

        modes.ConfigurarModes();

        if (gridNet)
        {
            PosarPrimeraPeça();
        }
        else
        {
            colocar.Iniciar();
        }
    }

    public void PosarPrimeraPeça()
    {
        Debug.Log("PosarPrimeraPeça");
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Aqui potser hi aniran diferents tipus de inici o de colocacio de peces segons el mode o el dia de l'any, etc..
        grid.CrearPeça(inicial, grid.Centre);
        gridNet = false;
        //colocar.Iniciar();
    }

    public void Reset()
    {
        gridNet = true;
    }


    new void OnDisable()
    {
        base.OnDisable();
        Reset();
    }
}

