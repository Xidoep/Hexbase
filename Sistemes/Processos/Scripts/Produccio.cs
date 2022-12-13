using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Produccio")]
public class Produccio : ScriptableObject
{
    [SerializeField] Grups grups;
    //[SerializeField] PoolPeces pool;

    [Nota("Ara es mostra només per debugar", NoteType.Warning)]
    [SerializeField] List<Peça> productors;
    
    
    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] Estat cami;
    [SerializeField] Subestat casa;
    //PROPIETATS
    bool Finalitzat => index == productors.Count;

    //INTERN
    System.Action enFinalitzar;
    int index;
    List<Peça> veins;
    List<string> connexions;
    float stepTime = 0.1f;
    List<Casa> casesProveides;
    List<Peça> productorsActualitzables;


    void OnEnable()
    {
        Resetejar();
    }
    public void AddProductor(Peça peça)
    {
        if (!productors.Contains(peça)) productors.Add(peça);
    }

    public void Resetejar()
    {
        productors = new List<Peça>();
    }


    public void Process(System.Action enFinalitzar)
    {
        Debugar.LogError("--------------PRODUCCIO---------------");
        index = 0;
        this.enFinalitzar = enFinalitzar;
        casesProveides = new List<Casa>();
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

        //Comprovar les necessitats covertes
        Producte[] recursos = productors[index].ExtreureProducte();
        if (productors[index].necessitatsCovertes == null)
        {
            productors[index].necessitatsCovertes = new List<Casa.Necessitat>();
        }

        //Comprovar necessitats eliminades
        for (int i = 0; i < productors[index].necessitatsCovertes.Count; i++)
        {
            if (productors[index].necessitatsCovertes[i] == null || productors[index].necessitatsCovertes[i].Peça.Subestat != casa) 
            {
                productors[index].necessitatsCovertes.RemoveAt(i);
                i--;
            } 
        }

        //Buscar a on distribuir, si cal
        int necessitatsActuals = productors[index].necessitatsCovertes.Count;
        if (necessitatsActuals < recursos.Length)
        {
            for (int r = 0; r < recursos.Length - necessitatsActuals; r++)
            //for (int r = 0; r < 3; r++)
            {
                BuscarCasaDesproveida(productors[index], recursos[0]);
            }
        }

        //Visualitzar
        GameObject[] productes = productors[index].Producte.Subestat.InformacioMostrar(productors[index].Producte);
        for (int i = 0; i < productes.Length; i++)
        {
            if(i < productors[index].necessitatsCovertes.Count)
                XS_Coroutine.StartCoroutine(Animacio_ProductesExtrets(productes[i], productors[index],productors[index].necessitatsCovertes[i].Peça.transform, i));
            else
                XS_Coroutine.StartCoroutine(Animacio_ProductesExtrets(productes[i], productors[index], productors[index].necessitatsCovertes[i].Peça.transform, i));
        }




        index++;
        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }

    void BuscarCasaDesproveida(Peça productor, Producte producte)
    {
        bool trobat = false;
        string debug = "PRODUCCIO DEBUG\n";
        connexions = grups.GrupByPeça(productor).connexionsId;
        for (int con = 0; con < connexions.Count; con++)
        {
            List<Peça> poble = grups.GrupById(connexions[con]).Peces;

            for (int p = 0; p < poble.Count; p++)
            {
                debug += $"Grup {p} to {poble[p].CasesCount} cases\n";
                for (int c = 0; c < poble[p].CasesCount; c++)
                {
                    debug += $"Casa {c}\n";
                    for (int n = 0; n < poble[p].Cases[c].Necessitats.Length; n++)
                    {
                        debug += $"Te {poble[p].Cases[c].Necessitats.Length} necessitats";
                        if (poble[p].Cases[c].Necessitats[n].Producte == producte && !poble[p].Cases[c].Necessitats[n].Proveit)
                        {
                            debug += $" ***No estava proveida, per tant la proveixo***";
                            productor.necessitatsCovertes.Add(poble[p].Cases[c].Necessitats[n]);
                            poble[p].Cases[c].Necessitats[n].Proveir();
                            trobat = true;
                        }
                        debug += $"\n";
                        if (trobat)
                            break;
                    }
                    if (trobat)
                        break;
                }
                if (trobat)
                    break;
            }
        }
        Debugar.LogError(debug);
    }

    IEnumerator Animacio_ProductesExtrets(GameObject producte, Peça productor, float index)
    {
        yield return new WaitForSeconds(0.55f + (index * 0.3f));
        new Animacio_Posicio(productor.Producte.transform.position, productor.transform.position, false, false).Play(producte, 0.5f, Transicio.clamp);
        Destroy(producte, 2);
    }
    IEnumerator Animacio_ProductesExtrets(GameObject producte, Peça productor, Transform casa, float index)
    {
        //GameObject producte = productor.Producte.Subestat.InformacioMostrar(productor.Producte)[0];
        yield return new WaitForSeconds(0.55f + (index * 0.3f));
        new Animacio_Posicio(productor.Producte.transform.position, productor.transform.position, false, false).Play(producte, 0.5f, Transicio.clamp);

        XS_Coroutine.StartCoroutine(Animacio_EnviarProducteACasa(producte, productor.transform, casa));
    }

    IEnumerator Animacio_EnviarProducteACasa(GameObject producte, Transform productor, Transform casa)
    {
        yield return new WaitForSeconds(0.75f);
        new Animacio_Posicio(productor.position, casa.position, false, false).Play(producte, 0.5f, Transicio.clamp);

        Destroy(producte, 2);
    }
}
