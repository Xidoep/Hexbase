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
    [SerializeField] SaveHex save;

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

    public void NovaPartida()
    {
        Grid.Instance.Resetejar();

        modes.Set(Mode.Pila);
        modes.ConfigurarModes();

        save.NouArxiu(modes.Mode);

        PosarPrimeraPeça();
    }

    public void PosarPrimeraPeça()
    {
        Debug.Log("PosarPrimeraPeça");

        //La primera partida ja ha de ser interessant. Serà el que ferà que vulguin tornar a jugar. Si la segona parida es diferent ja els hi petarà el cap.
        //Aqui potser hi aniran diferents tipus de inici o de colocacio de peces segons el mode o el dia de l'any, etc..
        Grid.Instance.CrearPeça(inicial, Grid.Instance.Centre);
        gridNet = false;
        //colocar.Iniciar();
    }

    public void Reset() => gridNet = true;
    public void GridBrut() => gridNet = false;


    new void OnDisable()
    {
        base.OnDisable();
        Reset();
    }
}

