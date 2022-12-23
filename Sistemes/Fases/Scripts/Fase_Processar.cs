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
    [SerializeField] Fase resoldre;

    [Apartat("ANIMACIONS")]
    [SerializeField] Visualitzacions visualitzacions;
    [SerializeField] Animacio_Scriptable actualitzar; //Aixo hauria de dirse: Veines

    //PRIVADES
    Grid grid;
    float startTime;
    Pe�a pe�a;
    List<Pe�a> perComprovar;
    List<Pe�a> canviades;
    List<Pe�a> animades;


    public override void FaseStart()
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
        //grups.Agrupdar(pe�a, Proximitat);
        grups.Agrupdar(pe�a, WFC_Inicial);
    }

    /*void Repoblacio_Primera()//Inicials
    {
        perComprovar = new List<Pe�a>() { pe�a };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(pe�a));

        repoblar.Proces(perComprovar, Proximitat);
    }*/

    
    void WFC_Inicial() //Primer WFC per la pe�a inicial.
    {
        wfc.Iniciar_WFC(pe�a, new List<Pe�a>(), Proximitat);
    }

    void Proximitat()
    {
        pe�a.animacio.Play(pe�a.Parent);
        animades = new List<Pe�a>() { pe�a };

        for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
        {
            actualitzar.Play(pe�a.VeinsPe�a[v].Parent);
            animades.Add(pe�a.VeinsPe�a[v]);
        }

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
        if(canviades != null && canviades.Count > 0)
        {
            wfc.Iniciar_WFC(pe�a, canviades, Produir);
        }
        else
        {
            Produir();
        }
    }

    void Produir()
    {
        Animar();
        produccio.Process(FinalitzarProcessos);
    }


    void Animar() {
        if (canviades.Contains(pe�a))
        {
            visualitzacions.CanviarEstat(pe�a);
            for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
            {
                actualitzar.Play(pe�a.VeinsPe�a[v].Parent);
            }
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

        visualitzacions.GuanyarPunts(1.5f);
    }

    void FinalitzarProcessos()
    {
        //Aix� tamb� hauria de ser un proces.


        save.Add(pe�a, grups);
        save.Actualitzar(animades, grups);



        //Aix� ha de ser un altre proces a part.
        grid.Dimensionar(pe�a);












        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", pe�a);
        resoldre.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
