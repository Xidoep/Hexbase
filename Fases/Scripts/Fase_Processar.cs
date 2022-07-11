using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Procesar")]
public class Fase_Processar : Fase
{

    Grid grid;
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [SerializeField] Fase colocar;

    [SerializeField] Estat[] desbloquejadores;



    float startTime;
    Pe�a pe�a;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        pe�a = (Pe�a)arg;

        Debug.LogError(pe�a);



        startTime = Time.realtimeSinceStartup;
        WFC();

    }


    void WFC()
    {
        WaveFunctionColapse.UltimaPe�aCreada = pe�a;
        pe�a.Actualitzar();

        WaveFunctionColapse.Process(pe�a, Agrupar, CrearPe�aDesbloquejadora);
    }

    void Agrupar()
    {
        grups.Agrupdar(pe�a, Proximitat);
    }

    
    void Proximitat(int index)
    {
        Debug.LogError("--------------REPOBLAR---------------");
        //if (proximitats == null) proximitats = new List<Pe�a>();
        //else proximitats.Clear();

        List<Pe�a> proximitats = new List<Pe�a>();
        proximitats.Add(pe�a);

        List<Pe�a> grup = grups.Peces(index);
        for (int i = 0; i < grup.Count; i++)
        {
            proximitats.Add(grup[i]);
        }

        Pe�a[] veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Length; i++)
        {
            proximitats.Add(veins[i]);
        }

        repoblar.Proces(proximitats);

        proximitat.Process(proximitats, Finalitzar);
    }


    void Finalitzar(List<Pe�a> peces)
    {
        repoblar.Proces(peces);
        Debug.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", this);
        colocar.Iniciar();
    }

    void CrearPe�aDesbloquejadora(Vector2Int coordenada)
    {
        grid.CrearPe�a(desbloquejadores[Random.Range(0,desbloquejadores.Length)], coordenada, true);
    }


    public override void Finalitzar()
    {
        onFinish?.Invoke();
    }
}
