using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Grups")]
public class Grups : ScriptableObject
{

    //*************************************************************************************************
    //FALTA: Que al buscar grups, agafi els grups de les peces adjacents a la pe�a que proves. No tots
    //*************************************************************************************************



    [SerializeField] List<Grup> grups;
    [SerializeField] List<Grup> pobles;

    [Linia]
    [Header("CASA")]
    [SerializeField] Estat casa;

    
    [Linia]
    [Header("CAMI")]
    [SerializeField] Estat cami;
    [Nota("Son els estats que permeten crear-hi un cam�", NoteType.Info)]
    [SerializeField] Detall_Tiles_Estats caminables;
    //System.Action enFinalitzar;

    [Linia]
    [Header("CONNEXIONS")]
    [SerializeField] Connexio[] connexions;


    public List<Grup> Pobles => pobles;


    //INTERN
    //Queue<Grup> pendents;
    List<Pe�a> veinsIguals;
    bool agrupada = false;
    int primerGrup = 0;
    Pe�a[] veins;
    List<Pe�a> veinsGrup;

    int index;

    void OnEnable()
    {
        grups = new List<Grup>();
        pobles = new List<Grup>();
    }

    public void Agrupdar(Pe�a pe�a, System.Action<int> enFinalitzar)
    {
        if (pe�a == null)
            return;

        veins = pe�a.VeinsPe�a;



        //VEINS IGUALS
        if (veinsIguals == null) veinsIguals = new List<Pe�a>();
        else veinsIguals.Clear();

        for (int v = 0; v < veins.Length; v++)
        {
            if (pe�a.Estat == veins[v].Estat) veinsIguals.Add(veins[v]);
        }

        if (veinsIguals.Count > 0)
        {
            IntentarAgrupar(pe�a);
        }
        else
        {
            CrearNouGrup(pe�a);
        }

        //SET VEINS AND CONNEXTATS.
        Grup grup = grups[pe�a.Grup];
        grup.SetVeins = TrobarVeins(grup);
        grup.SetConnectats = GetGrupsConnectats(grup, grup.Veins);

        //SET VEINS I CONNEXIONS DEL MEUS VEINS
        for (int i = 0; i < veins.Length; i++)
        {
            Grup vei = grups[veins[i].Grup];
            vei.SetVeins = TrobarVeins(vei);
            vei.SetConnectats = GetGrupsConnectats(vei, vei.Veins);

            /*for (int c = 0; c < connexions.Length; c++)
            {
                if(connexions[c].inici == grup.Estat && connexions[c].canal == vei.Estat)
                {
                    List<Pe�a> veinsCanal = vei.Veins;
                    for (int v = 0; v < veinsCanal.Count; v++)
                    {
                        grups[veinsCanal[i].Grup].SetConnectats = GetGrupsConnectats(grups[veinsCanal[i].Grup], grups[veinsCanal[i].Grup].Veins);
                    }
                }
            }*/
        }

        //SET LES CONNEXIONS DE CANNALS QUE ARRIBIN FINS A MI.
        /*for (int c = 0; c < connexions.Length; c++)
        {
            veinsGrup = grups[pe�a.Grup].Veins;
            for (int v = 0; v < veinsGrup.Count; v++)
            {
                if (veinsGrup[v].EstatIgualA(connexions[c].canal))
                {
                    //List<Pe�a> canal = grups[veinsGrup[v].Grup].Peces;
                    List<Pe�a> veinsCanal = grups[veinsGrup[v].Grup].Veins;
                    for (int i = 0; i < veinsCanal.Count; i++)
                    {
                        if(!grups[pe�a.Grup].Peces.Contains(veinsCanal[i]) && veinsCanal[i].EstatIgualA(connexions[c].inici))
                        //grups[veinsCanal[i].Grup].SetVeins = TrobarVeins(grups[veinsCanal[i].Grup]);
                        grups[veinsCanal[i].Grup].SetConnectats = GetGrupsConnectats(grups[veinsCanal[i].Grup], grups[veinsCanal[i].Grup].Veins);
                    }
                }
            }
        }*/
        /*if(grups[pe�a.Grup].Connectats != null && grups[pe�a.Grup].Connectats.Count > 0)
        {
            for (int i = 0; i < grups[pe�a.Grup].Connectats.Count; i++)
            {
                int connectat = grups[pe�a.Grup].Connectats[i];
                grups[connectat].SetConnectats = GetGrupsConnectats(grups[connectat], grups[connectat].Veins, connexios);
            }
        }*/
        
        //******************************************************************************************************************
        //Aixo s'utlitzava per ajuntar les peces caminables que toquen un cam�, perque formessin part del cami.
        //pero aixo crea conflictes amb la funcio Veins. Ja que no camptura per exemple una casa si un cami l'atravessa.
        //******************************************************************************************************************
        //CAMINABLES
        /*if (veinsIguals == null) veinsIguals = new List<Pe�a>();
        else veinsIguals.Clear();

        for (int v = 0; v < veins.Length; v++)
        {
            if (caminables.Estats.Contains(pe�a.Estat))
            {
                if (veins[v].Estat == cami) veinsIguals.Add(veins[v]);
            }
        }

        if (veinsIguals.Count > 0)
        {
            IntentarAgrupar(pe�a);
        }*/

        //CONVERTIR AIXO EN UN PROCES.
        //if (caminsSortida == null) caminsSortida = new List<Pe�a>();
        //else caminsSortida.Clear();

        //pendents = new Queue<Grup>();
        //pendents.Enqueue(grups[pe�a.Grup]);
        //Step(enFinalitzar);


       
        enFinalitzar.Invoke(index);
        //proximitat.Add(pe�a, enFinalitzar);
    }

    /*void Step(System.Action<int> enFinalitzar)
    {
        Grup grup = pendents.Dequeue();

        //List<Pe�a> veins = TrobarVeins(grup);
        //grup.SetVeins = veins;
        grup.SetConnectats = GetGrupsConnectats(grup.Veins);



        if (pendents.Count == 0)
        {
            //Debug.Log($"Agrupdar estat: {pe�a.name}");
            enFinalitzar.Invoke(index);
            return;
        }
    }*/

    List<int> GetGrupsConnectats(Grup grup, List<Pe�a> veins)
    {
        List<int> indexConnextats = new List<int>();
        for (int i = 0; i < connexions.Length; i++)
        {
            if (grup.Estat != connexions[i].inici)
                continue;

            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].EstatIgualA(connexions[i].canal))
                {
                    List<Pe�a> connectatsACami = Veins(veins[v].Grup);
                    for (int c = 0; c < connectatsACami.Count; c++)
                    {
                        if (!grup.Peces.Contains(connectatsACami[c]) && connectatsACami[c].EstatIgualA(connexions[i].objectiu))
                        {
                            indexConnextats.Add(connectatsACami[c].Grup);
                            //pendents.Enqueue(grups[connectatsACami[c].Grup]);
                        }
                    }
                }
            }

        }
        return indexConnextats;
    }

    private void IntentarAgrupar(Pe�a pe�a)
    {
        agrupada = false;
        primerGrup = 0;
        for (int g = 0; g < grups.Count; g++)
        {
            if (grups[g].Peces.Contains((Pe�a)veinsIguals[0]))
            {
                //aquest es el grup del primer vei igual.
                AfegirAGrup(pe�a, grups[g]);
                agrupada = true;
                primerGrup = g;
                veinsIguals.RemoveAt(0);
                break;
            }
        }

        if (agrupada)
        {
            if (veinsIguals.Count > 0)
            {
                for (int g = 0; g < grups.Count; g++)
                {
                    for (int v = 0; v < veinsIguals.Count; v++)
                    {
                        //Aquest son els grups dels altres veins iguals
                        if (grups[g].Peces.Contains(veinsIguals[v]))
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
            pobles = new List<Grup>();
        }
        Grup tmp = new Grup(pe�a.Estat, new List<Pe�a>() { pe�a }, pe�a.EstatIgualA(casa));
        grups.Add(tmp);
        if (tmp.EsPoble) pobles.Add(tmp);

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
        if (pobles.Contains(seleccionat)) pobles.Remove(seleccionat);
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

    public List<Pe�a> Peces(int index) => grups[index].Peces; 


    public List<Pe�a> Veins(int indexGrup) => Veins(grups[indexGrup]);
    public List<Pe�a> Veins(Grup grup) => grup.Veins;


    public bool EstaConnectat(int indexGrup) => grups[indexGrup].Connectats != null && grups[indexGrup].Connectats.Count > 0;
    public List<int> Connectats(int indexGrup) => grups[indexGrup].Connectats;
    List<Pe�a> TrobarVeins(Grup grup)
    {
        //if (veinsGrup == null) veinsGrup = new List<Pe�a>();
        //else veinsGrup.Clear();
        List<Pe�a> tmp = new List<Pe�a>();

        for (int g = 0; g < grup.Peces.Count; g++)
        {
            Pe�a[] veins = grup.Peces[g].VeinsPe�a;
            for (int v = 0; v < veins.Length; v++)
            {
                if (!tmp.Contains(veins[v]) && !grup.Peces.Contains(veins[v])) tmp.Add(veins[v]);
            }
            
        }

        return tmp;
    }

    public void CrearGrups_FromLoad(List<Pe�a> peces)
    {
        grups = new List<Grup>();
        pobles = new List<Grup>();

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
                pobles.Add(grups[peces[i].Grup]);
            }
        }
        for (int i = 0; i < grups.Count; i++)
        {
            List<Pe�a> veins = TrobarVeins(grups[i]);
            grups[i].SetVeins = veins;
            grups[i].SetConnectats = GetGrupsConnectats(grups[i], veins);
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

    [System.Serializable]
    public struct Connexio
    {
        public Estat inici;
        public Estat canal;
        public Estat objectiu;
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
        this.esPoble = esPoble;
    }
    Estat estat;
    [SerializeField] List<Pe�a> peces;
    bool esPoble = false;
    [SerializeField] List<Pe�a> veins;
    [SerializeField] List<int> connectats;

    public Estat Estat => estat;
    public List<Pe�a> Peces => peces;
    public bool EsPoble => esPoble;
    public List<Pe�a> Veins => veins;
    public List<int> Connectats => connectats;

    public void Set(Pe�a pe�a, Estat casa)
    {
        if(estat == null)
        {
            estat = pe�a.Estat;
            esPoble = pe�a.EstatIgualA(casa);
        }
        if (peces == null) peces = new List<Pe�a>();
        peces.Add(pe�a);
    }
    public List<Pe�a> SetVeins { set => veins = value; }
    public List<int> SetConnectats { set => connectats = value; }

}
