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
    Peça peça;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        peça = (Peça)arg;

        Debug.LogError(peça);



        startTime = Time.realtimeSinceStartup;
        WFC();

    }


    void WFC()
    {
        WaveFunctionColapse.UltimaPeçaCreada = peça;
        peça.Actualitzar();

        WaveFunctionColapse.Process(peça, Agrupar, CrearPeçaDesbloquejadora);
    }

    void Agrupar()
    {
        grups.Agrupdar(peça, Proximitat);
    }

    
    void Proximitat(int index)
    {
        Debug.LogError("--------------REPOBLAR---------------");
        //if (proximitats == null) proximitats = new List<Peça>();
        //else proximitats.Clear();

        List<Peça> proximitats = new List<Peça>();
        proximitats.Add(peça);

        List<Peça> grup = grups.Peces(index);
        for (int i = 0; i < grup.Count; i++)
        {
            proximitats.Add(grup[i]);
        }

        Peça[] veins = peça.VeinsPeça;
        for (int i = 0; i < veins.Length; i++)
        {
            proximitats.Add(veins[i]);
        }

        repoblar.Proces(proximitats);

        proximitat.Process(proximitats, Finalitzar);
    }


    void Finalitzar(List<Peça> peces)
    {
        repoblar.Proces(peces);
        Debug.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", this);
        colocar.Iniciar();
    }

    void CrearPeçaDesbloquejadora(Vector2Int coordenada)
    {
        grid.CrearPeça(desbloquejadores[Random.Range(0,desbloquejadores.Length)], coordenada, true);
    }


    public override void Finalitzar()
    {
        onFinish?.Invoke();
    }
}
