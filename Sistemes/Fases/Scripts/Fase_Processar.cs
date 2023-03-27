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

    [Apartat("SEG�ENT FASE")]
    [SerializeField] Fase resoldre;

    [Apartat("ANIMACIONS")]
    [SerializeField] Visualitzacions visualitzacions;

    [Apartat("RIU")]
    [SerializeField] Estat riu;

    //PRIVADES
    float startTime;
    Pe�a pe�a;
    List<Pe�a> perComprovar;
    List<Proximitat.Canvis> canviades;
    List<Pe�a> canviadesPe�a;
    List<Pe�a> animades;


    public override void FaseStart()
    {
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
        prediccio.FinalitzacioFor�ada();
        grups.Interrompre();
        grups.Agrupdar(grups.Grup, pe�a, WFC_Inicial);
    }

    /*void Repoblacio_Primera()//Inicials
    {
        perComprovar = new List<Pe�a>() { pe�a };
        perComprovar.AddRange(proximitat.GetPecesToComprovar(pe�a));

        repoblar.Proces(perComprovar, Proximitat);
    }*/

    
    void WFC_Inicial() //Primer WFC per la pe�a inicial.
    {
        if (!pe�a.EstatIgualA(riu))
            wfc.Iniciar_WFC(pe�a, new List<Pe�a>(), Proximitat);
        else
            Proximitat();

    }

    void Proximitat()
    {
        Debug.LogError($"Colocar pe�a {pe�a.name}");
        visualitzacions.Colocar(pe�a);
        //pe�a.animacio.Play(pe�a.Parent);
        //animades = new List<Pe�a>() { pe�a };
        animades = new List<Pe�a>();

        for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
        {
            visualitzacions.ReaccioVeina(pe�a.VeinsPe�a[v]);
            //actualitzar.Play(pe�a.VeinsPe�a[v].Parent);
            //animades.Add(pe�a.VeinsPe�a[v]);
        }

        perComprovar = new List<Pe�a>() { pe�a };
        List<Pe�a> comprovar = proximitat.GetPecesToComprovar(pe�a);
        for (int i = 0; i < comprovar.Count; i++)
        {
            if (!perComprovar.Contains(comprovar[i])) perComprovar.Add(comprovar[i]);
        }
        //perComprovar.AddRange(proximitat.GetPecesToComprovar(pe�a));

        proximitat.Process(perComprovar, Repoblacio, true);
    }




    void Repoblacio(List<Pe�a> comprovades, List<Proximitat.Canvis> canviades)
    {
        this.canviades = canviades;
        canviadesPe�a = new List<Pe�a>();
        for (int i = 0; i < canviades.Count; i++)
        {
            canviadesPe�a.Add(canviades[i].Pe�a);
        }
        repoblar.Proces(comprovades, WFC);
    }



    void WFC()
    {
        if (canviades != null && canviades.Count > 0 || pe�a.EstatIgualA(riu))
        {
            wfc.Iniciar_WFC(pe�a, canviadesPe�a, Produir);
        }
        else
        {
            Produir();
        }
    }

    void Produir()
    {
        Animar();
        CrearRanures();
        produccio.Process(Guardar);
    }


    void Animar() {
        /*if (canviades.Contains(pe�a))
        {
            visualitzacions.CanviarEstat(pe�a);
            for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
            {
                actualitzar.Play(pe�a.VeinsPe�a[v].Parent);
            }
        }*/

        for (int c = 0; c < canviades.Count; c++)
        {
            Debug.LogError($"Canviada: {canviades[c]}");
            if (animades.Contains(canviades[c].Pe�a))
                continue;

            visualitzacions.CanviarEstat(canviades[c].Pe�a);
            //actualitzar.Play(canviades[c].Parent);
            Debug.LogError($"Animar: {canviades[c].Pe�a}");
            animades.Add(canviades[c].Pe�a);
            visualitzacions.GuanyarExperienciaProximitat(canviades[c].Experiencia);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            for (int v = 0; v < canviades[c].Pe�a.VeinsPe�a.Count; v++)
            {
                if (animades.Contains(canviades[c].Pe�a.VeinsPe�a[v]))
                    continue;

                visualitzacions.ReaccioVeina(canviades[c].Pe�a.VeinsPe�a[v]);
                //actualitzar.Play(canviades[c].VeinsPe�a[v].Parent);
                Debug.LogError($"Animar: {canviades[c].Pe�a}");
                animades.Add(canviades[c].Pe�a.VeinsPe�a[v]);
            }
        }
    }

    void CrearRanures()
    {
        foreach (var coodVei in Grid.Instance.VeinsCoordenades(pe�a.Coordenades))
        {
            //Grid.Instance.CrearRanura(coodVei);
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(0.75f, Grid.Instance.CrearRanura, coodVei);
            //CrearRanura(coodVei);
        }
    }

    void Guardar()
    {
        //Aix� tamb� hauria de ser un proces.
        Debug.LogError($"Actualitzar {animades.Count}");
        for (int i = 0; i < animades.Count; i++)
        {
            Debug.LogError($"Actualitzar {animades[i].Coordenades}");
        }
        save.Add(pe�a, grups);
        save.Actualitzar(perComprovar, grups);



        //Aix� ha de ser un altre proces a part.
        Grid.Instance.Dimensionar(pe�a);












        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", pe�a);
        resoldre.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
