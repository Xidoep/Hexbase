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

    //INTERN
    float startTime;
    Peça peça;
    List<Peça> perComprovar;
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

    void Proximitat()
    {
        perComprovar = new List<Peça>() { peça };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(peça));

        repoblar.Proces(perComprovar);

        proximitat.Process(perComprovar, Repoblar);
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
        wfc.Iniciar_WFC(peça, canviades, Segona_Proximitat);
    }

    void Segona_Proximitat()
    {
        proximitat.Process(perComprovar, Segona_WFC);
    }
    void Segona_WFC(List<Peça> comprovades, List<Peça> canviades)
    {
        if(canviades.Count == 0)
        {
            Produir();
            return;
        }
        wfc.Iniciar_WFC(peça, canviades, Produir,true);
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
