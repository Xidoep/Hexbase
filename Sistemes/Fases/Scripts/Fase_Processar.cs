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

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    [Linia]
    [Nota("Estats per desbloquejar el WFC")]
    [SerializeField] Estat[] desbloquejadores;


    float startTime;
    Peça peça;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        peça = (Peça)arg;

        //Debug.LogError(peça);



        startTime = Time.realtimeSinceStartup;
        //WFC();
        Agrupar();
    }


    void Agrupar()
    {
        grups.Agrupdar(peça, Proximitat);
    }

    void Proximitat(int index)
    {
        List<Peça> proximitats = new List<Peça>() { peça };
        proximitats.AddRange(proximitat.GetPecesToComprovar(peça));

        repoblar.Proces(proximitats);

        proximitat.Process(proximitats, Repoblar);
    }

    void Repoblar(List<Peça> comprovades, List<Peça> canviades)
    //void Repoblar(List<Peça> comprovades)
    {
        //El segon repoblar és perque les cases es creen segons les peces que tenen al voltant,
        //per tant s'han de mirar despres que aquestes haguin "canviat".
        repoblar.Proces(comprovades);
        WFC(canviades);
        //WFC(comprovades);
    }

    void WFC(List<Peça> canviades)
    {
        wfc.Iniciar_WFC(peça, canviades, Produir);
    }

    void Produir(/*List<Peça> peces*/)
    {
        produccio.Process(FinalitzarProcessos);
    }

    void FinalitzarProcessos()
    {
        //repoblar.Proces(peces);
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
