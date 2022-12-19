using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Produccio")]
public class Produccio : ScriptableObject
{
    [SerializeField] Grups grups;
    [SerializeField] Fase_Resoldre resoldre;
    //[SerializeField] PoolPeces pool;

    [Nota("Ara es mostra nom�s per debugar", NoteType.Warning)]
    [SerializeField] List<Pe�a> productors;
    
    
    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] Estat cami;
    [SerializeField] Subestat casa;
    //PROPIETATS
    bool Finalitzat => index == productors.Count;

    //INTERN
    System.Action enFinalitzar;
    int index;
    List<Pe�a> veins;
    List<string> connexions;
    float stepTime = 0.1f;
    List<Pe�a> casesProveides;
    List<Pe�a> productorsActualitzables;
    List<Visualitzacio> visualitzacions;
    Vector3 offset;

    void OnEnable()
    {
        Resetejar();
    }
    public void AddProductor(Pe�a pe�a)
    {
        if (!productors.Contains(pe�a)) productors.Add(pe�a);
    }

    public void Resetejar()
    {
        productors = new List<Pe�a>();
    }


    public void Process(System.Action enFinalitzar)
    {
        Debugar.LogError("--------------PRODUCCIO---------------");
        index = 0;
        this.enFinalitzar = enFinalitzar;
        if (casesProveides == null) casesProveides = new List<Pe�a>();
        if (visualitzacions == null) visualitzacions = new List<Visualitzacio>();
        //CleanAllNeeds();
        Step();
    }

    void Step()
    {
        if (productors.Count == 0 || Finalitzat)
        {
            enFinalitzar.Invoke();
            return;
        }
        casesProveides.Clear();
        Pe�a.ProducteExtret[] productes = productors[index].Extraccio.productesExtrets;
        visualitzacions.Clear();

        /*bool buid = true;
        for (int i = 0; i < productes.Length; i++)
        {
            if (!productes[i].gastat)
            {
                buid = false;
                break;
            }
        }

        Debug.Log($"buid = {buid}");*/

        //if (!buid)
        //{
        GameObject[] infoProductes = productors[index].Extraccio.Subestat.InformacioMostrar(productors[index].Extraccio);

        Debug.Log($"{productes.Length} productes");
        for (int i = 0; i < productes.Length; i++)
        {
            if (productes[i].gastat)
            {
                visualitzacions.Add(new Visualitzacio(null, null, null));
                continue;
            }

            Pe�a proveida = BuscarCasaDesproveida(productors[index], productes[i].producte);
            if (proveida != null)
            {
                resoldre.Nivell.GuanyarExperiencia(1);
                casesProveides.Add(proveida);
                productes[i].gastat = true;
            }
            visualitzacions.Add(new Visualitzacio(productors[index].Extraccio, productors[index], proveida));
        }


        //Crear icones
        Debug.Log($"{visualitzacions.Count} visualitzacions");
        //Animar
        for (int i = 0; i < visualitzacions.Count; i++)
            {
                //les que ho tenen tot, han d'apareixre de l'extractor, anar cap al productor i cap a la casa
                //Els que tenen extractor i productor pero no casa. Han de sortir de l'extractor i tornar a entrar.
                XS_Coroutine.StartCoroutine(Animacio_ProductesExtrets(
                                    infoProductes[i],
                                    visualitzacions[i].productor,
                                    i,
                                    visualitzacions[i].casa));
            }
        //}




        index++;
        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }

    struct Visualitzacio
    {
        public Visualitzacio(Pe�a extractor, Pe�a productor, Pe�a casa)
        {
            this.extractor = extractor;
            this.productor = productor;
            this.casa = casa;
        }
        public Pe�a extractor;
        public Pe�a productor;
        public Pe�a casa;
    }


    Pe�a BuscarCasaDesproveida(Pe�a productor, Producte producte)
    {
        Pe�a casa = null;
        string debug = "PRODUCCIO DEBUG\n";
        connexions = grups.GrupByPe�a(productor).connexionsId;
        for (int con = 0; con < connexions.Count; con++)
        {
            List<Pe�a> poble = grups.GrupById(connexions[con]).Peces;

            for (int p = 0; p < poble.Count; p++)
            {
                debug += $"Casa {p}\n";
                if (!poble[p].TeCasa)
                    continue;

                for (int n = 0; n < poble[p].Casa.Necessitats.Length; n++)
                {
                    debug += $"Te {poble[p].Casa.Necessitats.Length} necessitats";
                    if (poble[p].Casa.Necessitats[n].Producte == producte && !poble[p].Casa.Necessitats[n].Proveit)
                    {
                        debug += $" ***No estava proveida, per tant la proveixo***";
                        poble[p].Casa.Necessitats[n].Proveir();
                        casa = poble[p];
                    }
                    debug += $"\n";
                    if (casa != null)
                        break;
                }
                if (casa != null)
                    break;
            }
            if (casa != null)
                break;
        }
        return casa;
        Debugar.LogError(debug);
    }

    IEnumerator Animacio_ProductesExtrets(GameObject producte, Pe�a productor, float index, Pe�a casa)
    {
        //GameObject producte = productor.Producte.Subestat.InformacioMostrar(productor.Producte)[0];
        yield return new WaitForSeconds(0.55f + (index * 0.3f));
        if(productor == null)
        {
            producte.GetComponent<UI_Producte>().Destruir(1f);
            Destroy(producte, 2f);
        }
        else
        {
            offset = producte.transform.localPosition;
            new Animacio_Posicio(productor.Extraccio.transform.position + offset, productor.transform.position + offset, false, false).Play(producte, 0.5f, Transicio.clamp);

            if (casa != null)
                XS_Coroutine.StartCoroutine(Animacio_EnviarProducteACasa(producte, productor.transform, casa.transform));
            else
            {
                producte.GetComponent<UI_Producte>().Destruir(1.5f);
                Destroy(producte, 2.5f);
            }
        }
        
    }


    IEnumerator Animacio_EnviarProducteACasa(GameObject producte, Transform productor, Transform casa)
    {
        yield return new WaitForSeconds(0.75f);
        new Animacio_Posicio(productor.position, casa.position, false, false).Play(producte, 0.5f, Transicio.clamp);

        Destroy(producte, 2);
    }
}
