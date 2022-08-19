using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Grups")]
public class Grups : ScriptableObject
{
    //*************************************************************************************************
    //FALTA: Que al buscar grups, agafi els grups de les peces adjacents a la pe�a que proves. No tots
    //*************************************************************************************************



    [SerializeField] List<Grup> grups;
    //[SerializeField] List<Grup> pobles;

    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] Estat casa;
    [SerializeField] Estat cami;
    [SerializeField] Subestat port;
    [SerializeField] Estat aigua;
    //System.Action enFinalitzar;

    //public List<Grup> Pobles => pobles;
    public List<Grup> Grup => grups;

    //INTERN
    //Queue<Grup> pendents;
    System.Action<int> enFinalitzar;
    List<Pe�a> veinsIguals;
    List<Pe�a> veinsCaminables;
    bool agrupada = false;
    int primerGrup = 0;
    List<Pe�a> veins;
    List<Pe�a> veinsGrup;

    int index;

    void OnEnable()
    {
        grups = new List<Grup>();
        //pobles = new List<Grup>();
        Setup();
    }

    public void Agrupdar(Pe�a pe�a, System.Action<int> enFinalitzar)
    {
        if (pe�a == null)
            return;

        this.enFinalitzar = enFinalitzar;

        veins = pe�a.VeinsPe�a;



        //VEINS IGUALS
        if (veinsIguals == null) veinsIguals = new List<Pe�a>();
        else veinsIguals.Clear();


        //PRIMER INTENT PER AJUNTAR ESTATS IGUALS
        for (int v = 0; v < veins.Count; v++)
        {
            if (pe�a.EstatIgualA(veins[v].Estat)) veinsIguals.Add(veins[v]);
        }

        if (veinsIguals.Count > 0)
            IntentarAgrupar(pe�a, veinsIguals);
        else CrearNouGrup(pe�a);

        

        /*
        //SI ES CAMINABLE BUSCA UN CAMI VEI PER AJUNTARSI
        if (EscCaminable(pe�a))
        {
            veinsIguals.Clear();

            for (int v = 0; v < veins.Count; v++)
            {
                if (EsCami(veins[v])) veinsIguals.Add(veins[v]);
            }

            if (veinsIguals.Count > 0)
                IntentarAgrupar(pe�a, veinsIguals);
        }

        //SI ES CAMI, BUSCA TOTS ELS CAMINABLES PER AJUNTARLOS
        if (EsCami(pe�a))
        {
            for (int v = 0; v < veins.Count; v++)
            {
                if (EscCaminable(veins[v]))
                {
                    IntentarAgrupar(veins[v], new List<Pe�a>() { pe�a });
                }
            }
        }
        */

        //SET VEINS AND CONNECTATS.
        Grup grup = grups[pe�a.Grup];
        grup.SetVeins = TrobarVeins(grup);

       



        XS_Coroutine.StartCoroutine_Ending(0.1f, AgruparVeins);


       
        //enFinalitzar.Invoke(index);
        //proximitat.Add(pe�a, enFinalitzar);
    }

    void AgruparVeins()
    {
        //SET VEINS I CONNEXIONS DEL MEUS VEINS
        for (int i = 0; i < veins.Count; i++)
        {
            Grup vei = grups[veins[i].Grup];
            vei.SetVeins = TrobarVeins(vei);


        }

        //ESTABLIR CONNEXIONS.
        for (int i = 0; i < grups.Count; i++)
        {
            Connexions(grups[i]);
        }

        enFinalitzar.Invoke(index);
    }


    private void IntentarAgrupar(Pe�a pe�a, List<Pe�a> veins)
    {
        agrupada = false;
        primerGrup = 0;
        //*************************************************************************************************************
        //En comptes de mirar tots els grups, s'han de capturar abans els grups implicats, que serien els grups dels veins.
        //*************************************************************************************************************

        for (int g = 0; g < grups.Count; g++)
        {
            if (grups[g].Peces.Contains((Pe�a)veins[0]))
            {
                //aquest es el grup del primer vei igual.
                AfegirAGrup(pe�a, grups[g]);
                agrupada = true;
                primerGrup = g;
                veins.RemoveAt(0);
                break;
            }
        }

        if (agrupada)
        {
            if (veins.Count > 0)
            {
                for (int g = 0; g < grups.Count; g++)//Per cada grup
                {
                    for (int v = 0; v < veins.Count; v++)//Per cada vei.
                    {
                        if(g >= grups.Count)
                        {
                            Debug.Log("He ajuntat l'ultim grup de la llista, aixi que no cal que continui.");
                            continue;
                        }
                        //Aquest son els grups dels altres veins iguals
                        if (grups[g].Peces.Contains(veins[v]))//mira si el vei es part del grup.
                        {
                            if (primerGrup.Equals(g))
                                continue;

                            Debug.LogError($"Juntar grup {primerGrup} i {g}");
                            primerGrup = AjuntarGrups(grups[primerGrup], grups[g]);
                        }
                    }
                }
            }

        }
    }

    void CrearNouGrup(Pe�a pe�a)
    {
        if(grups == null)
        {
            grups = new List<Grup>();
            //pobles = new List<Grup>();
        }
        Grup tmp = new Grup(pe�a.Estat, new List<Pe�a>() { pe�a }, pe�a.EstatIgualA(casa));
        grups.Add(tmp);
        //if (tmp.EsPoble) pobles.Add(tmp);

        pe�a.Grup = grups.Count - 1;
    }

    void AfegirAGrup(Pe�a pe�a, Grup grup) 
    {
        if (!grup.Peces.Contains(pe�a))
        {
            grup.Peces.Add(pe�a);
            pe�a.Grup = grups.IndexOf(grup);
        }

        /*proximitat.Add(pe�a);
        for (int i = 0; i < grup.peces.Count; i++)
        {
            proximitat.Add(grup.peces[i]);
        }*/
    }

    int AjuntarGrups(Grup desti, Grup seleccionat)
    {
        Pe�a[] pecesTmp = seleccionat.Peces.ToArray();
        for (int i = 0; i < pecesTmp.Length; i++)
        {
            desti.Peces.Add(pecesTmp[i]);
        }
        //if (pobles.Contains(seleccionat)) pobles.Remove(seleccionat);
        grups.Remove(seleccionat);

        for (int g = 0; g < grups.Count; g++)
        {
            for (int p = 0; p < grups[g].Peces.Count; p++)
            {
                grups[g].Peces[p].Grup = g;
            }
        }

        /*int indexOfDesti = grups.IndexOf(desti);
        for (int i = 0; i < desti.Peces.Count; i++)
        {
            desti.Peces[i].Grup = indexOfDesti;
        }*/
        /*for (int i = 0; i < desti.peces.Count; i++)
        {
            proximitat.Add(desti.peces[i]);
        }*/



        return grups.IndexOf(desti);
    }

    /*public Grup GrupOf(Pe�a pe�a)
    {
        int index = 0;
        for (int i = 0; i < grups.Count; i++)
        {
            if (grups[i].peces.Contains(pe�a))
            {
                index = i;
                break;
                
            }
        }
        return grups[index];
    }*/

    public List<Pe�a> Peces(Pe�a pe�a) => grups[pe�a.Grup].Peces;
    public List<Pe�a> Peces(int index) => grups[index].Peces;

    public List<Pe�a> Veins(Pe�a pe�a) => Veins(grups[pe�a.Grup]);
    public List<Pe�a> Veins(int indexGrup) => Veins(grups[indexGrup]);
    public List<Pe�a> Veins(Grup grup) => grup.Veins;

    void Connexions(Grup grup)
    {
        //List<int> connexions = new List<int>();
        //if (grup.connexions != null) connexions = grup.connexions;

        if (!grup.Peces[0].EstatIgualA(casa)) 
        {
            if (grup.connexions != null) grup.connexions = null;    
            return;
        }

        //Ell sempre estar� connectat amb si mateix.
        if (grup.connexions == null) grup.connexions = new List<int>();
        if (!grup.connexions.Contains(grups.IndexOf(grup))) grup.connexions.Add(grups.IndexOf(grup));

        //**************************************************************************************************************
        //Una vegada aixo funcioni, per optimitzar, cada com que un grup "canvii" es marca com a modificat. Quan s'acaba el proces es posa tot a false.
        //Aixi nom�s comprovaria els grups que s'han podificat. Aixo te sentit??? no ho he acabat de pensar del tot....
        //**************************************************************************************************************
        //Ah! i a la primera busca, millor guardar el que trobi per no tornar a fer el mateix, grup.Veins .EstatIgualA... bla bla. ok?
        //**************************************************************************************************************
        //Busquem cases veines.

        List<Pe�a> cases = new List<Pe�a>();
        List<Pe�a> camins = new List<Pe�a>();
        List<Pe�a> veinsCami;
        List<Pe�a> ports = new List<Pe�a>();
        List<Pe�a> altresPorts = new List<Pe�a>();
        List<Pe�a> veinsPort;
        List<Pe�a> mar;
        //Capturar cases, camins i ports
        for (int i = 0; i < grup.Veins.Count; i++)
        {
            if (grup.Veins[i].EstatIgualA(casa)) cases.Add(grup.Veins[i]);
            else if (grup.Veins[i].EstatIgualA(cami)) camins.Add(grup.Veins[i]);
            if (grup.Veins[i].SubestatIgualA(port)) ports.Add(grup.Veins[i]);
        }

        //Comprovar cases
        for (int i = 0; i < cases.Count; i++)
        {
            Debug.LogError("Add - CASA");
            AddConnexio(grup, cases[i]);
            //if (!connexions.Contains(cases[i].Grup)) connexions.Add(cases[i].Grup);
        }

        //Comprovar pobles connectats amb veins.
        for (int i = 0; i < camins.Count; i++)
        {
            veinsCami = grups[camins[i].Grup].Veins;
            for (int c = 0; c < veinsCami.Count; c++)
            {
                if (veinsCami[c].EstatIgualA(casa)) //Si troba un poble enganxat al cami, el connecta.
                {
                    Debug.LogError("Add - CASA >>> CAMI >>> CASA");
                    AddConnexio(grup, veinsCami[c]);
                    //if (!connexions.Contains(veinsCami[c].Grup)) connexions.Add(veinsCami[c].Grup);
                }
                else if (veinsCami[c].SubestatIgualA(port)) //Si troba un port enganxat el cami, el guarda com a port, per tractarlo al final.
                {
                    //Debugar.LogError("Add port a trav�s de cami");
                    //AddConnexio(ref connexions, grup, veinsCami[c]);
                    if (!ports.Contains(veinsCami[c])) ports.Add(veinsCami[c]);
                }
            }
        }


        Debug.LogError($"{ports.Count} ports");
        //Comprova els ports trobats (primer els enganxats i despres els solitaris.
        for (int p = 0; p < ports.Count; p++)
        {
            mar = MarIVoltants(ports[p].VeinsPe�a);

            Debug.LogError($"{mar.Count} peces de mar.");
            //Ports vora el mar
            for (int m = 0; m < mar.Count; m++)
            {
                if (mar[m].SubestatIgualA(port) && mar[m] != ports[p]) 
                {
                    Debug.LogError($"{mar[m].name} es un port");
                    if (!altresPorts.Contains(mar[m])) altresPorts.Add(mar[m]);
                }
            }

            //Faig una primera comprovacio dels ports, per saber quins tenen un poble enganxat.
            for (int ap = 0; ap < altresPorts.Count; ap++)
            {
                veinsPort = altresPorts[ap].VeinsPe�a;
                for (int vp = 0; vp < veinsPort.Count; vp++)
                {
                    if (veinsPort[vp].EstatIgualA(casa)) 
                    {
                        Debug.LogError("Add - CASA >>> PORT >>> MAR >>> PORT >>> CASA");
                        AddConnexio(grup, veinsPort[vp]);
                        //if (!connexions.Contains(veinsPort[vp].Grup)) connexions.Add(veinsPort[vp].Grup); //Una poblacio al costat del port d'arribada.
                    } 
                }
            }

            //Faig una segona comprovacio pels que nom�s hi arriba un cami, i trobar tots els pobles d'aquell cam�.
            for (int ap = 0; ap < altresPorts.Count; ap++)
            {
                veinsPort = altresPorts[ap].VeinsPe�a;
                for (int vp = 0; vp < veinsPort.Count; vp++)
                {
                    if (veinsPort[vp].EstatIgualA(cami))
                    {
                        veinsCami = grups[veinsPort[vp].Grup].Veins;
                        for (int c = 0; c < veinsCami.Count; c++)
                        {
                            if (veinsCami[c].EstatIgualA(casa))
                            {
                                Debug.LogError("Add - CASA >>> PORT >>> MAR >>> PORT >>> CAMI >>> CASA");
                                AddConnexio(grup, veinsCami[c]);
                                //if (!connexions.Contains(veinsCami[c].Grup)) connexions.Add(veinsCami[c].Grup);
                            }
                        }
                    }
                }
            }

           
        }



        //Comprova els ports llunyans i les rutes que porten a ciutats amb ports.


        //Comprova els ports llunyans i les rutes que porten a ciutats amb ports.


        //Comprova els ports llunyans i les rutes que porten a ports solitaris.

        //Comprovar els ports propers.


        /*for (int v = 0; v < grup.Veins.Count; v++)
        {
            if (grup.Veins[v].EstatIgualA(casa))//Si el vei es una casa, segurament ser� que jo soc una casa que s'ha convertit en un productor. I aquesta casa representa el poble on ja estava.
            {
                if (!connexions.Contains(grup.Veins[v].Grup)) connexions.Add(grup.Veins[v].Grup);
            }
            else if(grup.Veins[v].EstatIgualA(cami)) //Buscar una casa al llarg del cami.
            if (veins[v].EstatIgualA(cami))
            {
                List<Pe�a> veinsDelCami = Veins(veins[v]);
                tmp.AddRange(veinsDelCami);
            }
            List<Pe�a> veins = grup.Peces[g].VeinsPe�a;
            for (int v = 0; v < veins.Count; v++)
            {

            }
        }*/

        //return connexions;
    }

    void AddConnexio( Grup origen, Pe�a desti)
    {

        Debug.LogError($"From {grups.IndexOf(origen)}({grups[grups.IndexOf(origen)].Peces[0].name}) >>> to >>> {desti.Grup}({desti.name})");

        if (!origen.connexions.Contains(desti.Grup)) origen.connexions.Add(desti.Grup);
        else Debug.LogError("Ja connectat");

        if (grups[desti.Grup].connexions == null) grups[desti.Grup].connexions = new List<int>();
        if (!grups[desti.Grup].connexions.Contains(grups.IndexOf(origen))) grups[desti.Grup].connexions.Add(grups.IndexOf(origen));
        else Debug.LogError("Ja connectat");
    }

    List<Pe�a> MarIVoltants(List<Pe�a> voltantPort)
    {
        List<Pe�a> mar = new List<Pe�a>();
        for (int vp = 0; vp < voltantPort.Count; vp++)
        {
            if (voltantPort[vp].EstatIgualA(aigua))  //He trobat el mar 
            {
                mar.AddRange(grups[voltantPort[vp].Grup].Peces);
                mar.AddRange(grups[voltantPort[vp].Grup].Veins);
                break;
            }
        }
        return mar;
    }

    public List<Pe�a> VeinsAmbCami(Pe�a pe�a) => VeinsAmbCami(grups[pe�a.Grup]);
    public List<Pe�a> VeinsAmbCami(Grup grup) 
    {
        List<Pe�a> tmp = new List<Pe�a>();
        for (int g = 0; g < grup.Peces.Count; g++)
        {
            List<Pe�a> veins = grup.Peces[g].VeinsPe�a;
            for (int v = 0; v < veins.Count; v++)
            {
                if (!tmp.Contains(veins[v])) tmp.Add(veins[v]);

                if (veins[v].EstatIgualA(cami))
                {
                    List<Pe�a> veinsDelCami = Veins(veins[v]);
                    tmp.AddRange(veinsDelCami);
                }
            }
        }
        return tmp;
    }

    public List<Pe�a> VeinsAmbPort(Pe�a pe�a) //Es una combinacio de vein
    {
        //1 Si la casa toca un port. Salta al pas 3
        //2 O un cami arriba a un port
        //3 Busca veins aigua del port
        //4 de tots els veins del grup aigua
        //5 Busca un port que no sigui el mateix
        //6 I dels veins del port, busca una casa i agafa el seu poble. Torna al pas 5
        //7 Si no hi ha casa aprop de port, busca un cami.
        //8 per totes els veins del cami, busca una casa i agafa el seu poble. Torna al pas 7.
        //9 una vegada acabis amb totes els veins del poble, Torna al pas 5.
        return null;
    }
    List<Pe�a> TrobarVeins(Grup grup)
    {

        List<Pe�a> tmp = new List<Pe�a>();

        //Per cada una de les peces del grup
        for (int g = 0; g < grup.Peces.Count; g++)
        {
            //Per cada un dels veins de la pe�a del grup...
            List<Pe�a> veins = grup.Peces[g].VeinsPe�a;
            for (int v = 0; v < veins.Count; v++)
            {
                if (!tmp.Contains(veins[v])) //Si no l'he agafat encara
                {
                    tmp.Add(veins[v]);
                    /* if (!grup.Peces.Contains(veins[v])) tmp.Add(veins[v]); //Si no forma part del grup, es ve�
                     else //Si forma part del grup
                     {
                         if (grup.Estat == cami && !veins[v].EstatIgualA(cami)) tmp.Add(veins[v]); //Si formes part del cami pero no ets un cami, tambe contes com a vei.
                     }
                    */
                }
            }
            
        }

        return tmp;
    }

    public void CrearGrups_FromLoad(List<Pe�a> peces)
    {
        grups = new List<Grup>();
        //pobles = new List<Grup>();

        //Comprovar el numero de grups que es necessiten.
        int indexMesAlt = 0;
        for (int i = 0; i < peces.Count; i++)
        {
            if (indexMesAlt < peces[i].Grup) indexMesAlt = peces[i].Grup;
        }

        //Crear tots els grups de cop;
        //Grup[] tmp = new Grup[indexMesAlt + 1];
        //grups = new List<Grup>(tmp);
        //Debug.LogError($"{grups.Count} grups");

        for (int i = 0; i < indexMesAlt + 1; i++)
        {
            grups.Add(new Grup());
        }

        for (int i = 0; i < peces.Count; i++)
        {
            grups[peces[i].Grup].Set(peces[i], casa);
            if (peces[i].EstatIgualA(casa))
            {
                //pobles.Add(grups[peces[i].Grup]);
            }
        }
        for (int i = 0; i < grups.Count; i++)
        {
            List<Pe�a> veins = TrobarVeins(grups[i]);
            grups[i].SetVeins = veins;
        }

        //grups = new List<Grup>(tmp);
        /*for (int i = 0; i < peces.Count; i++)
        {
            if(grups.Count < peces[i].Grup + 1)
            {
                Debug.LogError($"Crear {(peces[i].Grup + 1) - grups.Count} nous grups.");
                for (int c = 0; c < (peces[i].Grup + 1) - grups.Count; c++)
                {
                    grups.Add(new Grup());
                }
            }

            Debug.LogError($"Add {peces[i]} al grup {peces[i].Grup}");
            grups[peces[i].Grup].Set(peces[i], casa);
        }*/
    }


    private void OnValidate() => Setup();
    protected virtual void Setup()
    {
       
    }
}



[System.Serializable]
public class Grup : System.Object
{
    public Grup() { }
    public Grup(Estat estat, List<Pe�a> peces, bool esPoble)
    {
        this.estat = estat;
        this.peces = peces;
        //this.esPoble = esPoble;
    }
    [SerializeField] Estat estat;
    [SerializeField] List<Pe�a> peces;
    //bool esPoble = false;
    [SerializeField] List<Pe�a> veins;
    public List<int> connexions;

    public Estat Estat => estat;
    public List<Pe�a> Peces => peces;
    //public bool EsPoble => esPoble;
    public List<Pe�a> Veins => veins;


    public void Set(Pe�a pe�a, Estat casa)
    {
        if(estat == null)
        {
            estat = pe�a.Estat;
            //esPoble = pe�a.EstatIgualA(casa);
        }
        if (peces == null) peces = new List<Pe�a>();
        peces.Add(pe�a);
    }
    public List<Pe�a> SetVeins { set => veins = value; }

}
