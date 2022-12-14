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

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    [Apartat("ANIMACIONS")]
    [SerializeField] Animacio_Scriptable actualitzar;

    //PRIVADES
    Grid grid;
    float startTime;
    Peça peça;
    List<Peça> perComprovar;
    List<Peça> canviades;


    public override void Inicialitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        peça = (Peça)arg;
        peça.Parent.localPosition = Vector3.up * 20;
        //Debug.LogError(peça);



        startTime = Time.realtimeSinceStartup;
        //WFC();
        Agrupar();
    }


    void Agrupar()
    {
        grups.Agrupdar(peça, Proximitat);
    }

    void Repoblacio_Primera()//Inicials
    {
        perComprovar = new List<Peça>() { peça };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(peça));

        repoblar.Proces(perComprovar, Proximitat);
    }

    void Proximitat()
    {
        perComprovar = new List<Peça>() { peça };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(peça));

        proximitat.Process(perComprovar, Repoblacio);
    }


    void Repoblacio(List<Peça> comprovades, List<Peça> canviades)
    {
        this.canviades = canviades;
        repoblar.Proces(comprovades, WFC);
    }



    void WFC()
    {
        wfc.Iniciar_WFC(peça, canviades, Produir);
    }

    void Produir()
    {
        Animar();
        produccio.Process(FinalitzarProcessos);
    }

    List<Peça> animades;

    void Animar() {
        peça.animacio.Play(peça.Parent);
        animades = new List<Peça>() { peça };

        for (int v = 0; v < peça.VeinsPeça.Count; v++)
        {
            actualitzar.Play(peça.VeinsPeça[v].Parent);
            animades.Add(peça.VeinsPeça[v]);
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
            for (int v = 0; v < canviades[c].VeinsPeça.Count; v++)
            {
                if (animades.Contains(canviades[c].VeinsPeça[v]))
                    continue;

                actualitzar.Play(canviades[c].VeinsPeça[v].Parent);
                animades.Add(canviades[c].VeinsPeça[v]);
            }
        }
    }

    void FinalitzarProcessos()
    {
        //Això també hauria de ser un proces.


        save.Add(peça, grups);
        save.Actualitzar(animades, grups);



        //Això ha de ser un altre proces a part.
        grid.Dimensionar(peça);












        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", peça);
        colocar.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
