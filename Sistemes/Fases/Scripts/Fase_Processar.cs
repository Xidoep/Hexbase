using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

/// <summary>
/// Analitza el grid.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Procesar")]
public class Fase_Processar : Fase
{
    [Apartat("PROCESSOS")]
    [SerializeField] WaveFunctionColpaseScriptable wfc;
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [SerializeField] Produccio produccio;
    [SerializeField] SaveHex save;

    [Apartat("SEG�ENT FASE")]
    [SerializeField] Fase colocar;

    [Apartat("ANIMACIONS")]
    [SerializeField] Animacio_Scriptable actualitzar;

    //PRIVADES
    Grid grid;
    float startTime;
    Pe�a pe�a;
    List<Pe�a> perComprovar;
    List<Pe�a> canviades;


    public override void Inicialitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        pe�a = (Pe�a)arg;
        pe�a.Parent.localPosition = Vector3.up * 20;
        //Debug.LogError(pe�a);



        startTime = Time.realtimeSinceStartup;
        //WFC();
        Agrupar();
    }


    void Agrupar()
    {
        grups.Agrupdar(pe�a, Proximitat);
    }

    void Repoblacio_Primera()//Inicials
    {
        perComprovar = new List<Pe�a>() { pe�a };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(pe�a));

        repoblar.Proces(perComprovar, Proximitat);
    }

    void Proximitat()
    {
        perComprovar = new List<Pe�a>() { pe�a };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(pe�a));

        proximitat.Process(perComprovar, Repoblacio);
    }


    void Repoblacio(List<Pe�a> comprovades, List<Pe�a> canviades)
    {
        this.canviades = canviades;
        repoblar.Proces(comprovades, WFC);
    }



    void WFC()
    {
        wfc.Iniciar_WFC(pe�a, canviades, Produir);
    }

    void Produir()
    {
        Animar();
        produccio.Process(FinalitzarProcessos);
    }

    List<Pe�a> animades;

    void Animar() {
        pe�a.animacio.Play(pe�a.Parent);
        animades = new List<Pe�a>() { pe�a };

        for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
        {
            actualitzar.Play(pe�a.VeinsPe�a[v].Parent);
            animades.Add(pe�a.VeinsPe�a[v]);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            if (animades.Contains(canviades[c]))
                continue;

            actualitzar.Play(canviades[c].Parent);
            animades.Add(canviades[c]);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            for (int v = 0; v < canviades[c].VeinsPe�a.Count; v++)
            {
                if (animades.Contains(canviades[c].VeinsPe�a[v]))
                    continue;

                actualitzar.Play(canviades[c].VeinsPe�a[v].Parent);
                animades.Add(canviades[c].VeinsPe�a[v]);
            }
        }
    }

    void FinalitzarProcessos()
    {
        //Aix� tamb� hauria de ser un proces.


        save.Add(pe�a, grups);
        save.Actualitzar(animades, grups);



        //Aix� ha de ser un altre proces a part.
        grid.Dimensionar(pe�a);












        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", pe�a);
        colocar.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
