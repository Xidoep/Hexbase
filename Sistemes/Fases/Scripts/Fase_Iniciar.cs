using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iniciat pel boto Inicial, Crea la primera peça del grid.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Iniciar")]
public class Fase_Iniciar : Fase
{
    [SerializeScriptableObject][SerializeField] Fase colocar;
    [SerializeScriptableObject][SerializeField] Modes modes;
    [SerializeScriptableObject][SerializeField] Estat inicial;
    [SerializeScriptableObject][SerializeField] SaveHex save;

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
            Debug.Log("Saltar a colocar");
            colocar.Iniciar();
        }


    }

    /*public void NovaPartida()
    {
        SetupPartida(Mode.Pila);
    }
   
    public void NovaFreeStyle()
    {
        SetupPartida(Mode.FreeStyle);
    }

    void SetupPartida(Mode mode)
    {
        Grid.Instance.Resetejar();

        modes.Set(mode);
        modes.ConfigurarModes();

        save.NouArxiu(modes.Mode);

        PosarPrimeraPeça();
    }*/

    public void PosarPrimeraPeça()
    {
        Debug.Log("PosarPrimeraPeça");

        Grid.Instance.CrearPeça(inicial, Grid.Instance.Centre);
        gridNet = false;
    }

    public void GridNet() => gridNet = true;
    public void GridBrut() => gridNet = false;


    new void OnDisable()
    {
        base.OnDisable();
        GridNet();
    }
}

