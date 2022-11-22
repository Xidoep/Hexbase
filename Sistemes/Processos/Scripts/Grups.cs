using System.Linq;
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
    List<Grup> grupsPendents;
    List<string> idsPobles;

    //Queue<Grup> pendents;
    System.Action enFinalitzar;
    List<Pe�a> veinsIguals;
    List<Pe�a> veinsCaminables;
    bool agrupada = false;
    int primerGrup = 0;
    List<Pe�a> veinsPe�a;
    List<Pe�a> veinsGrup;
    int index;
    Grup grupActual;
    List<Grup> grupsVeinsPe�a;
    Grup buscat;

    void OnEnable()
    {
        Resetejar();
        //pobles = new List<Grup>();
        //Setup();
    }

    //AGRUPA LA PE�A QUE AS COLOCAT
    public void Agrupdar(Pe�a pe�a, System.Action enFinalitzar)
    {
        if (pe�a == null)
            return;

        this.enFinalitzar = enFinalitzar;
       
        if (veinsIguals == null) veinsIguals = new List<Pe�a>();
        else veinsIguals.Clear();
        if (grupsVeinsPe�a == null) grupsVeinsPe�a = new List<Grup>();
        else grupsVeinsPe�a.Clear();



        //BUSCAR VEINS DE LA PE�A IGUALS
        veinsPe�a = pe�a.VeinsPe�a;
        
        for (int v = 0; v < veinsPe�a.Count; v++)
        {
            if (pe�a.EstatIgualA(veinsPe�a[v].Estat)) veinsIguals.Add(veinsPe�a[v]);
            grupsVeinsPe�a.Add(GrupByPe�a(veinsPe�a[v]));
        }
        Debugar.LogError($"Connectar {grupsVeinsPe�a.Count} veins iguals");


        //INTENTAR AGRUPAR O CREAR GRUP NOU
        if (veinsIguals.Count > 0)
            grupActual = IntentarAgrupar(pe�a, veinsIguals);
        else grupActual = CrearNouGrup(pe�a);


        //TROBAR VEINS DEL GRUP
        grupActual.TrobarVeins();
  

        //PREPARAR GRUP PER ACTUALITZAR
        grupsPendents = new List<Grup>() { grupActual };



        //AFGEIR A LA LLISTA ELS GRUPS VEINS
        for (int i = 0; i < grupActual.Veins.Count; i++)
        {
            Grup grupVei = GrupByPe�a(grupActual.Veins[i]);
            if (!grupsPendents.Contains(grupVei)) 
            {
                grupVei.TrobarVeins();
                grupsPendents.Add(grupVei);
            } 
        }


        //NETEJAR LES CONNEXIONS QUE NO EXISTEIXEN
        idsPobles = new List<string>();
        for (int i = 0; i < grups.Count; i++)
        {
            if (!grups[i].EsPoble)
                continue;

            idsPobles.Add(grups[i].Id);
        }
        for (int g = 0; g < grups.Count; g++)
        {
            if (!grups[g].EsPoble)
                continue;

            if (grups[g].connexionsId == null) grups[g].connexionsId = new List<string>();
            for (int c = 0; c < grups[g].connexionsId.Count; c++)
            {
                if (!idsPobles.Contains(grups[g].connexionsId[c])) grups[g].connexionsId.RemoveAt(c);
            }
        }

        //POROCES DE CONNEXIO
        Step();

        //CONNECTAR
        /*for (int i = 0; i < pendents.Count; i++)
        {
            Connexions(pendents[i]);
        }*/

        //enFinalitzar.Invoke();

        //Old...
        /*
        //TROBAR VEINS DEL GRUP DE LA PE�A ACABADA DE COLOCAR
        Grup grupActual = grups[pe�a.Grup];
        grupActual.TrobarVeins(grups);


        //TROBAR ELS VEINS DELS GRUPS VEINS
        for (int i = 0; i < grupActual.Veins.Count; i++)
        {
            Grup grupVei = grups[grupActual.Veins[i].Grup];
            grupVei.TrobarVeins(grups);
            if (!perActualitzar.Contains(grupVei)) perActualitzar.Add(grupVei);
        }

        //
        List<Grup> perConnectar = new List<Grup>() { grupActual };
        //PER CADA GRUP VEI POBLE, CONNECTAR.
        for (int i = 0; i < grupActual.GrupVeins.Count; i++)
        {
            Grup g = GrupFromId(grupActual.GrupVeins[i]);
            if (!perActualitzar.Contains(g)) perActualitzar.Add(g);
        }
        */

        //CONNECTAR

        //


        //XS_Coroutine.StartCoroutine_Ending(0.1f, AgruparVeins);
    }

    void Step()
    {
        if(grupsPendents.Count == 0)
        {
            Debugar.LogError("FINALITZAT!");
            enFinalitzar.Invoke();

            return;
        }

        Connectar(grupsPendents[0]);
        grupsPendents.RemoveAt(0);

        XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }

    //AGRUPA 
    /*void AgruparVeins()
    {
        //SET VEINS I CONNEXIONS DEL MEUS VEINS
        

        //ESTABLIR CONNEXIONS.
        for (int i = 0; i < grups.Count; i++)
        {
            Connexions(grups[i]);
        }

        enFinalitzar.Invoke();
    }
    void Connectar()
    {

    }*/

    Grup IntentarAgrupar(Pe�a pe�a, List<Pe�a> veinsIguals)
    {
        agrupada = false;
        //primerGrup = 0;
        Grup elMeuGrup = null;

        for (int i = 0; i < veinsIguals.Count; i++)
        {
            if(elMeuGrup == null)
            {
                elMeuGrup = GrupByPe�a(veinsIguals[i]);
                AfegirAGrup(pe�a, elMeuGrup);
            }
            else
            {
                Grup altreGrup = GrupByPe�a(veinsIguals[i]);
                if (altreGrup == elMeuGrup)
                    continue;

                AjuntarGrups(elMeuGrup, altreGrup);
            }
        }

        return elMeuGrup;

        /*for (int g = 0; g < grups.Count; g++)
        {
            if (grups[g].Peces.Contains((Pe�a)veinsIguals[0]))
            {
                //aquest es el grup del primer vei igual.
                AfegirAGrup(pe�a, grups[g]);
                agrupada = true;
                //primerGrup = g;
                veinsIguals.RemoveAt(0);
                elMeuGrup = grups[g];
                break;
            }
        }


        if (agrupada)
        {
            if (veinsIguals.Count > 0)
            {
                for (int g = 0; g < grups.Count; g++)//Per cada grup
                {
                    for (int v = 0; v < veinsIguals.Count; v++)//Per cada vei.
                    {
                        if(g >= grups.Count)
                        {
                            Debug.Log("He ajuntat l'ultim grup de la llista, aixi que no cal que continui.");
                            continue;
                        }
                        //Aquest son els grups dels altres veins iguals
                        if (grups[g].Peces.Contains(veinsIguals[v]))//mira si el vei es part del grup.
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

        return elMeuGrup;

        */
        
        void AfegirAGrup(Pe�a pe�a, Grup grup)
        {
            if (!grup.Peces.Contains(pe�a))
            {
                grup.Peces.Add(pe�a);
                //pe�a.Grup = grup.Id;
            }

        }
        int AjuntarGrups(Grup desti, Grup seleccionat)
        {
            desti.Peces.AddRange(seleccionat.Peces);
            grups.Remove(seleccionat);

            /*for (int g = 0; g < grups.Count; g++)
            {
                for (int p = 0; p < grups[g].Peces.Count; p++)
                {
                    grups[g].Peces[p].Grup = g;
                }
            }*/
            return grups.IndexOf(desti);
        }
    }

    Grup CrearNouGrup(Pe�a pe�a)
    {
        if(grups == null) grups = new List<Grup>();

        Grup tmp = new Grup(pe�a.Estat, new List<Pe�a>() { pe�a }, casa);
        tmp.TrobarVeins();
        grups.Add(tmp);

        //pe�a.Grup = tmp.Id;
        return tmp;
    }

    public void Resetejar()
    {
        grups = new List<Grup>();
    }

    



    public List<Pe�a> Peces(Pe�a pe�a) => GrupByPe�a(pe�a).Peces;
    public List<Pe�a> Veins(Pe�a pe�a) => GrupByPe�a(pe�a).Veins;


    void Connectar(Grup grup)
    {
        if (!grup.EsPoble)
            return;

        //NETEJA GRUPS
        //if (grup.connexions == null) grup.connexions = new List<int>() { };
        //if (!grup.connexions.Contains(grups.IndexOf(grup))) grup.connexions.Add(grups.IndexOf(grup));
        //grup.connexions = new List<int>() { grups.IndexOf(grup) };
        grup.connexionsId = new List<string>() { grup.Id };


        //CAPTURAR VEINS TIPUS: CASES, CAMINS I PORTS.
        if (grup.Cases == null) grup.Cases = new List<Pe�a>();
        if (grup.Camins == null) grup.Camins = new List<Pe�a>();
        if (grup.Ports == null) grup.Ports = new List<Pe�a>();

        for (int i = 0; i < grup.Veins.Count; i++)
        {
            if (grup.Veins[i].EstatIgualA(casa))
            {
                if (!grup.Cases.Contains(grup.Veins[i])) grup.Cases.Add(grup.Veins[i]);
            }
            else if (grup.Veins[i].EstatIgualA(cami)) 
            {
                if (!grup.Camins.Contains(grup.Veins[i])) grup.Camins.Add(grup.Veins[i]);
            }
            if (grup.Veins[i].SubestatIgualA(port)) 
            {
                if (!grup.Ports.Contains(grup.Veins[i])) grup.Ports.Add(grup.Veins[i]);
            } 
        }


        //****************************************
        //NO TE SENTIT! Si has d'ajuntar 2 cases de costat, ja s'hauran agrupat previementa abans i formaran part del mateix grup,
        //per tant aixo no podr� passar mai!!!
        //****************************************
        //AJUNTAR CASES
        for (int i = 0; i < grup.Cases.Count; i++)
        {
            Debugar.LogError("Add - CASA");
            AddConnexio(grup, grup.Cases[i]);
        }



        //AJUNTA POBLES CONNECTATS PER CAMINS I GUARDA PORTS
        List<Pe�a> veinsCami;
        for (int i = 0; i < grup.Camins.Count; i++)
        {
            //BUSCAR A TOTS ELS VEINS DEL CAM�
            veinsCami = GrupByPe�a(grup.Camins[i]).Veins;
            for (int c = 0; c < veinsCami.Count; c++)
            {
                if (veinsCami[c].EstatIgualA(casa) && GrupByPe�a(veinsCami[c]) != grup) //SI ES UNA CASA, i no pertany al meu grup, EL CONNECTO
                {
                    Debugar.LogError("CASA >>> CAMI >>> CASA");
                    AddConnexio(grup, veinsCami[c]);
                }
               
            }
            if (grup.Camins[i].SubestatIgualA(port)) //SI ES UN PORT, EL GUARDO
            {
                if (!grup.Ports.Contains(grup.Camins[i])) grup.Ports.Add(grup.Camins[i]);
            }
        }



        List<Pe�a> costes = new List<Pe�a>();
        List<Pe�a> altresPorts = new List<Pe�a>();
        List<Pe�a> veinsAltresPort;
        List<Pe�a> veinsCaminsAltresPorts;

        //AJUNTAR POBLES CONNECTATS PER PORTS
        for (int p = 0; p < grup.Ports.Count; p++)
        {
            //AGAFA LES COSTES DEL MAR QUE DONA AL PORT.
            List<Pe�a> veinsPort = grup.Ports[p].VeinsPe�a;
            for (int vp = 0; vp < veinsPort.Count; vp++)
            {
                if (veinsPort[vp].EstatIgualA(aigua))
                {
                    List<Pe�a> costa = GrupByPe�a(veinsPort[vp]).Veins;
                    for (int c = 0; c < costa.Count; c++)
                    {
                        if (!costes.Contains(costa[c])) costes.Add(costa[c]);
                    }
                }
            }

            //BUSCAR ALTRES PORTS QUE NO ESTIGUIN CONNECTATS PER UN CAMI
            for (int m = 0; m < costes.Count; m++)
            {
                if (costes[m].SubestatIgualA(port) //UN PORT
                    && costes[m] != grup.Ports[p]) //NO SIGUI EL MATEIX PORT
                {
                    if (!altresPorts.Contains(costes[m])) altresPorts.Add(costes[m]);
                }
            }

            
            for (int ap = 0; ap < altresPorts.Count; ap++)
            {
                //CONNECTAR ELS POBLES ENGANXATS AL PORT TROBAT
                veinsAltresPort = altresPorts[ap].VeinsPe�a;
                for (int vp = 0; vp < veinsAltresPort.Count; vp++)
                {
                    if (veinsAltresPort[vp].EstatIgualA(casa)) 
                    {
                        Debugar.LogError("CASA >>> PORT >>> MAR >>> PORT >>> CASA");
                        AddConnexio(grup, veinsAltresPort[vp]);
                    }
                }

                //CONNECTAR ELS POBLES QUE ARRIBEN AMB UN CAMI FINS AL PORT TROBAT
                for (int vap = 0; vap < veinsAltresPort.Count; vap++)
                {
                    if (veinsAltresPort[vap].EstatIgualA(cami))
                    {
                        veinsCaminsAltresPorts = GrupByPe�a(veinsAltresPort[vap]).Veins;
                        for (int c = 0; c < veinsCaminsAltresPorts.Count; c++)
                        {
                            if (veinsCaminsAltresPorts[c].EstatIgualA(casa))
                            {
                                Debugar.LogError("CASA >>> PORT >>> MAR >>> PORT >>> CAMI >>> CASA");
                                AddConnexio(grup, veinsCaminsAltresPorts[c]);
                            }
                        }
                    }
                }
            }

            

           
        }




        void AddConnexio(Grup elMeuGrup, Pe�a objectiu)
        {
            Grup grupObjectiu = GrupByPe�a(objectiu);
            if (grupObjectiu.Equals(elMeuGrup))
                return;

            Debugar.LogError($"Connectar ({grups[grups.IndexOf(elMeuGrup)].Peces[0].name}) amb  ({objectiu.name}");

            //CONNECTOR EL MEU GRUP AMB EL GRUP DE L'OBJECTIU
            //if (!elMeuGrup.connexions.Contains(objectiu.Grup)) elMeuGrup.connexions.Add(objectiu.Grup);
            if (!elMeuGrup.connexionsId.Contains(grupObjectiu.Id)) elMeuGrup.connexionsId.Add(grupObjectiu.Id);

            //SI L'OBJECTIU NO EM TE A MI, S'ENV� A PENDENTS
            //if (grups[objectiu.Grup].connexions == null) grups[objectiu.Grup].connexions = new List<int>();
            //if (!grups[objectiu.Grup].connexions.Contains(grups.IndexOf(elMeuGrup))) grupsPendents.Add(grups[objectiu.Grup]);

            if (grupObjectiu.connexionsId == null) grupObjectiu.connexionsId = new List<string>();
            if (!grupObjectiu.connexionsId.Contains(elMeuGrup.Id)) grupsPendents.Add(grupObjectiu);



        }


        /*List<Pe�a> MarIVoltants(List<Pe�a> voltantPort)
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
        }*/
    }




















    public List<Pe�a> VeinsAmbCami(Pe�a pe�a) 
    {
        List<Pe�a> tmp = new List<Pe�a>();
        Grup grup = GrupByPe�a(pe�a);
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
    public void CrearGrups_FromLoad(Grup nouGrup, Pe�a pe�a)
    {
        if (!grups.Contains(nouGrup)) 
        {
            nouGrup.Netejar();
            grups.Add(nouGrup);
        }

        nouGrup.Load(pe�a);
    }
    public void CrearGrups_FromLoad(List<SavedPe�a> peces)
    {
        grups = new List<Grup>();

        for (int p = 0; p < peces.Count; p++)
        {
            for (int g = 0; g < grups.Count; g++)
            {
                
            }
        }

        //Si no hi ha el grup amb la id, es crea (amb: id, poble i connexionsId)
        //i s'hi afageix la pe�a a peces.

        //Si ja est� creat, s'hi afageix a peces.

        //Quan s'haguin afegit tots, s'agafen les peces veines


        //pobles = new List<Grup>();

        //Comprovar el numero de grups que es necessiten.
        int indexMesAlt = 0;
        for (int i = 0; i < peces.Count; i++)
        {
            //if (indexMesAlt < peces[i].Grup) indexMesAlt = peces[i].Grup;
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
            //grups[peces[i].Grup].Load(peces[i], peces[i].Estat, casa);
        }

        for (int i = 0; i < grups.Count; i++)
        {
            grups[i].TrobarVeins();
        }

    }


    public Grup GrupById(string id)
    {
        buscat = null;
        for (int i = 0; i < grups.Count; i++)
        {
            if (grups[i].Id.Equals(id))
            {
                buscat = grups[i];
                break;
            }
        }
        return buscat;
    }
    public Grup GrupByPe�a(Pe�a pe�a)
    {
        buscat = null;
        for (int i = 0; i < grups.Count; i++)
        {
            if (grups[i].Peces.Contains(pe�a))
            {
                buscat = grups[i];
                break;
            }
        }
        return buscat;
    }

}



[System.Serializable]
public class Grup : System.Object
{
    public string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new System.Random();
        return new string(Enumerable.Repeat(chars,length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public Grup() { }
    public Grup(Estat estat, List<Pe�a> peces, Estat casa)
    {
        id = estat.name + "_" + RandomString(20);
        //this.estat = estat;
        this.poble = estat == casa;
        this.peces = peces;
    }
    [SerializeField] string id;
    //[SerializeField] Estat estat;
    [SerializeField] bool poble;
    [SerializeField] List<Pe�a> peces;
    [SerializeField] List<Pe�a> pecesVeines;
    //[SerializeField] List<string> grupsVeins;
    //public List<int> connexions;
    public List<string> connexionsId;

    List<Pe�a> cases;
    [SerializeField] List<Pe�a> camins;
    [SerializeField] List<Pe�a> ports;


    public string Id => id;
    //public bool IdIgual(string id) => string.Equals(this.id, id);
    public List<Pe�a> Peces => peces;
    public bool EsPoble => poble;
    public List<Pe�a> Veins => pecesVeines;

    public List<Pe�a> Cases { set => cases = value; get => cases; }
    public List<Pe�a> Camins { set => camins = value; get => camins; }
    public List<Pe�a> Ports { set => ports = value; get => ports; }



    //INTERN
    List<Pe�a> tmpVeins;

    public void TrobarVeins()
    {
        pecesVeines = new List<Pe�a>();
        //grupsVeins = new List<string>();

        //Per cada una de les peces del grup
        for (int g = 0; g < peces.Count; g++)
        {
            //Per cada un dels veins de la pe�a del grup...
            tmpVeins = peces[g].VeinsPe�a;
            for (int v = 0; v < tmpVeins.Count; v++)
            {
                if (!peces.Contains(tmpVeins[v]) && !pecesVeines.Contains(tmpVeins[v])) //Si no forma part del meu grup
                {
                    pecesVeines.Add(tmpVeins[v]);
                    //if (!grupsVeins.Contains(grups[tmpVeins[v].Grup].id)) grupsVeins.Add(grups[tmpVeins[v].Grup].id);
                }
            }

        }
    }

    public void Netejar()
    {
        peces = new List<Pe�a>();
        pecesVeines = new List<Pe�a>();
        //connexionsId = new List<string>();
        cases = new List<Pe�a>();
        camins = new List<Pe�a>();
        ports = new List<Pe�a>();
    }
    public void Load(Pe�a pe�a)
    {
        if (peces == null) peces = new List<Pe�a>();
        if(!peces.Contains(pe�a)) peces.Add(pe�a);
    }

}
