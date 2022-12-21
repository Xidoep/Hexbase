using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iniciat pel boto Inicial, Crea la primera pe�a del grid.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Iniciar")]
public class Fase_Iniciar : Fase
{
    [SerializeField] Fase colocar;
    [SerializeField] Modes modes;
    [SerializeField] Estat inicial;

    Grid grid;

    public override void FaseStart()
    {
        Debug.Log("Fase_Iniciar > Actualitzar");

        modes.ConfigurarModes();

        PosarPrimeraPe�a();
    }

    public void PosarPrimeraPe�a()
    {
        Debug.Log("PosarPrimeraPe�a");
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Aqui potser hi aniran diferents tipus de inici o de colocacio de peces segons el mode o el dia de l'any, etc..
        grid.CrearPe�a(inicial, grid.Centre);

        //colocar.Iniciar();
    }
}

