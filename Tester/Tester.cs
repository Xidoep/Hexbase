using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Tester")]
public class Tester : ScriptableObject
{
    [SerializeField] Grid grid;
    [SerializeField] EstatPeça[] peces;

    Hexagon[] hexagons;
    List<Hexagon> viables;

    [ContextMenu("Iniciar")]
    void Iniciar()
    {
        if(grid == null)
        {
            grid = FindObjectOfType<Grid>();
        }

        StepDelayed();
    }

    void StepDelayed()
    {
        XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }

    void Step()
    {
        hexagons = grid.GetComponentsInChildren<Hexagon>();
        viables = new List<Hexagon>();
        for (int i = 0; i < hexagons.Length; i++)
        {
            if (hexagons[i].EstatNull) 
                viables.Add(hexagons[i]);

        }

        Hexagon seleccionat = viables[Random.Range(0, viables.Count)];
        if(!seleccionat.EstatNull)
        {
            Debug.LogError("PERO SI AQUESTA NO ES NULLA!!!");
        }

        grid.Seleccionada = peces[Random.Range(0, peces.Length)];
        seleccionat.CrearPeça();
        Destroy(seleccionat.gameObject, 0.05f);
        WaveFunctionColapse.EnFinalitzar = StepDelayed;
    }
}
