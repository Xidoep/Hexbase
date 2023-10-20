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
    [SerializeScriptableObject] [SerializeField] Nivell nivell;
    [SerializeScriptableObject] [SerializeField] Visualitzacions visualitzacions;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase resoldre;

    //[Apartat("ANIMACIONS")]
    //[SerializeField] Visualitzacions visualitzacions;

    [Apartat("RIU")]
    [SerializeField] EstatColocable riu;

    //PRIVADES
    float startTime;
    Peça peça;
    List<Peça> perComprovar;
    List<Peça> comprovades;
    List<Proximitat.Canvis> canviades;
    List<Peça> canviadesPeça;
    //List<Peça> animades;
    List<Peça> recreades;


    System.Action<Transform, List<Peça>> enColocar;
    System.Action<Transform> enCanviarEstat;
    System.Action<Transform> enCanviarEstatVeins;

    public System.Action<Transform, List<Peça>> EnColocar { get => enColocar; set => enColocar = value; }
    public System.Action<Transform> EnCanviarEstat { get => enCanviarEstat; set => enCanviarEstat = value; }
    public System.Action<Transform> EnCanviarEstatVeins { get => enCanviarEstatVeins; set => enCanviarEstatVeins = value; }

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

       // animades = new List<Peça>();
        /*visualitzacions.Colocar(peça);
        for (int v = 0; v < peça.VeinsPeça.Count; v++)
        {
            visualitzacions.Colocar_ReaccioVei(peça.VeinsPeça[v]);
        }*/

        perComprovar = new List<Peça>() { peça };
        List<Peça> comprovar = proximitat.GetPecesToComprovar(peça); //Et dona totes les peces comprovables al voltant de la que has colocat, tinguent en compte grups, camins, ports, etc...

        for (int i = 0; i < comprovar.Count; i++)
        {
            if (!perComprovar.Contains(comprovar[i])) perComprovar.Add(comprovar[i]);
        }

        //proximitat.Process(perComprovar, Repoblacio, true);
        proximitat.ProcessReceptes(perComprovar, Repoblacio, true);

        //Debug.Log($"peça == null ? {peça == null}");
        //Debug.Log($"peça.Parent == null ? {peça.Parent == null}");
        //Debug.Log($"peça.VeinsPeça == null ? {peça.VeinsPeça == null}");

        if (peça != null)
        {
            peça.CrearTilesFisics();
            enColocar?.Invoke(peça.Parent, peça.VeinsPeça);
        }

        this.comprovades = perComprovar;
        for (int i = 0; i < perComprovar.Count; i++)
        {
            perComprovar[i].AmagarInformacio();
        }
    }




    void Repoblacio(List<Peça> comprovades, List<Proximitat.Canvis> canviades)
    {
        for (int i = 0; i < comprovades.Count; i++)
        {
            comprovades[i].AmagarInformacio();
        }

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
        Crear_i_Animar();
        CrearRanures();

        produccio.Process(Guardar_i_Acabar);
    }
    void Crear_i_Animar() 
    {
        recreades = new List<Peça>();

        //CREAR LA COLOCADA
        peça.CrearTilesFisics();
        recreades.Add(peça);

        //CREAR ELS VEINS DE LA COLOCADA, QUE NO HAGUIN CANVIAR
        for (int i = 0; i < peça.VeinsPeça.Count; i++)
        {
            if (CanviadesContains(peça.VeinsPeça[i]))
                continue;

            peça.VeinsPeça[i].CrearTilesFisics();
            recreades.Add(peça.VeinsPeça[i]);
        }

        //PRIMER: RECREAR LES PECES CANVIADES I LES PECES VEINES QUE NO HAGUIN SET RECREADES.
        //SEGON: ANIMAR LES PECES CANVIADES I LES PECES VEINES
        /*Debug.Log($"{canviadesPeça.Count} Peces canviades!!!!");
        for (int c = 0; c < canviadesPeça.Count; c++)
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(c * 0.5f, CrearIAnimar, canviadesPeça[c]);
            //Aqui donar punts.
        }*/
        Debug.Log($"{canviades.Count} Peces canviades!!!!");
        for (int i = 0; i < canviades.Count; i++)
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(i * 0.5f, CrearIAnimar, canviades[i].Peça);      
            nivell.GuanyarExperiencia(canviades[i].Experiencia, (i * 0.5f) + 3);
            visualitzacions.PuntsFlotants((i * 0.5f) + 1, canviades[i].Peça.transform.position, canviades[i].Experiencia);
        }

        void CrearIAnimar(Peça peça)
        {
            if (!recreades.Contains(peça))
            {
                recreades.Add(peça);
                peça.CrearTilesFisics();
            }

            //visualitzacions.CanviarEstat(peça);
            enCanviarEstat(peça.Parent);

            for (int v = 0; v < peça.VeinsPeça.Count; v++)
            {
                if (CanviadesContains(peça.VeinsPeça[v]))
                    continue;

                //Aquí si que pot ser que ja l'hagui crecreat, per tant, s'ha de mirar.
                if (!recreades.Contains(peça.VeinsPeça[v]))
                {
                    recreades.Add(peça.VeinsPeça[v]);
                    peça.VeinsPeça[v].CrearTilesFisics();
                }
                enCanviarEstatVeins(peça.VeinsPeça[v].Parent);
                //visualitzacions.CanviarEstat_ReaccioVei(peça.VeinsPeça[v]);
            }
        }

        bool CanviadesContains(Peça peça)
        {
            //return canviadesPeça.Contains(peça);

            for (int i = 0; i < canviades.Count; i++)
            {
                if (!Equals(peça, canviades[i].Peça))
                    continue;

                return true;
            }
            return false;
        }
    }

    

    void CrearRanures()
    {
        foreach (var coodVei in Grid.Instance.VeinsCoordenades(peça.Coordenades))
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(0.75f, Grid.Instance.CrearRanura, coodVei);
        }
    }

    void Guardar_i_Acabar(List<Peça> proveides)
    {
        //Això hauria de ser un proces.
        /*Debug.LogError($"Actualitzar {animades.Count}");
        for (int i = 0; i < animades.Count; i++)
        {
            Debug.LogError($"Actualitzar {animades[i].Coordenades}");
        }*/
        save.Add(peça, grups);
        save.Actualitzar(perComprovar, grups);


        Grid.Instance.Dimensionar(peça);


        for (int i = 0; i < comprovades.Count; i++)
        {
            if (proveides.Contains(comprovades[i]))
                continue;

            comprovades[i].MostrarInformacio();
        }
        for (int i = 0; i < canviadesPeça.Count; i++)
        {
            if (proveides.Contains(canviadesPeça[i]))
                continue;

            canviadesPeça[i].MostrarInformacio();
        }


        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", peça);
        resoldre.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
