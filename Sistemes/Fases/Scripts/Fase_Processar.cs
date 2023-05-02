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
    [SerializeScriptableObject] [SerializeField] WaveFunctionColpaseScriptable wfc;
    [SerializeScriptableObject] [SerializeField] Prediccio prediccio;
    [SerializeScriptableObject] [SerializeField] Grups grups;
    [SerializeScriptableObject] [SerializeField] Proximitat proximitat;
    [SerializeScriptableObject] [SerializeField] Repoblar repoblar;
    [SerializeScriptableObject] [SerializeField] Produccio produccio;
    [SerializeScriptableObject] [SerializeField] SaveHex save;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase resoldre;

    [Apartat("ANIMACIONS")]
    [SerializeField] Visualitzacions visualitzacions;

    [Apartat("RIU")]
    [SerializeField] Estat riu;

    //PRIVADES
    float startTime;
    Peça peça;
    List<Peça> perComprovar;
    List<Proximitat.Canvis> canviades;
    List<Peça> canviadesPeça;
    List<Peça> animades;


    public override void FaseStart()
    {
        peça = (Peça)arg;
        peça.Parent.localPosition = Vector3.up * 20;

        startTime = Time.realtimeSinceStartup;
        Agrupar();
    }


    void Agrupar()
    {

        prediccio.FinalitzacioForçada();
        grups.Interrompre();
        grups.Agrupdar(grups.Grup, peça, WFC_Inicial);
    }

    
    void WFC_Inicial() //Primer WFC per la peça inicial.
    {
        if (!peça.EstatIgualA(riu))
            wfc.Iniciar_WFC(peça, new List<Peça>(), Proximitat);
        else
            Proximitat();

    }

    void Proximitat()
    {
        Debug.LogError($"Colocar peça {peça.name}");
        visualitzacions.Colocar(peça);
        animades = new List<Peça>();

        for (int v = 0; v < peça.VeinsPeça.Count; v++)
        {
            visualitzacions.ReaccioVeina(peça.VeinsPeça[v]);
        }

        perComprovar = new List<Peça>() { peça };
        List<Peça> comprovar = proximitat.GetPecesToComprovar(peça); //Et dona totes les peces comprovables al voltant de la que has colocat, tinguent en compte grups, camins, ports, etc...

        for (int i = 0; i < comprovar.Count; i++)
        {
            if (!perComprovar.Contains(comprovar[i])) perComprovar.Add(comprovar[i]);
        }

        //proximitat.Process(perComprovar, Repoblacio, true);
        proximitat.ProcessReceptes(perComprovar, Repoblacio, true);
    }




    void Repoblacio(List<Peça> comprovades, List<Proximitat.Canvis> canviades)
    {
        this.canviades = canviades;
        canviadesPeça = new List<Peça>();
        for (int i = 0; i < canviades.Count; i++)
        {
            canviadesPeça.Add(canviades[i].Peça);
        }
        repoblar.Proces(comprovades, WFC);
    }



    void WFC()
    {
        if (canviades != null && canviades.Count > 0 || peça.EstatIgualA(riu))
        {
            wfc.Iniciar_WFC(peça, canviadesPeça, Produir);
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
        for (int c = 0; c < canviades.Count; c++)
        {
            Debug.LogError($"Canviada: {canviades[c]}");
            if (animades.Contains(canviades[c].Peça))
                continue;

            visualitzacions.CanviarEstat(canviades[c].Peça);
            Debug.LogError($"Animar: {canviades[c].Peça}");
            animades.Add(canviades[c].Peça);
            visualitzacions.GuanyarExperienciaProximitat(canviades[c].Experiencia);
        }

        for (int c = 0; c < canviades.Count; c++)
        {
            for (int v = 0; v < canviades[c].Peça.VeinsPeça.Count; v++)
            {
                if (animades.Contains(canviades[c].Peça.VeinsPeça[v]))
                    continue;

                visualitzacions.ReaccioVeina(canviades[c].Peça.VeinsPeça[v]);
                Debug.LogError($"Animar: {canviades[c].Peça}");
                animades.Add(canviades[c].Peça.VeinsPeça[v]);
            }
        }
    }

    void CrearRanures()
    {
        foreach (var coodVei in Grid.Instance.VeinsCoordenades(peça.Coordenades))
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(0.75f, Grid.Instance.CrearRanura, coodVei);
        }
    }

    void Guardar()
    {
        //Això hauria de ser un proces.
        Debug.LogError($"Actualitzar {animades.Count}");
        for (int i = 0; i < animades.Count; i++)
        {
            Debug.LogError($"Actualitzar {animades[i].Coordenades}");
        }
        save.Add(peça, grups);
        save.Actualitzar(perComprovar, grups);


        Grid.Instance.Dimensionar(peça);


        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", peça);
        resoldre.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
