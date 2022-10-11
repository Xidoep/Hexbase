using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Tester")]
public class Tester : ScriptableObject
{
    [SerializeField] Estat[] peces;
    [SerializeField] Fase_Processar processar;
    [SerializeField] Fase_Colocar colocar;

    Grid grid;


    Ranura[] ranures;
    Hexagon[] hexagons;
    List<Hexagon> viables;
    Coroutine proces;

    [ContextMenu("Iniciar")]
    void Iniciar()
    {
        processar.OnFinish = StepDelayed;
        if (grid == null)
        {
            grid = FindObjectOfType<Grid>();
        }

        StepDelayed();
    }

    void StepDelayed()
    {
        proces = XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }

    void Step()
    {
        ranures = grid.GetComponentsInChildren<Ranura>();
        /*hexagons = grid.GetComponentsInChildren<Hexagon>();
        viables = new List<Hexagon>();
        for (int i = 0; i < hexagons.Length; i++)
        {
            if (hexagons[i].EstatNull) 
                viables.Add(hexagons[i]);

        }*/

        //Hexagon seleccionat = viables[Random.Range(0, viables.Count)];
        Ranura seleccionat = ranures[Random.Range(0, ranures.Length)];
        /*if (!seleccionat.EstatNull)
        {
            Debug.LogError("PERO SI AQUESTA NO ES NULLA!!!");
        }*/

        colocar.Seleccionar(peces[Random.Range(0, peces.Length)]);
        seleccionat.CrearPeça();
        //Destroy(seleccionat.gameObject, 0.05f);
        //WaveFunctionColapse.EnFinalitzar = StepDelayed;
    }

    private void OnDisable()
    {
        proces?.StopCoroutine();
    }

    private void OnDestroy()
    {
        proces?.StopCoroutine();
    }
}
