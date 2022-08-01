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

    public List<Grup> Pobles => pobles;


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
        pobles = new List<Grup>();
        Setup();
    }

    bool CamiICaminable(Pe�a jo, Pe�a tu) => (jo.EstatIgualA(cami) && caminables.Estats.Contains(tu.Estat)) || (tu.EstatIgualA(cami) && caminables.Estats.Contains(jo.Estat));
    bool EsCami(Pe�a pe�a) => pe�a.EstatIgualA(cami);
    bool EscCaminable(Pe�a pe�a) => caminables.Estats.Contains(pe�a.Estat);

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
                    if (!grup.Peces.Contains(veins[v])) tmp.Add(veins[v]); //Si no forma part del grup, es ve�
                    else //Si forma part del grup
                    {
                        if (grup.Estat == cami && !veins[v].EstatIgualA(cami)) tmp.Add(veins[v]); //Si formes part del camp, pero no ets un cami, tambe contes com a vei.
                        //if (grup.Estat == cami && caminables.Estats.Contains(veins[v].Estat)) tmp.Add(veins[v]); //Si jo soc un cam� i tu ets un caminable, ets ve�.
                    }
                }
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
        if (caminables == null) caminables = XS_Editor.LoadAssetAtPath<Detall_Tiles_Estats>("Assets/XidoStudio/Hexbase/Peces/Detalls/Tiles_CAMINABLES.asset");
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

    public Estat Estat => estat;
    public List<Pe�a> Peces => peces;
    public bool EsPoble => esPoble;
    public List<Pe�a> Veins => veins;

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

}
