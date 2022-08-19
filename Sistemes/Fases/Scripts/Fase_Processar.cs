using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Procesar")]
public class Fase_Processar : Fase
{
    Grid grid;

    [Apartat("PROCESSOS")]
    [SerializeField] WaveFunctionColpaseScriptable wfc;
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [SerializeField] Produccio produccio;

    [Apartat("SEG�ENT FASE")]
    [SerializeField] Fase colocar;

    [Linia]
    [Nota("Estats per desbloquejar el WFC")]
    [SerializeField] Estat[] desbloquejadores;


    float startTime;
    Pe�a pe�a;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        pe�a = (Pe�a)arg;

        //Debug.LogError(pe�a);



        startTime = Time.realtimeSinceStartup;
        //WFC();
        Agrupar();
    }


    void Agrupar()
    {
        grups.Agrupdar(pe�a, Proximitat);
    }

    void Proximitat(int index)
    {
        List<Pe�a> proximitats = new List<Pe�a>() { pe�a };
        proximitats.AddRange(proximitat.GetPecesToComprovar(pe�a));

        repoblar.Proces(proximitats);

        proximitat.Process(proximitats, Repoblar);
    }

    void Repoblar(List<Pe�a> comprovades, List<Pe�a> canviades)
    //void Repoblar(List<Pe�a> comprovades)
    {
        //El segon repoblar �s perque les cases es creen segons les peces que tenen al voltant,
        //per tant s'han de mirar despres que aquestes haguin "canviat".
        repoblar.Proces(comprovades);
        WFC(canviades);
        //WFC(comprovades);
    }

    void WFC(List<Pe�a> canviades)
    {
        wfc.Iniciar_WFC(pe�a, canviades, Produir);
    }

    void Produir(/*List<Pe�a> peces*/)
    {
        produccio.Process(FinalitzarProcessos);
    }

    void FinalitzarProcessos()
    {
        //repoblar.Proces(peces);
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
