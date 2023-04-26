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

    [Nota("Ara es mostra només per debugar", NoteType.Warning)]
    [SerializeField] List<Peça> productors;
    
    
    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] Estat cami;
    [SerializeField] Subestat casa;

    [Space(30)]
    [SerializeField] Visualitzacions visualitzacions;
    /*
    [Apartat("ANIMACIONS")]
    [SerializeField] Animacio_Scriptable producteProveir;
    [SerializeField] Animacio_Scriptable necessitatProveida;

    [SerializeField] Utils_InstantiableFromProject EfecteGuanyarPunts;
    */
    //PROPIETATS
    bool Finalitzat => index == productors.Count;

    //INTERN
    System.Action enFinalitzar;
    int index;
    List<Peça> veins;
    List<string> connexions;
    float stepTime = 0.1f;
    List<Peça> casesProveides;
    List<Peça> productorsActualitzables;
    Vector3 offset;
    public List<Visualitzacions.Producte> visualitzacioProducte;

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


    public void Process(System.Action enFinalitzar)
    {
        Debugar.LogError("--------------PRODUCCIO---------------");
        index = 0;
        productesAVisualitzar = 0;
        this.enFinalitzar = enFinalitzar;

        if (casesProveides == null) 
            casesProveides = new List<Peça>();
        else 
            casesProveides.Clear();

        if (visualitzacioProducte == null) visualitzacioProducte = new List<Visualitzacions.Producte>();
        //CleanAllNeeds();
        Step();
    }


    void StepRecepta()
    {
        if(productors.Count == 0 || Finalitzat)
        {
            enFinalitzar.Invoke();
            return;
        }

        //Si el productor no te productes, salta al seguent.
        //Estaria be borrarlo per optmitzar

        //Agafar tots els productes d'un productor.

    }

    void Step()
    {
        if (productors.Count == 0 || Finalitzat)
        {
            enFinalitzar.Invoke();
            return;
        }

        visualitzacioProducte.Clear();

        //COMPROVAR SI L'HI QUEDEN PRODUCTES SENSE GASTAR
        bool hiHaProductePerGastar = false;
        for (int i = 0; i < productors[index].Connexio.ProductesExtrets.Length; i++)
        {
            if (!productors[index].Connexio.ProductesExtrets[i].gastat)
            {
                hiHaProductePerGastar = true;
                productors[index].Connexio.MostrarInformacio?.Invoke(productors[index].Connexio, true);
                productors[index].Connexio.SetBlocarInformacio = true;
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

                Peça proveida = BuscarCasaDesproveida(productors[index], productors[index].Connexio.ProductesExtrets[i].producte, out int indexNecessitat);
                if (proveida != null)
                {
                    proveida.MostrarInformacio?.Invoke(proveida, true);
                    proveida.SetBlocarInformacio = true;
                    resoldre.Nivell.GuanyarExperiencia(1);
                    casesProveides.Add(proveida);

                    productors[index].Connexio.ProductesExtrets[i].gastat = true;
                }
                visualitzacioProducte.Add(new Visualitzacions.Producte(productors[index], i, proveida, indexNecessitat));
            }



            index++;

            //VISUALITZAR
            for (int i = 0; i < visualitzacioProducte.Count; i++)
            {
                Debugar.Log($"index = {index}. Productors = {productors.Count}|| i = {i}. visualitzacioProducte = {visualitzacioProducte.Count - 1}|| {index == productors.Count && i == visualitzacioProducte.Count - 1}");
                visualitzacions.Produccio(visualitzacioProducte[i], index == productors.Count && i == visualitzacioProducte.Count - 1, MostrarInformacioFinal);
            }

        }
        else
        {
            //ESBORRAR PRODUCTORS QUE JA NO CALGUI VISUALITZAR (ESGOTATS)
            productors.RemoveAt(index);
        }

        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }


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


    Peça BuscarCasaDesproveidaRecepta(Peça productor, Producte producte, out int index)
    {
        /*
         * Hauria de ser...
         * a vera...
         * fins ara tenia que, cada productor, agafava els seus productes i buscava els cases connectades i el si enviava un prodcte,
         * si la necessitat de la casa i el producte que produi eren iguals
         * Ara tenim varis problemes:
         * no se quines necessitats te cada casa, ni forma de saber-ho.
         * les necessitats poden ser 1 o 2 o mes, i no necessariament del mateix tipus (referintme a productes) Mai podran demanar un estat i un producte. Prohibit!!!
         * Puc saber quan una de les receptes d'una peça es compleix, pero no quina d'elles ho fa.
         *  per tant, no se quins productes s'utilitzen per completar la recepta.
         * Tampoc es poden acumular productes, osigui, que si una casa necessita 2 productes, no en pot donar 1 una produccio i esperar pel seguent...
         * 
         * Bueno, a no ser que crei una clase intermitja entre la casa i la recepta que es digui: Necessitat.
         * La necessitat tindrà una recepta que espera complir, i una llista de productes acumulats.
         * Els productes acumulats es mantenen allà fins que es compleix la condicio.
         * Així puc mantenir el workflow d'ara, i que es mes facil...
         * Potser pero, que un prodcte es quedi encallat en una casa sense que hi arribin els seguents...
         * Espera, aixo vol dir pero, que cada casa necessita el seu processador.
         * I aquesta classe intermitja haurà de intentar processsar els elements que...
         * A no ser... que aquesta classe i la casa per extensio guardin informacio de la peça on estan.
         * 
         */


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
                    poble[p].Cases[c].need.Proveir(producte);
                    //------------------------------------------------nou


                    //debug += $"Te {poble[p].Casa.Necessitats.Length} necessitats";
                    if (poble[p].Cases[c].Necessitats[0].Producte == producte && !poble[p].Cases[c].Necessitats[0].Proveit)
                    {
                        //debug += $" ***No estava proveida, per tant la proveixo***";
                        //poble[p].mostrarInformacio?.Invoke(poble[p], true);
                        poble[p].Cases[c].Proveir();
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
                        poble[p].Cases[c].Proveir();
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










}
