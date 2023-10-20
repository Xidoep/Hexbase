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

    [Apartat("SEG�ENT FASE")]
    [SerializeField] Fase resoldre;

    //[Apartat("ANIMACIONS")]
    //[SerializeField] Visualitzacions visualitzacions;

    [Apartat("RIU")]
    [SerializeField] EstatColocable riu;

    //PRIVADES
    float startTime;
    Pe�a pe�a;
    List<Pe�a> perComprovar;
    List<Pe�a> comprovades;
    List<Proximitat.Canvis> canviades;
    List<Pe�a> canviadesPe�a;
    //List<Pe�a> animades;
    List<Pe�a> recreades;


    System.Action<Transform, List<Pe�a>> enColocar;
    System.Action<Transform> enCanviarEstat;
    System.Action<Transform> enCanviarEstatVeins;

    public System.Action<Transform, List<Pe�a>> EnColocar { get => enColocar; set => enColocar = value; }
    public System.Action<Transform> EnCanviarEstat { get => enCanviarEstat; set => enCanviarEstat = value; }
    public System.Action<Transform> EnCanviarEstatVeins { get => enCanviarEstatVeins; set => enCanviarEstatVeins = value; }

    public override void FaseStart()
    {
        pe�a = (Pe�a)arg;
        pe�a.Parent.localPosition = Vector3.up * 20;

        startTime = Time.realtimeSinceStartup;
        Agrupar();
    }


    void Agrupar()
    {

        prediccio.FinalitzacioFor�ada();
        grups.Interrompre();
        grups.Agrupdar(grups.Grup, pe�a, WFC_Inicial);
    }

    
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

       // animades = new List<Pe�a>();
        /*visualitzacions.Colocar(pe�a);
        for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
        {
            visualitzacions.Colocar_ReaccioVei(pe�a.VeinsPe�a[v]);
        }*/

        perComprovar = new List<Pe�a>() { pe�a };
        List<Pe�a> comprovar = proximitat.GetPecesToComprovar(pe�a); //Et dona totes les peces comprovables al voltant de la que has colocat, tinguent en compte grups, camins, ports, etc...

        for (int i = 0; i < comprovar.Count; i++)
        {
            if (!perComprovar.Contains(comprovar[i])) perComprovar.Add(comprovar[i]);
        }

        //proximitat.Process(perComprovar, Repoblacio, true);
        proximitat.ProcessReceptes(perComprovar, Repoblacio, true);

        //Debug.Log($"pe�a == null ? {pe�a == null}");
        //Debug.Log($"pe�a.Parent == null ? {pe�a.Parent == null}");
        //Debug.Log($"pe�a.VeinsPe�a == null ? {pe�a.VeinsPe�a == null}");

        if (pe�a != null)
        {
            pe�a.CrearTilesFisics();
            enColocar?.Invoke(pe�a.Parent, pe�a.VeinsPe�a);
        }

        this.comprovades = perComprovar;
        for (int i = 0; i < perComprovar.Count; i++)
        {
            perComprovar[i].AmagarInformacio();
        }
    }




    void Repoblacio(List<Pe�a> comprovades, List<Proximitat.Canvis> canviades)
    {
        for (int i = 0; i < comprovades.Count; i++)
        {
            comprovades[i].AmagarInformacio();
        }

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
        Crear_i_Animar();
        CrearRanures();

        produccio.Process(Guardar_i_Acabar);
    }
    void Crear_i_Animar() 
    {
        recreades = new List<Pe�a>();

        //CREAR LA COLOCADA
        pe�a.CrearTilesFisics();
        recreades.Add(pe�a);

        //CREAR ELS VEINS DE LA COLOCADA, QUE NO HAGUIN CANVIAR
        for (int i = 0; i < pe�a.VeinsPe�a.Count; i++)
        {
            if (CanviadesContains(pe�a.VeinsPe�a[i]))
                continue;

            pe�a.VeinsPe�a[i].CrearTilesFisics();
            recreades.Add(pe�a.VeinsPe�a[i]);
        }

        //PRIMER: RECREAR LES PECES CANVIADES I LES PECES VEINES QUE NO HAGUIN SET RECREADES.
        //SEGON: ANIMAR LES PECES CANVIADES I LES PECES VEINES
        /*Debug.Log($"{canviadesPe�a.Count} Peces canviades!!!!");
        for (int c = 0; c < canviadesPe�a.Count; c++)
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(c * 0.5f, CrearIAnimar, canviadesPe�a[c]);
            //Aqui donar punts.
        }*/
        Debug.Log($"{canviades.Count} Peces canviades!!!!");
        for (int i = 0; i < canviades.Count; i++)
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(i * 0.5f, CrearIAnimar, canviades[i].Pe�a);      
            nivell.GuanyarExperiencia(canviades[i].Experiencia, (i * 0.5f) + 3);
            visualitzacions.PuntsFlotants((i * 0.5f) + 1, canviades[i].Pe�a.transform.position, canviades[i].Experiencia);
        }

        void CrearIAnimar(Pe�a pe�a)
        {
            if (!recreades.Contains(pe�a))
            {
                recreades.Add(pe�a);
                pe�a.CrearTilesFisics();
            }

            //visualitzacions.CanviarEstat(pe�a);
            enCanviarEstat(pe�a.Parent);

            for (int v = 0; v < pe�a.VeinsPe�a.Count; v++)
            {
                if (CanviadesContains(pe�a.VeinsPe�a[v]))
                    continue;

                //Aqu� si que pot ser que ja l'hagui crecreat, per tant, s'ha de mirar.
                if (!recreades.Contains(pe�a.VeinsPe�a[v]))
                {
                    recreades.Add(pe�a.VeinsPe�a[v]);
                    pe�a.VeinsPe�a[v].CrearTilesFisics();
                }
                enCanviarEstatVeins(pe�a.VeinsPe�a[v].Parent);
                //visualitzacions.CanviarEstat_ReaccioVei(pe�a.VeinsPe�a[v]);
            }
        }

        bool CanviadesContains(Pe�a pe�a)
        {
            //return canviadesPe�a.Contains(pe�a);

            for (int i = 0; i < canviades.Count; i++)
            {
                if (!Equals(pe�a, canviades[i].Pe�a))
                    continue;

                return true;
            }
            return false;
        }
    }

    

    void CrearRanures()
    {
        foreach (var coodVei in Grid.Instance.VeinsCoordenades(pe�a.Coordenades))
        {
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(0.75f, Grid.Instance.CrearRanura, coodVei);
        }
    }

    void Guardar_i_Acabar(List<Pe�a> proveides)
    {
        //Aix� hauria de ser un proces.
        /*Debug.LogError($"Actualitzar {animades.Count}");
        for (int i = 0; i < animades.Count; i++)
        {
            Debug.LogError($"Actualitzar {animades[i].Coordenades}");
        }*/
        save.Add(pe�a, grups);
        save.Actualitzar(perComprovar, grups);


        Grid.Instance.Dimensionar(pe�a);


        for (int i = 0; i < comprovades.Count; i++)
        {
            if (proveides.Contains(comprovades[i]))
                continue;

            comprovades[i].MostrarInformacio();
        }
        for (int i = 0; i < canviadesPe�a.Count; i++)
        {
            if (proveides.Contains(canviadesPe�a[i]))
                continue;

            canviadesPe�a[i].MostrarInformacio();
        }


        Debugar.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", pe�a);
        resoldre.Iniciar();
    }



    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
    }*/
}
