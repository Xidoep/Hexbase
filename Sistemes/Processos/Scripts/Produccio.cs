using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Produccio")]
public class Produccio : ScriptableObject
{
    [SerializeScriptableObject][SerializeField] Grups grups;
    //[SerializeScriptableObject] [SerializeField] Fase_Resoldre resoldre;
    [SerializeScriptableObject] [SerializeField] Nivell nivell;
    [SerializeScriptableObject] [SerializeField] Visualitzacions visualitzacions;
    //[SerializeField] PoolPeces pool;

    [Nota("Ara es mostra només per debugar", NoteType.Warning)]
    [SerializeField] List<Peça> productors;

    [SerializeField] Utils_InstantiableFromProject producte;
    [SerializeField] Utils_InstantiableFromProject habitant;
    [SerializeField] Utils_InstantiableFromProject proveit;

    //[Apartat("ESTATS NECESSARIS")]
    //[SerializeField] Estat cami;
    //[SerializeField] Subestat casa;

    /*
    [Apartat("ANIMACIONS")]
    [SerializeField] Animacio_Scriptable producteProveir;
    [SerializeField] Animacio_Scriptable necessitatProveida;

    [SerializeField] Utils_InstantiableFromProject EfecteGuanyarPunts;
    */

    //PROPIETATS
    public List<Peça> Productors => productors;



    bool Finalitzat => index == productors.Count;


    //INTERN
    
    System.Action<List<Peça>> enFinalitzar;
    int index;
    //List<Peça> veins;
    List<string> connexions;
    float stepTime = 0.1f;
    List<Peça> proveides;
    Peça productorActual;
    //List<Peça> productorsActualitzables;
    //Vector3 offset;
    //[SerializeField] List<Visualitzacions.Producte> visualitzacioProducte;




    int productesAVisualitzar = 0;


    void OnEnable()
    {
        Resetejar();
    }
    public void AddProductor(Peça peça)
    {
        if (!productors.Contains(peça)) productors.Add(peça);
    }
    public void RemoveProductor(Peça peça)
    {
        if (productors.Contains(peça)) productors.Remove(peça);
    }

    public void Resetejar()
    {
        productors = new List<Peça>();
    }


    public void Process(System.Action<List<Peça>> enFinalitzar)
    {
        Debugar.LogError("--------------PRODUCCIO---------------");
        index = 0;
        productesAVisualitzar = 0;
        this.enFinalitzar = enFinalitzar;

        if (proveides == null) 
            proveides = new List<Peça>();
        else 
            proveides.Clear();

        //if (visualitzacioProducte == null) visualitzacioProducte = new List<Visualitzacions.Producte>();
        //CleanAllNeeds();
        XS_Coroutine.StartCoroutine_Ending(2, Step);
        //Step();
    }


    void Step()
    {
        if (productors.Count == 0 || Finalitzat)
        {
            enFinalitzar.Invoke(proveides);
            return;
        }

        //visualitzacioProducte.Clear();

        //COMPROVAR SI L'HI QUEDEN PRODUCTES SENSE GASTAR
        bool hiHaProductePerGastar = false;
        for (int i = 0; i < productors[index].Connexio.ProductesExtrets.Length; i++)
        {
            if (!productors[index].Connexio.ProductesExtrets[i].gastat)
            {
                hiHaProductePerGastar = true;
                //productors[index].Connexio.MostrarInformacio?.Invoke(productors[index].Connexio);
                //productors[index].Connexio.SetBlocarInformacio = true;
                break;
            }
        }

        if (hiHaProductePerGastar)
        {
            //BUSCAR UNA CASA DESPROVEIDA CONNECTADA
            for (int i = 0; i < productors[index].Connexio.ProductesExtrets.Length; i++)
            {
                if (productors[index].Connexio.ProductesExtrets[i].gastat)
                    continue;

                //Peça proveida = BuscarCasaDesproveida(productors[index], productors[index].Connexio.ProductesExtrets[i].producte, out int indexNecessitat);
                Peça proveida = BuscarCasaDesproveidaRecepta(productors[index], productors[index].Connexio.ProductesExtrets[i].producte, out int indexNecessitat);
                if (proveida != null)
                {
                    //proveida.MostrarInformacio?.Invoke(proveida, true);
                    //proveida.SetBlocarInformacio = true;

                    //resoldre.Nivell.GuanyarExperiencia(1);
                    //nivell.GuanyarExperiencia(1, 12);

                    proveides.Add(proveida);

                    productors[index].Connexio.ProductesExtrets[i].gastat = true;

                    productorActual = productors[index];

                    //0 = Crea efectes
                    Transform p = producte.InstantiateReturn(productors[index].transform.position).transform;

                    //1 = Mou els productes fins al Productor;
                    /*new Animacio_Posicio(
                        productorActual.Connexio.transform.position,
                        productorActual.transform.position
                        ).Play(p, 1, 1, Transicio.clamp, false);
                    Destroy(p.GetComponent<Lector>(), 2);

                    //1.5 = surt habitant
                    habitant.Instantiate(proveida.transform.position, 1.5f);
                    
                    //2.1 = Mou els productes fins a les cases
                    XS_Coroutine.StartCoroutine_Ending(2.1f, () =>
                    new Animacio_Posicio(
                         productorActual.transform.position,
                         proveida.transform.position
                        ).Play(p, 1, Transicio.clamp, false)
                    );

                    //3 = Instanciar Cor
                    proveit.Instantiate(proveida.transform.position + Vector3.up * 2, 3);
                    */
                    //4 = punts flotants
                    //6 = Augmenta punts.



                    //----------

                    //0.5 = apareix habitant
                    habitant.Instantiate(proveida.transform.position, 0.5f);

                    //1 = Mou productes fins a la casa
                    new Animacio_Posicio(
                         productorActual.transform.position,
                         proveida.transform.position
                        ).Play(p, 1, 1, Transicio.clamp, false);

                    //2 = Apareix Cor.
                    proveit.Instantiate(proveida.transform.position + Vector3.up * 1.5f, 2);

                    XS_Coroutine.StartCoroutine_Ending_FrameDependant(5, productorActual.MostrarInformacio);
                    //3 = punts flotants
                    //5 = Augmenta punts.

                }
                //visualitzacioProducte.Add(new Visualitzacions.Producte(productors[index], i, proveida, indexNecessitat));
            }



            index++;

            /*
            //VISUALITZAR
            for (int i = 0; i < visualitzacioProducte.Count; i++)
            {
                Debugar.Log($"index = {index}. Productors = {productors.Count}|| i = {i}. visualitzacioProducte = {visualitzacioProducte.Count - 1}|| {index == productors.Count && i == visualitzacioProducte.Count - 1}");
                visualitzacions.Produccio(visualitzacioProducte[i], index == productors.Count && i == visualitzacioProducte.Count - 1, MostrarInformacioFinal);
            }
            */
        }
        else
        {
            //ESBORRAR PRODUCTORS PERQUE JA NO CALGUI VISUALITZAR (ESGOTATS)
            productors.RemoveAt(index);
        }

        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }

    /*
    void MostrarInformacioFinal()
    {
        for (int i = 0; i < casesProveides.Count; i++)
        {
            casesProveides[i].MostrarInformacio?.Invoke(casesProveides[i], false);
        }
        for (int i = 0; i < productors.Count; i++)
        {
            productors[i].Connexio.MostrarInformacio?.Invoke(productors[i].Connexio, false);
        }
    }
    */

    Peça BuscarCasaDesproveidaRecepta(Peça productor, Producte producte, out int index)
    {
        Peça casa = null;
        int _index = -1;


        //string debug = "PRODUCCIO DEBUG\n";
        connexions = grups.GrupByPeça(grups.GetGrups, productor).ConnexionsId;
        if (connexions == null)
        {
            index = -1;
            return null;
        }

        for (int con = 0; con < connexions.Count; con++)
        {
            List<Peça> poble = grups.GrupById(grups.GetGrups, connexions[con]).Peces;

            for (int p = 0; p < poble.Count; p++)
            {
                //debug += $"Casa {p}\n";
                if (!poble[p].TeCasa)
                    continue;

                for (int c = 0; c < poble[p].CasesLength; c++)
                {
                    Processador.Proces proces = poble[p].Cases[c].Proveir(producte);
                    //if (poble[p].Cases[c].Proveir(producte))
                    if (proces.confirmat)
                    {
                        _index = c;
                        casa = poble[p];
                        visualitzacions.PuntsFlotants(2.6f, poble[p].transform.position + (Vector3.up * 1.4f), proces.experiencia);
                        nivell.GuanyarExperiencia(proces.experiencia, 5);
                        Debug.Log("PROVEIDA!");
                    }

                    //debug += $"\n";
                    if (casa != null)
                        break;
                }
                if (casa != null)
                    break;
            }
            if (casa != null)
                break;
        }
        index = _index;
        //Debugar.LogError(debug);
        return casa;
    }

    

    /*
    Peça BuscarCasaDesproveida(Peça productor, Producte producte, out int index)
    {
        Peça casa = null;
        int _index = -1;
        //string debug = "PRODUCCIO DEBUG\n";
        connexions = grups.GrupByPeça(grups.Grup, productor).connexionsId;
        for (int con = 0; con < connexions.Count; con++)
        {
            List<Peça> poble = grups.GrupById(grups.Grup, connexions[con]).Peces;

            for (int p = 0; p < poble.Count; p++)
            {
                //debug += $"Casa {p}\n";
                if (!poble[p].TeCasa)
                    continue;

                for (int c = 0; c < poble[p].CasesLength; c++)
                {
                    //debug += $"Te {poble[p].Casa.Necessitats.Length} necessitats";
                    if (poble[p].Cases[c].Necessitats[0].Producte == producte && !poble[p].Cases[c].Necessitats[0].Proveit)
                    {
                        //debug += $" ***No estava proveida, per tant la proveixo***";
                        //poble[p].mostrarInformacio?.Invoke(poble[p], true);
                        poble[p].Cases[c].Proveir(producte);
                        _index = c;
                        casa = poble[p];
                    }
                    //debug += $"\n";
                    if (casa != null)
                        break;
                }
                if (casa != null)
                    break;
            }
            if (casa != null)
                break;
        }
        index = _index;
        //Debugar.LogError(debug);
        return casa;
    }
    */









}
