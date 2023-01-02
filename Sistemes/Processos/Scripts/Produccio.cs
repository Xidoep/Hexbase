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

        if (casesProveides == null) 
            casesProveides = new List<Peça>();
        else 
            casesProveides.Clear();

        if (visualitzacioProducte == null) visualitzacioProducte = new List<Visualitzacions.Producte>();
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
        //Peça.ProducteExtret[] productes = productors[index].Extraccio.productesExtrets;
        visualitzacioProducte.Clear();

        //Comprovar si cal visualitzar
        bool calVisualitzar = false;
        for (int i = 0; i < productors[index].Extraccio.productesExtrets.Length; i++)
        {
            if (!productors[index].Extraccio.productesExtrets[i].gastat)
            {
                calVisualitzar = true;
                productors[index].Extraccio.mostrarInformacio?.Invoke(productors[index].Extraccio, true);
                productors[index].Extraccio.BlocarInformacio = true;
                break;
            }
        }

        if (calVisualitzar)
        {
            for (int i = 0; i < productors[index].Extraccio.productesExtrets.Length; i++)
            {
                if (productors[index].Extraccio.productesExtrets[i].gastat)
                    continue;

                Peça proveida = BuscarCasaDesproveida(productors[index], productors[index].Extraccio.productesExtrets[i].producte, out int indexNecessitat);
                if (proveida != null)
                {
                    proveida.mostrarInformacio?.Invoke(proveida, true);
                    proveida.BlocarInformacio = true;
                    resoldre.Nivell.GuanyarExperiencia(1);
                    casesProveides.Add(proveida);

                    productors[index].Extraccio.productesExtrets[i].gastat = true;
                }
                visualitzacioProducte.Add(new Visualitzacions.Producte(productors[index], i, proveida, indexNecessitat));
            }
        }

        for (int i = 0; i < visualitzacioProducte.Count; i++)
        {
            visualitzacions.Produccio(visualitzacioProducte[i], i == visualitzacioProducte.Count - 1, MostrarInformacioFinal);
        }


        /*Debug.Log($"{productes.Length} productes");
        for (int i = 0; i < productes.Length; i++)
        {
            if (productes[i].gastat)
            {
                visualitzacioProducte.Add(new Visualitzacions.Producte(null, -1, null, -1));
                continue;
            }

            calVisualitzar = true;
            Peça proveida = BuscarCasaDesproveida(productors[index], productes[i].producte, out int indexNecessitat);
            if (proveida != null)
            {
                resoldre.Nivell.GuanyarExperiencia(1);
                casesProveides.Add(proveida);
                //proveida.BlocarInformacio = true;
                productes[i].gastat = true;
            }
            visualitzacioProducte.Add(new Visualitzacions.Producte(productors[index], i, proveida, indexNecessitat));
        }


        //Crear icones
        Debug.Log($"{visualitzacioProducte.Count} visualitzacions");
        //Animar
        if (calVisualitzar)
        {

            for (int i = 0; i < visualitzacioProducte.Count; i++)
            {
                visualitzacions.Produccio(visualitzacioProducte[i]);
            }
        }*/

        index++;
        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }

    void MostrarInformacioFinal()
    {
        for (int i = 0; i < casesProveides.Count; i++)
        {
            casesProveides[i].mostrarInformacio?.Invoke(casesProveides[i], false);
        }
        for (int i = 0; i < productors.Count; i++)
        {
            productors[i].Extraccio.mostrarInformacio?.Invoke(productors[i].Extraccio, false);
        }
    }


    Peça BuscarCasaDesproveida(Peça productor, Producte producte, out int index)
    {
        Peça casa = null;
        int _index = -1;
        //string debug = "PRODUCCIO DEBUG\n";
        connexions = grups.GrupByPeça(productor).connexionsId;
        for (int con = 0; con < connexions.Count; con++)
        {
            List<Peça> poble = grups.GrupById(connexions[con]).Peces;

            for (int p = 0; p < poble.Count; p++)
            {
                //debug += $"Casa {p}\n";
                if (!poble[p].TeCasa)
                    continue;

                for (int n = 0; n < poble[p].Casa.Necessitats.Length; n++)
                {
                    //debug += $"Te {poble[p].Casa.Necessitats.Length} necessitats";
                    if (poble[p].Casa.Necessitats[n].Producte == producte && !poble[p].Casa.Necessitats[n].Proveit)
                    {
                        //debug += $" ***No estava proveida, per tant la proveixo***";
                        //poble[p].mostrarInformacio?.Invoke(poble[p], true);
                        poble[p].Casa.Necessitats[n].Proveir();
                        _index = n;
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
