using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Visualitzacions")]
public class Visualitzacions : ScriptableObject
{
    const string DESTACAT = "_Destacat";

    [Header("LISTENERS")]
    [SerializeField] Output_GuanyarExperiencia[] guanyarExperiencia;
    [SerializeField] SaveHex save;
    [SerializeField] Fase_Processar faseProcessar;
    [SerializeField] Informacio_Grup informacioGrup;
    [SerializeField] Informacio_Connexio informacioConnexio;
    [SerializeField] Menu_Pila menuPila;
    [SerializeField] Nivell nivell;
    [Space(20)]
    [SerializeField] Grups grups;

    [Apartat("PREFABS / NEEDS")]
    [SerializeField] Utils_TextSetup puntsFlotants;
    WaitForSeconds wfs_puntsFlotants;

    [Space(20)]
    [SerializeField] AnimacioPerCodi caure;
    [SerializeField] AnimacioPerCodi colocar;
    [SerializeField] AnimacioPerCodi colocar_reaccioVei;
    [SerializeField] AnimacioPerCodi canviarEstat;
    [SerializeField] AnimacioPerCodi canviarEstat_reaccioVei;

    [Space(20)]
    [SerializeField] MaterialPropertyBlock resaltar;
    [SerializeField] MaterialPropertyBlock noResaltar;

    [Space(20)]
    [SerializeField] AnimacioPerCodi desapareixre;
    [SerializeField] AnimacioPerCodi desapareixreParent;
    [SerializeField] AnimacioPerCodi pilaPosicio1, pilaPosicio1Parent;
    [SerializeField] AnimacioPerCodi pilaPosicio2, pilaPosicio2Parent;



    private void OnEnable()
    {
        //PUNTS FLOTANTS
        for (int i = 0; i < guanyarExperiencia.Length; i++)
        {
            guanyarExperiencia[i].EnPuntuar += PuntsFlotants;
        }
        wfs_puntsFlotants = new WaitForSeconds(1);

        //PROCESSAR
        save.EnColocar += Caure;
        faseProcessar.EnColocar += Colocar;
        faseProcessar.EnCanviarEstat += CanviEstat;
        faseProcessar.EnCanviarEstatVeins += CanviEstatVei;

        //RESALTAR
        informacioGrup.EnResaltar += Resaltar;
        informacioGrup.EnDesresaltar += Desresaltar;
        informacioConnexio.EnResaltar += DestacarPeça;

        resaltar = new MaterialPropertyBlock();
        resaltar.SetInt(DESTACAT, 1);
        noResaltar = new MaterialPropertyBlock();
        noResaltar.SetInt(DESTACAT, 0);

        //PILA
        menuPila.EnDesapareixre += Desapareixre;
        menuPila.EnPosicio1 += PrimeraPosicio;
        menuPila.EnPosicio2 += SegonaPosicio;

        //NIVELL
        nivell.EnGuanyarExperiencia += UIExperiencia;
        nivell.EnPujarNivell += UINivell;
    }
    private void OnDisable()
    {
        //PUNTS FLOTANTS
        for (int i = 0; i < guanyarExperiencia.Length; i++)
        {
            guanyarExperiencia[i].EnPuntuar -= PuntsFlotants;
        }

        //PROCESSAR
        save.EnColocar -= Caure;
        faseProcessar.EnColocar -= Colocar;
        faseProcessar.EnCanviarEstat -= CanviEstat;
        faseProcessar.EnCanviarEstatVeins -= CanviEstatVei;

        //RESALTAR
        informacioGrup.EnResaltar -= Resaltar;
        informacioGrup.EnDesresaltar -= Desresaltar;
        informacioConnexio.EnResaltar -= DestacarPeça;

        //PILA
        menuPila.EnDesapareixre -= Desapareixre;
        menuPila.EnPosicio1 -= PrimeraPosicio;
        menuPila.EnPosicio2 -= SegonaPosicio;

        //NIVELL
        nivell.EnGuanyarExperiencia -= UIExperiencia;
        nivell.EnPujarNivell -= UINivell;
    }


    #region PUNTS FLOTANTS
    public void PuntsFlotants(Peça peça, int experiencia) => XS_Coroutine.StartCoroutine(PuntsFlotants_Corrutina(peça, experiencia));
    public void PuntsFlotants(float delay, Vector3 posicio, int experiencia) 
    {
        XS_Coroutine.StartCoroutine_Ending_FrameDependant(delay, () =>
        Instantiate(
            puntsFlotants,
            posicio - Utils_MainCamera_Acces.Camera.transform.forward * 1,
            Quaternion.Euler(Utils_MainCamera_Acces.Camera.transform.forward)).Setup(experiencia));
    } 
    IEnumerator PuntsFlotants_Corrutina(Peça peça, int experiencia)
    {
        Debug.Log("Crear punts flotants");
        yield return wfs_puntsFlotants;
        Instantiate(
            puntsFlotants,
            peça.transform.position - Utils_MainCamera_Acces.Camera.transform.forward * 1,
            Quaternion.Euler(Utils_MainCamera_Acces.Camera.transform.forward)).Setup(experiencia);
    }

    
    #endregion

    #region COLOCAR
    void Caure(Peça peça) => caure.Play(peça.Parent);
    void Colocar(Transform parent, List<Peça> veins)
    {
        colocar.Play(parent);
        for (int i = 0; i < veins.Count; i++)
        {
            colocar_reaccioVei.Play(veins[i].Parent);
        }
    }
    void CanviEstat(Transform parent) => canviarEstat.Play(parent);
    void CanviEstatVei(Transform parent) => canviarEstat_reaccioVei.Play(parent);
    #endregion

    #region RESALTAR
    void Resaltar(Peça peça)
    {
        Grup grup = grups.GrupByPeça(grups.Grup, peça);
        grup.Resaltat = true;

        //Destacar el grup en si.
        DestacarPeces(grup.Peces, true);

        //Destacar el grup dels cammins connectats, que també contenen els ports.
        for (int i = 0; i < grup.Camins.Count; i++)
        {
            Grup cami = grups.GrupByPeça(grups.Grup, grup.Camins[i]);
            cami.Resaltat = true;
            DestacarPeces(cami.Peces, true);
        }

        //Destacar els grups de les connexionsId.
        for (int i = 0; i < grup.ConnexionsId.Count; i++)
        {
            if (grup.ConnexionsId[i] == grup.Id)
                continue;

            Grup connectat = grups.GrupById(grups.Grup, grup.ConnexionsId[i]);
            connectat.Resaltat = true;
            DestacarPeces(connectat.Peces, true);
        }

        //Destacar els ports als que estan connectats els ports connectats.
        for (int i = 0; i < grup.Ports.Count; i++)
        {
            Grup port = grups.GrupByPeça(grups.Grup, grup.Ports[i]);
            port.Resaltat = true;
        }
    }
    void Desresaltar()
    {
        for (int i = 0; i < grups.Grup.Count; i++)
        {
            if (grups.Grup[i].Resaltat)
            {
                DestacarPeces(grups.Grup[i].Peces, false);
                grups.Grup[i].Resaltat = false;
            }

        }
    }
    void DestacarPeces(List<Peça> peces, bool destacar)
    {
        for (int p = 0; p < peces.Count; p++)
        {
            DestacarPeça(peces[p], destacar);
        }
    }
    void DestacarPeça(Hexagon hexagon, bool destacar)
    {
        MeshRenderer[] meshRenderers = hexagon.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].SetPropertyBlock(destacar ? resaltar : noResaltar);
        }
    }
    #endregion

    #region PILA
    void PrimeraPosicio(Transform transform)
    {
        pilaPosicio1.Play(transform);
        pilaPosicio1Parent.Play(transform.parent.GetComponent<RectTransform>());
        //animacions.primeraPosicio.Play(transform);
        //animacions.primeraPosicioParent.Play(transform.parent.GetComponent<RectTransform>());
    }
    void SegonaPosicio(Transform transform)
    {
        pilaPosicio2.Play(transform);
        pilaPosicio2Parent.Play(transform.parent.GetComponent<RectTransform>());
        //animacions.segonaPosicio.Play(transform);
        //animacions.segonaPosicioParent.Play(transform.parent.GetComponent<RectTransform>());
    }

    void Desapareixre(Transform transform)
    {
        desapareixre.Play(transform);
        desapareixreParent.Play(transform.parent.GetComponent<RectTransform>());
        //animacions.colocarPeça.Play(transform);
        //animacions.colocarPeçaParent.Play(transform.parent.GetComponent<RectTransform>());
    }
    #endregion

    #region NIVELL
    public void UINivell(int nivell)
    {

    }
    public void UIExperiencia(int experiencia)
    {

    }
    #endregion

    [Space(20)]
    [SerializeField] Animacions animacions;
    [Space(20)]
    [SerializeField] Prefabs prefabs;
    [Space(20)]
    [SerializeField] Sons sons;



    [System.Serializable]
    public struct Animacions
    {
        [Header("PRODUCCIO")]
        public AnimacioPerCodi producteProveir;
        public AnimacioPerCodi necessitatProveida;

        [Apartat("PECES UI")]
        public AnimacioPerCodi primeraPosicio;
        public AnimacioPerCodi primeraPosicioParent;
        public AnimacioPerCodi segonaPosicio, segonaPosicioParent;
        public AnimacioPerCodi colocarPeça, colocarPeçaParent;

        [Apartat("INFORMACIO")]
        public AnimacioPerCodi amagarInformacio;

        [Apartat("NIVELL")]
        public AnimacioPerCodi guanyarExperiencia;

        [Apartat("PECES")]
        public AnimacioPerCodi colocar;
        public AnimacioPerCodi colocar_reaccioVei;
        public AnimacioPerCodi canviarEstat;
        public AnimacioPerCodi canviarEstat_reaccioVei;
    }

    [System.Serializable]
    public struct Prefabs
    {

        [Header("PREDICCIONS")]
        public GameObject canvi;
        public GameObject mesHabitants;
        public GameObject menysHabitants;
        public GameObject connexio;

        public List<GameObject> prediccions;
    }

    public void PredirCanvi(Vector2Int coordenada)
    {
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(Grid.Instance.Instanciar(prefabs.canvi, coordenada));

    }
    public void PredirMesHabitants(Vector2Int coordenada)
    {
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(Grid.Instance.Instanciar(prefabs.mesHabitants, coordenada));
    }

    public void PredirMenysHabitants(Vector2Int coordenada)
    {
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(Grid.Instance.Instanciar(prefabs.menysHabitants, coordenada));
    }
    public void PredirConnexio(Vector2Int coordenada)
    {
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(Grid.Instance.Instanciar(prefabs.connexio, coordenada));
    }

    public void AmagarPrediccions()
    {
        for (int i = 0; i < prefabs.prediccions.Count; i++)
        {
            Destroy(prefabs.prediccions[i]);
        }
        prefabs.prediccions.Clear();
    }










    [System.Serializable]
    public struct Producte
    {
        public Producte(Peça productor, int indexProducte, Peça casa, int indexNecessitat)
        {
            this.productor = productor;
            this.indexProducte = indexProducte;
            this.casa = casa;
            this.indexNecessitat = indexNecessitat;
        }
        public Peça productor;
        public int indexProducte;
        public Peça casa;
        public int indexNecessitat;
    }

    [System.Serializable]
    public struct Sons
    {
        public So snap;
    }

    public So Snap => sons.snap;
}
