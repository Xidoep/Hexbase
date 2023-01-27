using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Visualitzacions")]
public class Visualitzacions : ScriptableObject
{
    [SerializeField] Animacions animacions;
    [Space(20)]
    [SerializeField] Prefabs prefabs;
    [Space(20)]
    [SerializeField] Materials materials;

    List<Vector3> posicions;
    Transform camera;
    Grid grid;

    [System.Serializable]
    public struct Animacions
    {
        [Header("PRODUCCIO")]
        public Animacio_Scriptable producteProveir;
        public Animacio_Scriptable necessitatProveida;

        [Apartat("PECES UI")]
        public Animacio_Scriptable primeraPosicio;
        public Animacio_Scriptable segonaPosicio;
        public Animacio_Scriptable segonaPosicioParent;
        public Animacio_Scriptable colocarPeça;
        public Animacio_Scriptable colocarPeçaParent;

        [Apartat("INFORMACIO")]
        public Animacio_Scriptable amagarInformacio;

        [Apartat("NIVELL")]
        public Animacio_Scriptable guanyarExperiencia;

        [Apartat("PECES")]
        public Animacio_Scriptable canviarEstat;
    }

    [System.Serializable]
    public struct Prefabs
    {
        [Header("PUNTS")]
        public Utils_InstantiableFromProject guanyarPunts;

        [Header("PREDICCIONS")]
        public GameObject canvi;
        public GameObject mesHabitants;
        public GameObject menysHabitants;

        public List<GameObject> prediccions;
    }

    public void PredirCanvi(Vector2Int coordenada)
    {
        if (grid == null) grid = GameObject.FindObjectOfType<Grid>();
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(grid.Instanciar(prefabs.canvi, coordenada));

    }
    public void PredirMesHabitants(Vector2Int coordenada)
    {
        if (grid == null) grid = GameObject.FindObjectOfType<Grid>();
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(grid.Instanciar(prefabs.mesHabitants, coordenada));
    }

    public void PredirMenysHabitants(Vector2Int coordenada)
    {
        if (grid == null) grid = GameObject.FindObjectOfType<Grid>();
        if (prefabs.prediccions == null) prefabs.prediccions = new List<GameObject>();

        prefabs.prediccions.Add(grid.Instanciar(prefabs.menysHabitants, coordenada));
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
    public struct Materials
    {
        public const string DESTACAT = "_Destacat";

        [Header("GRUPS")]
        public MaterialPropertyBlock resaltar;
        public MaterialPropertyBlock noResaltar;

        public void Setup()
        {
            resaltar = new MaterialPropertyBlock();
            resaltar.SetInt(DESTACAT, 1);
            noResaltar = new MaterialPropertyBlock();
            noResaltar.SetInt(DESTACAT, 0);
        }
    }

    private void OnEnable()
    {
        materials.Setup();
    }

    public void Destacar(Grups grups, Peça peça, bool destacar)
    {
        if (destacar)
        {
            Grup grup = grups.GrupByPeça(grups.Grup, peça);
            grup.resaltat = true;

            //Destacar el grup en si.
            DestacarPeces(grup.Peces, true);

            //Destacar el grup dels cammins connectats, que també contenen els ports.
            for (int i = 0; i < grup.Camins.Count; i++)
            {
                Grup cami = grups.GrupByPeça(grups.Grup, grup.Camins[i]);
                cami.resaltat = true;
                DestacarPeces(cami.Peces, true);
            }

            //Destacar els grups de les connexionsId.
            for (int i = 0; i < grup.connexionsId.Count; i++)
            {
                if (grup.connexionsId[i] == grup.Id)
                    continue;

                Grup connectat = grups.GrupById(grups.Grup, grup.connexionsId[i]);
                connectat.resaltat = true;
                DestacarPeces(connectat.Peces, true);
            }

            //Destacar els ports als que estan connectats els ports connectats.
            for (int i = 0; i < grup.Ports.Count; i++)
            {
                Grup port = grups.GrupByPeça(grups.Grup, grup.Ports[i]);
                port.resaltat = true;
                //Dibuixar una linia entre els ports connectats.
                /*for (int c = 0; c < port.connexionsId.Count; c++)
                {
                    LineRenderer linia = new GameObject("Connexió maritima").AddComponent<LineRenderer>();
                    linia.transform.SetParent(grup.Ports[i].transform);
                    linia.SetPositions(new Vector3[]
                    {
                        grup.Ports[i].transform.position,
                        grups.GrupById(grups.Grup,port.connexionsId[c]).Peces[0].transform.position
                    });
                }*/

            }

        }
        else
        {
            for (int i = 0; i < grups.Grup.Count; i++)
            {
                if (grups.Grup[i].resaltat)
                {
                    DestacarPeces(grups.Grup[i].Peces, false);
                    grups.Grup[i].resaltat = false;
                }
                
            }
        }




        void DestacarPeces(List<Peça> peces, bool destacar)
        {
            for (int p = 0; p < peces.Count; p++)
            {
                Debugar.LogError($"Resaltar peça: {peces[p].gameObject.name}");
                DestacarPeça(peces[p], destacar);
                /*MeshRenderer[] meshRenderers = peces[p].GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < meshRenderers.Length; i++)
                    meshRenderers[i].SetPropertyBlock(destacar ? materials.resaltar : materials.noResaltar);
                }*/
                /*if (peces[p].SubestatIgualA(port))
                {
                    LineRenderer[] lineRenderers = peces[p].GetComponentsInChildren<LineRenderer>();
                    for (int l = lineRenderers.Length; l > 0; l--)
                    {
                        Destroy(lineRenderers[l]);
                    }
                }*/
            }
        }
    }
    public void DestacarPeça(Peça peça, bool destacar)
    {
        MeshRenderer[] meshRenderers = peça.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].SetPropertyBlock(destacar ? materials.resaltar : materials.noResaltar);
        }
    } 
   

    public void CanviarEstat(Peça peça) => animacions.canviarEstat.Play(peça.Parent);

    public void PrimeraPosicio(Transform transform) => animacions.primeraPosicio.Play(transform);
    public void SegonaPosicio(Transform transform) 
    {
        animacions.segonaPosicio.Play(transform);
        animacions.segonaPosicioParent.Play(transform.parent.GetComponent<RectTransform>());
    } 

    public void ColocarPeça(Transform transform) 
    {
        animacions.colocarPeça.Play(transform);
        animacions.colocarPeçaParent.Play(transform.parent.GetComponent<RectTransform>());
    }

    public void AddGuanyarPunts(Vector3 posicio)
    {
        if (posicions == null) posicions = new List<Vector3>();

        posicions.Add(posicio);
    }
   
    public void GuanyarPunts(float delay)
    {
        if (camera == null) camera = Camera.main.transform;

        XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

        void Animacio() 
        {
            for (int i = 0; i < posicions.Count; i++)
            {
                prefabs.guanyarPunts.Instantiate(posicions[0], Quaternion.Euler(camera.forward));
                posicions.RemoveAt(0);
            }
        } 
    }

    public void AmagarInformacio(GameObject gameObject)
    {
        animacions.amagarInformacio.Play(gameObject.transform);
        Destroy(gameObject, 0.51f);
    }

    public void GuanyarExperiencia(Transform transform) => animacions.guanyarExperiencia.Play(transform);

    public void Produccio(Visualitzacions.Producte v, bool ultima, System.Action enFinalitzar)
    {
        XS_Coroutine.StartCoroutine_Ending(0.55f + (v.indexProducte * 0.3f), Animacio);
        //v.productor.Extraccio.mostrarInformacio?.Invoke(v.productor, true);
        //v.productor.Extraccio.BlocarInformacio = true;

        void Animacio()
        {
            
            if (v.productor == null)
            {
                DestruirProducte(v.productor, v.indexProducte, ultima, enFinalitzar);
            }
            else
            {
                ExtreureProducte(v.productor, v.indexProducte);

                if (v.casa != null)
                    RepartirProducte(v.productor, v.indexProducte, v.casa, v.indexNecessitat, ultima, enFinalitzar);
                else
                    DestruirProducte(v.productor, v.indexProducte, ultima, enFinalitzar);
            }
        }

        void ExtreureProducte(Peça p, int i)
        {
            GameObject producte = p.Extraccio.productesExtrets[i].informacio.gameObject;

            Vector3 offset = producte.transform.localPosition;
            new Animacio_Posicio(p.Extraccio.transform.position + offset, p.transform.position + offset, false, false).Play(producte.transform, 0.5f, Transicio.clamp);
        }
        void RepartirProducte(Peça p, int iProd, Peça c, int iNeed, bool ultima, System.Action enFinalitzar)
        {
            XS_Coroutine.StartCoroutine_Ending(0.75f, Animacio);

            void Animacio()
            {
                //c.mostrarInformacio?.Invoke(c, true);
                //c.BlocarInformacio = true;

                GameObject producte = p.Extraccio.productesExtrets[iProd].informacio.gameObject;

                float temps = Vector3.Distance(p.transform.position, c.transform.position) * 0.25f;

                new Animacio_Posicio(p.transform.position, c.transform.position, false, false).Play(producte.transform, temps, Transicio.clamp);

                ProducteProveir(p, producte, temps + 1);
                NecessitatProveida(c, c.Casa.Necessitats[iNeed].Informacio.gameObject, temps + 1, ultima, enFinalitzar);
            }
        }
        void ProducteProveir(Peça p, GameObject ui, float delay)
        {
            XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

            void Animacio()
            {
                animacions.producteProveir.Play(ui.transform);
                //Destroy(gameObject, 1.5f);
                p.Extraccio.BlocarInformacio = false;
                //p.Extraccio.mostrarInformacio?.Invoke(p.Extraccio, false);
            }
        }

        void NecessitatProveida(Peça c, GameObject ui, float delay, bool ultima, System.Action enFinalitzar)
        {
            XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

            void Animacio()
            {
                animacions.necessitatProveida.Play(ui.transform);
                GuanyarPunts(ui.transform.position);
                c.BlocarInformacio = false;
                c.mostrarInformacio?.Invoke(c, false);

                if(ultima)
                    XS_Coroutine.StartCoroutine_Ending(0.5f, enFinalitzar);
            }
        }

        void GuanyarPunts(Vector3 posicio)
        {
            if (camera == null) camera = Camera.main.transform;
            prefabs.guanyarPunts.Instantiate(posicio, Quaternion.Euler(camera.forward));
        }
    }


    public void DestruirProducte(Peça p, int i, bool ultima, System.Action enFinalitzar) 
    {

        p.Extraccio.productesExtrets[i].informacio.gameObject.GetComponent<UI_Producte>().Destruir(1.5f);
        XS_Coroutine.StartCoroutine_Ending(1.6f, RemostrarLaInformacio);

        void RemostrarLaInformacio()
        {
            p.Extraccio.BlocarInformacio = false;

            if (ultima)
                enFinalitzar.Invoke();
            //productor.Extraccio.mostrarInformacio?.Invoke(productor.Extraccio.Informacio, productor, false);
        }
        
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
}
