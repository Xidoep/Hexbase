using System;
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
    Action enFinalitzar;
    int index;
    List<Peça> veins;
    List<string> connexions;
    float stepTime = 0.1f;

    List<Casa> casesProveides;

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
        CleanAllNeeds();
        Step();
    }

    void Step()
    {
        if (productors.Count == 0 || Finalitzat)
        {
            enFinalitzar.Invoke();
            for (int c = 0; c < casesProveides.Count; c++)
            {
                for (int n = 0; n < casesProveides[c].Necessitats.Length; n++)
                {
                    casesProveides[c].Necessitats[n].Comprovat = false;
                }
            }
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
        if (productors[index].necessitatsCovertes.Count < recursos.Length)
        {
            for (int r = 0; r < recursos.Length - productors[index].necessitatsCovertes.Count; r++)
            {
                BuscarCasaDesproveida(productors[index], recursos[0]);
            }
        }

        


        //Produir
        for (int i = 0; i < productors[index].necessitatsCovertes.Count; i++)
        {
            XS_Coroutine.StartCoroutine(Animacio_ProductesExtrets(productors[index],productors[index].necessitatsCovertes[i].Peça.transform, i));
        }
        //




        //RepartimentEquitatiu(productors[index]);
        //productors[index].Produir();

        index++;
        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }

    void CleanAllNeeds()
    {
        for (int po = 0; po < grups.Grup.Count; po++)
        {
            if (!grups.Grup[po].Peces[0].SubestatIgualA(casa))
                continue;

            for (int pe = 0; pe < grups.Grup[po].Peces.Count; pe++)
            {
                for (int c = 0; c < grups.Grup[po].Peces[pe].CasesCount; c++)
                {
                    grups.Grup[po].Peces[pe].Cases[c].Clean();
                }
            }
            
        }
    }
    void BuscarCasaDesproveida(Peça productor, Producte producte)
    {
        bool trobat = false;
        connexions = grups.GrupByPeça(productor).connexionsId;
        for (int con = 0; con < connexions.Count; con++)
        {
            List<Peça> poble = grups.GrupById(connexions[con]).Peces;

            for (int p = 0; p < poble.Count; p++)
            {
                for (int c = 0; c < poble[p].CasesCount; c++)
                {
                    for (int n = 0; n < poble[p].Cases[c].Necessitats.Length; n++)
                    {
                        if (poble[p].Cases[c].Necessitats[n].Producte == producte && !poble[p].Cases[c].Necessitats[n].proveit)
                        {
                            productor.necessitatsCovertes.Add(poble[p].Cases[c].Necessitats[n]);
                            poble[p].Cases[c].Necessitats[n].proveit = true;
                            trobat = true;
                        }

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
    }
    void RepartimentEquitatiu(Peça productor)
    {
        bool proveit = false;

        //Extreure productes
        Producte[] recursos = productor.ExtreureProducte();
        Debug.LogError($"Donar {recursos.Length} Recursos");

        for (int r = 0; r < recursos.Length; r++)
        {
            //Agafa les connexions que te el proveidor
            connexions = grups.GrupByPeça(productor).connexionsId;
            for (int c = 0; c < connexions.Count; c++)
            {
                //Per cada connexio que te, intenta proveir el producte extret
                proveit = IntentarProveir(productor,grups.GrupById(connexions[c]).Peces, recursos[r]);

                //Si l'ha aconseguit proveir, no ho continua intentant.
                if (proveit) 
                    break;
            }

            if (!proveit)
            {

                Debug.LogError("Sobra aquest recurs. haig de buscar un poble connectat per enviar els recursos.");
            }
        }
       
    }

    bool IntentarProveir(Peça productor, List<Peça> poble, Producte producte)
    {
        bool proveit = false;
        for (int p = 0; p < poble.Count; p++)
        {
            for (int c = 0; c < poble[p].CasesCount; c++)
            {
                for (int n = 0; n < poble[p].Cases[c].Necessitats.Length; n++)
                {
                    if (poble[p].Cases[c].Necessitats[n].Comprovat)
                        continue;

                    if (poble[p].Cases[c].Necessitats[n].Producte != producte)
                        continue;

                    if (poble[p].Cases[c].Necessitats[n].TeProveidor)
                    {
                        if(poble[p].Cases[c].Necessitats[n].Proveidor == productor.Coordenades)
                        {
                            poble[p].Cases[c].Necessitats[n].Comprovat = true;

                            Debug.LogError($"Recuros ja donat a la casa {c} de la peça {poble[p].gameObject.name}", poble[p].gameObject);
                            XS_Coroutine.StartCoroutine(Animacio_ProductesExtrets(productor, poble[p].Cases[c].Peça.transform, c + n));
                            casesProveides.Add(poble[p].Cases[c]);
                            proveit = true;
                            break;
                        }
                        continue;
                    }
                   
                    poble[p].Cases[c].Necessitats[n].Proveir(productor.Coordenades);
                    poble[p].Cases[c].Necessitats[n].Comprovat = true;

                    Debug.LogError($"Donat un recuros a la casa {c} de la peça {poble[p].gameObject.name}", poble[p].gameObject);
                    XS_Coroutine.StartCoroutine(Animacio_ProductesExtrets(productor, poble[p].Cases[c].Peça.transform, c + n));
                    casesProveides.Add(poble[p].Cases[c]);
                    proveit = true;

                    if (proveit)
                        break;
                }
                if (proveit)
                    break;
            }
            if (proveit) 
                break;
        }

        //ResetStepTime();
        return proveit;
    }

    //void TempsDeVisualitzacio() { }
    //void TempsDeVisualitzacio() => stepTime = 1;
    //void ResetStepTime() => stepTime = 0.1f;

    IEnumerator Animacio_ProductesExtrets(Peça productor, Transform casa, float index)
    {
        GameObject producte = productor.Producte.Subestat.MostrarInformacio(productor.Producte)[0];
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
