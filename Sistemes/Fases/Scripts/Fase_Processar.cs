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

    void FinalitzarProcessos()
    {
        save.Add(peça, grups);
        
        peça.animacio.Play(peça.Parent);
        new Animacio_Esdeveniment(peça.Detalls).Play(peça.gameObject, 1.5f, Transicio.clamp);

        for (int i = 0; i < peça.VeinsPeça.Count; i++)
        {
            //((Animacio_RotacioVector)actualitzar.Animacions[0]).SetEix(XS_Utils.XS_Direction.GetDirectionToTarget(peça.VeinsPeça[i].transform, peça.transform));
            actualitzar.Play(peça.VeinsPeça[i].Parent);
            new Animacio_Esdeveniment(peça.VeinsPeça[i].Detalls).Play(peça.VeinsPeça[i].gameObject, 1.5f, Transicio.clamp);
        }

        for (int i = 0; i < canviades.Count; i++)
        {
            if (canviades[i] == peça)
                continue;

            actualitzar.Play(canviades[i].Parent);
            new Animacio_Esdeveniment(canviades[i].Detalls).Play(canviades[i].gameObject, 1.5f, Transicio.clamp);
        }




        //save.Actualitzar(perComprovar, grups);

        //repoblar.Proces(peces);
        Debug.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", this);
        colocar.Iniciar();
    }






    public override void Finalitzar()
    {
        OnFinish_Invocar();
    }
}
