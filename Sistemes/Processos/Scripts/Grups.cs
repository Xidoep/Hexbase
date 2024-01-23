using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Grups")]
public class Grups : ScriptableObject
{
    public void Setup(EstatColocable casa, EstatColocable cami, Estat port, EstatColocable aigua)
    {
        this.casa = casa;
        this.cami = cami;
        this.port = port;
        this.aigua = aigua;
    }

    [SerializeField] List<Grup> grups;

    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] EstatColocable casa;
    [SerializeField] EstatColocable cami;
    [SerializeField] Estat port;
    [SerializeField] EstatColocable aigua;

    public List<Grup> GetGrups => grups;
    public List<Pe�a> ConnexionsFetes => connexionsFetes;

    //INTERN
    List<Grup> grupsPendents;
    List<string> idsPobles;

    System.Action enFinalitzar;
    List<Pe�a> veinsIguals;

    List<Pe�a> veinsPe�a;

    Grup grupActual;
    List<Grup> grupsVeinsPe�a;
    Grup buscat;
    List<Pe�a> connexionsFetes;




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
        if (connexionsFetes == null) connexionsFetes = new List<Pe�a>();
        else connexionsFetes.Clear();

        //BUSCAR VEINS DE LA PE�A IGUALS
        veinsPe�a = pe�a.VeinsPe�a;
        
        for (int v = 0; v < veinsPe�a.Count; v++)
        {
            if (pe�a.EstatIgualA(veinsPe�a[v].Estat)) veinsIguals.Add(veinsPe�a[v]);
            grupsVeinsPe�a.Add(GrupByPe�a(grups, veinsPe�a[v]));
        }

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

            if (grups[g].ConnexionsId == null) grups[g].ConnexionsId = new List<string>();
            for (int c = 0; c < grups[g].ConnexionsId.Count; c++)
            {
                if (!idsPobles.Contains(grups[g].ConnexionsId[c])) grups[g].ConnexionsId.RemoveAt(c);
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
            {
                grup.Peces.Add(pe�a);
            }

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
    //public void ResaltarGrup(Pe�a pe�a) => visualitzacions.Destacar(this, pe�a, true);

    //public void ReixarDeResaltar() => visualitzacions.Destacar(this, null, false);

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
        //grup.connexionsId = new List<string>() { grup.Id };


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
        for (int i = 0; i < grup.Camins.Count; i++)
        {
            //BUSCAR A TOTS ELS VEINS DEL CAM�
            Grup cami = GrupByPe�a(grups, grup.Camins[i]);
            for (int c = 0; c < cami.Veins.Count; c++)
            {
                if (cami.Veins[c].EstatIgualA(casa) && GrupByPe�a(grups, cami.Veins[c]) != grup) //SI ES UNA CASA, i no pertany al meu grup, EL CONNECTO
                {
                    Debugar.LogError("CASA >>> CAMI >>> CASA");
                    AddConnexio(grups, grup, cami.Veins[c]);
                }
               
            }
            for (int c = 0; c < cami.Peces.Count; c++)
            {
                if (cami.Peces[c].SubestatIgualA(port)) //SI ES UN PORT, EL GUARDO
                {
                    if (!grup.Ports.Contains(cami.Peces[c])) grup.Ports.Add(cami.Peces[c]);
                }
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
                        //ConnectarPorts(grups, grup.Ports[p], costes[m]);
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
            if (!elMeuGrup.ConnexionsId.Contains(grupObjectiu.Id)) 
            {
                elMeuGrup.ConnexionsId.Add(grupObjectiu.Id);
                if (!connexionsFetes.Contains(objectiu)) connexionsFetes.Add(objectiu);
            } 

            //SI L'OBJECTIU NO EM TE A MI, S'ENV� A PENDENTS
            if (grupObjectiu.ConnexionsId == null) grupObjectiu.ConnexionsId = new List<string>();
            if (!grupObjectiu.ConnexionsId.Contains(elMeuGrup.Id)) grupsPendents.Add(grupObjectiu);

            
        }

        void ConnectarPorts(List<Grup> grups, Pe�a port1, Pe�a port2)
        {
            Grup grupPort1 = GrupByPe�a(grups, port1);
            Grup grupPort2 = GrupByPe�a(grups, port2);
            if (!grupPort1.ConnexionsId.Contains(grupPort2.Id)) grupPort1.ConnexionsId.Add(grupPort2.Id);
            if (!grupPort2.ConnexionsId.Contains(grupPort1.Id)) grupPort2.ConnexionsId.Add(grupPort1.Id);
        }
    }




















    public List<Pe�a> VeinsAmbCami(List<Grup> grups, Pe�a pe�a) 
    {
        List<Pe�a> tmp = new List<Pe�a>();
        Grup grup = GrupByPe�a(grups, pe�a);
        if(grup == null)
            return tmp;

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


    public void Load(List<GrupGuardat> guardats)
    {
        grups = new List<Grup>();
        for (int i = 0; i < guardats.Count; i++)
        {
            grups.Add(new Grup(guardats[i]));
        }

        /*for (int g = 0; g < guardats.Length; g++)
        {
            int indexId = -1;
            for (int i = 0; i < grups.Count; i++)
            {
                if (grups[i].Id == guardats[g].id)
                {
                    indexId = i;
                    break;
                }
            }

            if (indexId == -1) //No est� el grup creat.
            {
                grups.Add(new Grup(guardats[g]));
            }
        }*/
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
        //Debug.LogError($"Buscar grup de la pe�a {pe�a.name}");
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

    //>>> CREAR GRUP FROM LOAD
    public Grup(GrupGuardat guardat) 
    {
        id = guardat.id;
        poble = guardat.poble;
        
        peces = new List<Pe�a>();
        for (int i = 0; i < guardat.peces.Length; i++)
        {
            peces.Add((Pe�a)Grid.Instance.Get(guardat.peces[i]));
        }

        veins = new List<Pe�a>();
        for (int i = 0; i < guardat.veines.Length; i++)
        {
            veins.Add((Pe�a)Grid.Instance.Get(guardat.veines[i]));
        }

        connexionsId = new List<string>(guardat.connexionsId);

        cases = new List<Pe�a>();
        for (int i = 0; i < guardat.cases.Length; i++)
        {
            cases.Add((Pe�a)Grid.Instance.Get(guardat.cases[i]));
        }

        camins = new List<Pe�a>();
        for (int i = 0; i < guardat.camins.Length; i++)
        {
            camins.Add((Pe�a)Grid.Instance.Get(guardat.camins[i]));
        }

        ports = new List<Pe�a>();
        for (int i = 0; i < guardat.ports.Length; i++)
        {
            ports.Add((Pe�a)Grid.Instance.Get(guardat.ports[i]));
        }
    }

    //>>> CREAR NOU GRUP
    public Grup(EstatColocable estat, List<Pe�a> peces, EstatColocable casa, int index)
    {
        id = estat.name + "_" + RandomString(index, 20);
        //this.estat = estat;
        this.poble = estat == casa;
        this.peces = peces;
        if (this.poble)
            ConnexionsId = new List<string>() { id };
    }
    //>>> CREAR COPIA DE GRUP
    public Grup(Grup copia)
    {
        id = copia.id;
        poble = copia.poble;

        peces = new List<Pe�a>();
        for (int i = 0; i < copia.peces.Count; i++)
        {
            peces.Add(copia.peces[i]);
        }

        veins = new List<Pe�a>();
        for (int i = 0; i < copia.veins.Count; i++)
        {
            veins.Add(copia.veins[i]);
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
    [SerializeField] List<Pe�a> veins;
    [SerializeField] List<string> connexionsId;

    [SerializeField] List<Pe�a> cases;
    [SerializeField] List<Pe�a> camins;
    [SerializeField] List<Pe�a> ports;

    [SerializeField] bool resaltat;

    public string Id => id;
    public List<Pe�a> Peces => peces;
    public bool EsPoble => poble;
    public List<Pe�a> Veins => veins;

    public List<string> ConnexionsId { get => connexionsId; set => connexionsId = value; }
    public List<Pe�a> Cases { set => cases = value; get => cases; }
    public List<Pe�a> Camins { set => camins = value; get => camins; }
    public List<Pe�a> Ports { set => ports = value; get => ports; }
    public bool Resaltat { set => resaltat = value; get => resaltat; }


    //INTERN
    List<Pe�a> tmpVeins;

    public void TrobarVeins()
    {
        veins = new List<Pe�a>();
        //Per cada una de les peces del grup
        for (int g = 0; g < peces.Count; g++)
        {
            //Per cada un dels veins de la pe�a del grup...
            tmpVeins = peces[g].VeinsPe�a;
            for (int v = 0; v < tmpVeins.Count; v++)
            {
                if (!peces.Contains(tmpVeins[v]) && !veins.Contains(tmpVeins[v])) //Si no forma part del meu grup
                {
                    veins.Add(tmpVeins[v]);
                }
            }

        }
    }


}
