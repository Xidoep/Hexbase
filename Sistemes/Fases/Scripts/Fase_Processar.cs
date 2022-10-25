using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

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
    [SerializeField] SaveHex save;

    [Apartat("SEG�ENT FASE")]
    [SerializeField] Fase colocar;

    [Apartat("ANIMACIONS")]
    [SerializeField] Animacio_Scriptable actualitzar;
    [SerializeField] Animacio_Scriptable actDetalls;

    //INTERN
    float startTime;
    Pe�a pe�a;
    List<Pe�a> perComprovar;
    List<Pe�a> canviades; 
    public override void Actualitzar()
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

    void Proximitat()
    {
        perComprovar = new List<Pe�a>() { pe�a };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(pe�a));

        repoblar.Proces(perComprovar);

        proximitat.Process(perComprovar, Repoblar);
    }

    void Repoblar(List<Pe�a> comprovades, List<Pe�a> canviades)
    //void Repoblar(List<Pe�a> comprovades)
    {
        this.canviades = canviades;
        //El segon repoblar �s perque les cases es creen segons les peces que tenen al voltant,
        //per tant s'han de mirar despres que aquestes haguin "canviat".
        repoblar.Proces(comprovades);
        WFC(canviades);
        //WFC(comprovades);
    }

    void WFC(List<Pe�a> canviades)
    {
        wfc.Iniciar_WFC(pe�a, canviades, Segona_Proximitat);
    }

    void Segona_Proximitat()
    {
        proximitat.Process(perComprovar, Segona_WFC);
    }
    void Segona_WFC(List<Pe�a> comprovades, List<Pe�a> canviades)
    {
        
        if (canviades.Count == 0)
        {
            Produir();
            return;
        }
        wfc.Iniciar_WFC(pe�a, canviades, Produir,true);
    }

    void Produir(/*List<Pe�a> peces*/)
    {
        produccio.Process(FinalitzarProcessos);
    }

    List<Pe�a> animades;

    void FinalitzarProcessos()
    {
        pe�a.animacio.Play(pe�a.Parent);
        new Animacio_Esdeveniment(pe�a.Detalls).Play(pe�a.gameObject, 1.5f, Transicio.clamp);
        animades = new List<Pe�a>() { pe�a };

        for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
        {
            actualitzar.Play(pe�a.VeinsPe�a[v].Parent);
            new Animacio_Esdeveniment(pe�a.VeinsPe�a[v].Detalls).Play(pe�a.VeinsPe�a[v].gameObject, 1.5f, Transicio.clamp);
            animades.Add(pe�a.VeinsPe�a[v]);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            if (animades.Contains(canviades[c]))
                continue;

            actualitzar.Play(canviades[c].Parent);
            new Animacio_Esdeveniment(canviades[c].Detalls).Play(canviades[c].gameObject, 1.5f, Transicio.clamp);
            animades.Add(canviades[c]);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            for (int v = 0; v < canviades[c].VeinsPe�a.Count; v++)
            {
                if (animades.Contains(canviades[c].VeinsPe�a[v]))
                    continue;

                actualitzar.Play(canviades[c].VeinsPe�a[v].Parent);
                new Animacio_Esdeveniment(canviades[c].VeinsPe�a[v].Detalls).Play(canviades[c].VeinsPe�a[v].gameObject, 1.5f, Transicio.clamp);
                animades.Add(canviades[c].VeinsPe�a[v]);
            }
        }

        save.Add(pe�a, grups);
        save.Actualitzar(animades, grups);


        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", pe�a);
        colocar.Iniciar();
    }






    public override void Finalitzar()
    {
        OnFinish_Invocar();
    }
}
