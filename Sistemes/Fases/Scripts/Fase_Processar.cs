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
    [SerializeField] Prediccio prediccio;
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [SerializeField] Produccio produccio;
    [SerializeField] SaveHex save;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase resoldre;

    [Apartat("ANIMACIONS")]
    [SerializeField] Visualitzacions visualitzacions;

    [Apartat("RIU")]
    [SerializeField] Estat riu;

    //PRIVADES
    Grid grid;
    float startTime;
    Peça peça;
    List<Peça> perComprovar;
    List<Peça> canviades;
    List<Peça> animades;


    public override void FaseStart()
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
        //grups.Agrupdar(peça, Proximitat);
        prediccio.FinalitzacioForçada();
        grups.Interrompre();
        grups.Agrupdar(grups.Grup, peça, WFC_Inicial);
    }

    /*void Repoblacio_Primera()//Inicials
    {
        perComprovar = new List<Peça>() { peça };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(peça));

        repoblar.Proces(perComprovar, Proximitat);
    }*/

    
    void WFC_Inicial() //Primer WFC per la peça inicial.
    {
        if (!peça.EstatIgualA(riu))
            wfc.Iniciar_WFC(peça, new List<Peça>(), Proximitat);
        else
            Proximitat();

    }

    void Proximitat()
    {
        peça.animacio.Play(peça.Parent);
        //animades = new List<Peça>() { peça };
        animades = new List<Peça>();

        for (int v = 0; v < peça.VeinsPeça.Count; v++)
        {
            visualitzacions.ReaccioVeina(peça.VeinsPeça[v]);
            //actualitzar.Play(peça.VeinsPeça[v].Parent);
            //animades.Add(peça.VeinsPeça[v]);
        }

        perComprovar = new List<Peça>() { peça };
        List<Peça> comprovar = proximitat.GetPecesToComprovar(peça);
        for (int i = 0; i < comprovar.Count; i++)
        {
            if (!perComprovar.Contains(comprovar[i])) perComprovar.Add(comprovar[i]);
        }
        //perComprovar.AddRange(proximitat.GetPecesToComprovar(peça));

        proximitat.Process(perComprovar, Repoblacio, true);
    }


    void Repoblacio(List<Peça> comprovades, List<Peça> canviades)
    {
        this.canviades = canviades;
        repoblar.Proces(comprovades, WFC);
    }



    void WFC()
    {
        if (canviades != null && canviades.Count > 0 || peça.EstatIgualA(riu))
        {
            wfc.Iniciar_WFC(peça, canviades, Produir);
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
        /*if (canviades.Contains(peça))
        {
            visualitzacions.CanviarEstat(peça);
            for (int v = 0; v < peça.VeinsPeça.Count; v++)
            {
                actualitzar.Play(peça.VeinsPeça[v].Parent);
            }
        }*/

        for (int c = 0; c < canviades.Count; c++)
        {
            Debug.LogError($"Canviada: {canviades[c]}");
            if (animades.Contains(canviades[c]))
                continue;


            visualitzacions.CanviarEstat(canviades[c]);
            //actualitzar.Play(canviades[c].Parent);
            animades.Add(canviades[c]);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            for (int v = 0; v < canviades[c].VeinsPeça.Count; v++)
            {
                if (animades.Contains(canviades[c].VeinsPeça[v]))
                    continue;

                visualitzacions.ReaccioVeina(canviades[c].VeinsPeça[v]);
                //actualitzar.Play(canviades[c].VeinsPeça[v].Parent);
                animades.Add(canviades[c].VeinsPeça[v]);
            }
        }

        visualitzacions.GuanyarExperienciaProximitat();
    }

    void FinalitzarProcessos()
    {
        //Això també hauria de ser un proces.


        save.Add(peça, grups);
        save.Actualitzar(animades, grups);



        //Això ha de ser un altre proces a part.
        grid.Dimensionar(peça);












        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", peça);
        resoldre.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
