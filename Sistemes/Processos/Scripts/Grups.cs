using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Grups")]
public class Grups : ScriptableObject
{
    [SerializeField] List<Grup> grups;
    [SerializeField] Visualitzacions visualitzacions;

    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] Estat casa;
    [SerializeField] Estat cami;
    [SerializeField] Subestat port;
    [SerializeField] Estat aigua;

    [Apartat("MATERIALS")]
    [SerializeField] Shader[] reseltables;

    public List<Grup> Grup => grups;


    //INTERN
    List<Grup> grupsPendents;
    List<string> idsPobles;

    System.Action enFinalitzar;
    List<Pe�a> veinsIguals;

    List<Pe�a> veinsPe�a;

    Grup grupActual;
    List<Grup> grupsVeinsPe�a;
    Grup buscat;

    void OnEnable() => Resetejar();



    //AGRUPA LA PE�A QUE AS COLOCAT
    public void Agrupdar(List<Grup> grups, Pe�a pe�a, System.Action enFinalitzar)
    {
        if (pe�a == null)
            return;

        Debugar.LogError("--------------GRUPS---------------");

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
            grupsVeinsPe�a.Add(GrupByPe�a(grups, veinsPe�a[v]));
        }
        Debugar.LogError($"Connectar {grupsVeinsPe�a.Count} veins iguals");

        //INTENTAR AGRUPAR O CREAR GRUP NOU
        if (veinsIguals.Count > 0)
            grupActual = IntentarAgrupar(grups, pe�a, veinsIguals);
        else grupActual = CrearNouGrup(grups, pe�a);

        //TROBAR VEINS DEL GRUP
        grupActual.TrobarVeins();
  
        //PREPARAR GRUP PER ACTUALITZAR
        grupsPendents = new List<Grup>() { grupActual };

        //AFGEIR A LA LLISTA ELS GRUPS VEINS
        for (int i = 0; i < grupActual.Veins.Count; i++)
        {
            Grup grupVei = GrupByPe�a(grups, grupActual.Veins[i]);
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
        Step(grups);
    }

    void Step(List<Grup> grups)
    {
        if(grupsPendents.Count == 0)
        {
            Debugar.LogError("FINALITZAT! (GRUPS)");
            enFinalitzar.Invoke();

            return;
        }

        Connectar(grups, grupsPendents[0]);
        grupsPendents.RemoveAt(0);
        StartCoroutine_Ending(0.001f, Step, grups);
        //XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }

    #region CORRUTINA

    class CorrutinaEstaticaMonoBehavior : MonoBehaviour
    {
        private void OnDisable() => Destroy(this.gameObject);
    }
    CorrutinaEstaticaMonoBehavior corrutinaEstaticaMonoBehavior;
    void Init()
    {
        if (corrutinaEstaticaMonoBehavior == null)
        {
            GameObject gameObject = new GameObject("CorrutinaEstatica");
            corrutinaEstaticaMonoBehavior = gameObject.AddComponent<CorrutinaEstaticaMonoBehavior>();
        }
    }
    WaitForSecondsRealtime waitForSecondsRealtime;
    public Coroutine StartCoroutine_Ending(float time, System.Action<List<Grup>> ending, List<Grup> grups)
    {
        Init();
        waitForSecondsRealtime = new WaitForSecondsRealtime(time);
        return corrutinaEstaticaMonoBehavior.StartCoroutine(LoopTime(ending, grups));
    }
    IEnumerator LoopTime(System.Action<List<Grup>> ending, List<Grup> grups)
    {
        yield return waitForSecondsRealtime;
        ending.Invoke(grups);
    }
    #endregion

    public void Interrompre()
    {
        grupsPendents = new List<Grup>();
    }


    Grup IntentarAgrupar(List<Grup> grups, Pe�a pe�a, List<Pe�a> veinsIguals)
    {
        Grup elMeuGrup = null;

        for (int i = 0; i < veinsIguals.Count; i++)
        {
            if(elMeuGrup == null)
            {
                elMeuGrup = GrupByPe�a(grups, veinsIguals[i]);
                AfegirAGrup(pe�a, elMeuGrup);
            }
            else
            {
                Grup altreGrup = GrupByPe�a(grups, veinsIguals[i]);
                if (altreGrup == elMeuGrup)
                    continue;

                AjuntarGrups(elMeuGrup, altreGrup);
            }
        }

        return elMeuGrup;
        
        void AfegirAGrup(Pe�a pe�a, Grup grup)
        {
            if (!grup.Peces.Contains(pe�a))
                grup.Peces.Add(pe�a);

        }
        int AjuntarGrups(Grup desti, Grup seleccionat)
        {
            desti.Peces.AddRange(seleccionat.Peces);
            grups.Remove(seleccionat);

            return grups.IndexOf(desti);
        }
    }

    Grup CrearNouGrup(List<Grup> grups, Pe�a pe�a)
    {
        if(grups == null) grups = new List<Grup>();

        Grup tmp = new Grup(pe�a.Estat, new List<Pe�a>() { pe�a }, casa, grups.Count);
        tmp.TrobarVeins();
        grups.Add(tmp);

        return tmp;
    }

    public void Resetejar() => grups = new List<Grup>();

    //AIXO HA D'ANAR A VISUALITZACIONS!!!
    public void ResaltarGrup(Pe�a pe�a) => visualitzacions.Destacar(this, pe�a, true);

    public void ReixarDeResaltar() => visualitzacions.Destacar(this, null, false);

    public List<Pe�a> Peces(List<Grup> grups, Pe�a pe�a) 
    {
        Grup grup = GrupByPe�a(grups, pe�a);
        if (grup != null)
            return grup.Peces;
        else return null;
    }
    public List<Pe�a> Veins(List<Grup> grups, Pe�a pe�a) 
    {
        Grup grup = GrupByPe�a(grups, pe�a);
        if (grup != null)
            return grup.Veins;
        else return new List<Pe�a>();
        
    }


    void Connectar(List<Grup> grups, Grup grup)
    {
        if (!grup.EsPoble)
            return;

        //NETEJA GRUPS
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

        //AJUNTAR CASES
        for (int i = 0; i < grup.Cases.Count; i++)
        {
            Debugar.LogError("Add - CASA");
            AddConnexio(grups, grup, grup.Cases[i]);
        }

        //AJUNTA POBLES CONNECTATS PER CAMINS I GUARDA PORTS
        List<Pe�a> veinsCami;
        for (int i = 0; i < grup.Camins.Count; i++)
        {
            //BUSCAR A TOTS ELS VEINS DEL CAM�
            veinsCami = GrupByPe�a(grups, grup.Camins[i]).Veins;
            for (int c = 0; c < veinsCami.Count; c++)
            {
                if (veinsCami[c].EstatIgualA(casa) && GrupByPe�a(grups, veinsCami[c]) != grup) //SI ES UNA CASA, i no pertany al meu grup, EL CONNECTO
                {
                    Debugar.LogError("CASA >>> CAMI >>> CASA");
                    AddConnexio(grups, grup, veinsCami[c]);
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
                    List<Pe�a> costa = GrupByPe�a(grups, veinsPort[vp]).Veins;
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
                    if (!altresPorts.Contains(costes[m])) 
                    {
                        altresPorts.Add(costes[m]);
                        ConnectarPorts(grups, grup.Ports[p], costes[m]);
                    } 
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
                        AddConnexio(grups, grup, veinsAltresPort[vp]);
                    }
                }

                //CONNECTAR ELS POBLES QUE ARRIBEN AMB UN CAMI FINS AL PORT TROBAT
                for (int vap = 0; vap < veinsAltresPort.Count; vap++)
                {
                    if (veinsAltresPort[vap].EstatIgualA(cami))
                    {
                        veinsCaminsAltresPorts = GrupByPe�a(grups, veinsAltresPort[vap]).Veins;
                        for (int c = 0; c < veinsCaminsAltresPorts.Count; c++)
                        {
                            if (veinsCaminsAltresPorts[c].EstatIgualA(casa))
                            {
                                Debugar.LogError("CASA >>> PORT >>> MAR >>> PORT >>> CAMI >>> CASA");
                                AddConnexio(grups, grup, veinsCaminsAltresPorts[c]);
                            }
                        }
                    }
                }
            }
        }

        void AddConnexio(List<Grup> grups, Grup elMeuGrup, Pe�a objectiu)
        {
            Grup grupObjectiu = GrupByPe�a(grups, objectiu);
            if (grupObjectiu.Equals(elMeuGrup))
                return;

            Debugar.LogError($"Connectar ({grups[grups.IndexOf(elMeuGrup)].Peces[0].name}) amb  ({objectiu.name}");

            //CONNECTOR EL MEU GRUP AMB EL GRUP DE L'OBJECTIU
            if (!elMeuGrup.connexionsId.Contains(grupObjectiu.Id)) elMeuGrup.connexionsId.Add(grupObjectiu.Id);

            //SI L'OBJECTIU NO EM TE A MI, S'ENV� A PENDENTS
            if (grupObjectiu.connexionsId == null) grupObjectiu.connexionsId = new List<string>();
            if (!grupObjectiu.connexionsId.Contains(elMeuGrup.Id)) grupsPendents.Add(grupObjectiu);
        }

        void ConnectarPorts(List<Grup> grups, Pe�a port1, Pe�a port2)
        {
            Grup grupPort1 = GrupByPe�a(grups, port1);
            Grup grupPort2 = GrupByPe�a(grups, port2);
            if (!grupPort1.connexionsId.Contains(grupPort2.Id)) grupPort1.connexionsId.Add(grupPort2.Id);
            if (!grupPort2.connexionsId.Contains(grupPort1.Id)) grupPort2.connexionsId.Add(grupPort1.Id);
        }
    }




















    public List<Pe�a> VeinsAmbCami(List<Grup> grups, Pe�a pe�a) 
    {
        List<Pe�a> tmp = new List<Pe�a>();
        Grup grup = GrupByPe�a(grups, pe�a);
        for (int g = 0; g < grup.Peces.Count; g++)
        {
            List<Pe�a> veins = grup.Peces[g].VeinsPe�a;
            for (int v = 0; v < veins.Count; v++)
            {
                if (!tmp.Contains(veins[v])) tmp.Add(veins[v]);

                if (veins[v].EstatIgualA(cami))
                {
                    List<Pe�a> veinsDelCami = Veins(grups, veins[v]);
                    tmp.AddRange(veinsDelCami);
                }
            }
        }
        return tmp;
    }

    #region LOAD
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
        //Comprovar el numero de grups que es necessiten.
        int indexMesAlt = 0;
        for (int i = 0; i < peces.Count; i++)
        {
            //if (indexMesAlt < peces[i].Grup) indexMesAlt = peces[i].Grup;
        }

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

    #endregion


    public Grup GrupById(List<Grup> grups, string id)
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
    public Grup GrupByPe�a(List<Grup> grups, Pe�a pe�a)
    {
        Debug.LogError($"Buscar grup de la pe�a {pe�a.name}");
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
    public string RandomString(int seed, int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new System.Random(seed);
        return new string(Enumerable.Repeat(chars,length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public Grup() { }
    public Grup(Estat estat, List<Pe�a> peces, Estat casa, int index)
    {
        id = estat.name + "_" + RandomString(index, 20);
        //this.estat = estat;
        this.poble = estat == casa;
        this.peces = peces;
    }
    public Grup(Grup copia)
    {
        id = copia.id;
        poble = copia.poble;

        peces = new List<Pe�a>();
        for (int i = 0; i < copia.peces.Count; i++)
        {
            peces.Add(copia.peces[i]);
        }

        pecesVeines = new List<Pe�a>();
        for (int i = 0; i < copia.pecesVeines.Count; i++)
        {
            pecesVeines.Add(copia.pecesVeines[i]);
        }

        connexionsId = new List<string>();
        if(copia.connexionsId != null)
        {
            for (int i = 0; i < copia.connexionsId.Count; i++)
            {
                connexionsId.Add(copia.connexionsId[i]);
            }
        }

        cases = new List<Pe�a>();
        if (copia.cases != null)
        {
            for (int i = 0; i < copia.cases.Count; i++)
            {
                cases.Add(copia.cases[i]);
            }
        }
       
        camins = new List<Pe�a>();
        if (copia.camins != null)
        {
            for (int i = 0; i < copia.camins.Count; i++)
            {
                camins.Add(copia.camins[i]);
            }
        }
        
        ports = new List<Pe�a>();
        if (copia.ports != null)
        {
            for (int i = 0; i < copia.ports.Count; i++)
            {
                ports.Add(copia.ports[i]);
            }
        }
    }
    [SerializeField] string id;
    [SerializeField] bool poble;
    [SerializeField] List<Pe�a> peces;
    [SerializeField] List<Pe�a> pecesVeines;
    public List<string> connexionsId;

    List<Pe�a> cases;
    [SerializeField] List<Pe�a> camins;
    [SerializeField] List<Pe�a> ports;

    public bool resaltat;

    public string Id => id;
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
                }
            }

        }
    }

    public void Netejar()
    {
        peces = new List<Pe�a>();
        pecesVeines = new List<Pe�a>();
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
