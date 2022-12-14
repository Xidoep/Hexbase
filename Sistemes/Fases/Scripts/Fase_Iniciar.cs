using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iniciat pel boto Inicial, Crea la primera peça del grid.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Iniciar")]
public class Fase_Iniciar : Fase
{
    [Linia]
    [Apartat("INICIAL")]
    [SerializeField] Fase colocar;
    [SerializeField] Estat inicial; //Posar aixo en un scriptable que controli la peça que s'ha seleccionat. "Seleccio" o algo aixi

    Grid grid;

    public override void Inicialitzar()
    {
        Debug.Log("Fase_Iniciar > Actualitzar");
        PosarPrimeraPeça();
    }

    public void PosarPrimeraPeça()
    {
        Debug.Log("PosarPrimeraPeça");
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearPeça(inicial, grid.Centre);

        colocar.Iniciar();
    }
}

public enum Mode { FreeSyle, pila }