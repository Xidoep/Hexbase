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

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    [Apartat("ANIMACIONS")]
    [SerializeField] Animacio_Scriptable actualitzar;
    [SerializeField] Animacio_Scriptable actDetalls;

    //INTERN
    float startTime;
    Peça peça;
    List<Peça> perComprovar;
    List<Peça> canviades; 
    public override void Actualitzar()
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
        this.canviades = canviades;
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
        
        if (canviades.Count == 0)
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

    List<Peça> animades;

    void FinalitzarProcessos()
    {
        peça.animacio.Play(peça.Parent);
        new Animacio_Esdeveniment(peça.Detalls).Play(peça.gameObject, 1.5f, Transicio.clamp);
        animades = new List<Peça>() { peça };

        for (int v = 0; v < peça.VeinsPeça.Count; v++)
        {
            actualitzar.Play(peça.VeinsPeça[v].Parent);
            new Animacio_Esdeveniment(peça.VeinsPeça[v].Detalls).Play(peça.VeinsPeça[v].gameObject, 1.5f, Transicio.clamp);
            animades.Add(peça.VeinsPeça[v]);
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
            for (int v = 0; v < canviades[c].VeinsPeça.Count; v++)
            {
                if (animades.Contains(canviades[c].VeinsPeça[v]))
                    continue;

                actualitzar.Play(canviades[c].VeinsPeça[v].Parent);
                new Animacio_Esdeveniment(canviades[c].VeinsPeça[v].Detalls).Play(canviades[c].VeinsPeça[v].gameObject, 1.5f, Transicio.clamp);
                animades.Add(canviades[c].VeinsPeça[v]);
            }
        }

        save.Add(peça, grups);
        save.Actualitzar(animades, grups);


        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", peça);
        colocar.Iniciar();
    }






    public override void Finalitzar()
    {
        OnFinish_Invocar();
    }
}
