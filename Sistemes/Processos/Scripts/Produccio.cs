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
    List<VisualitzacioProducte> visualitzacioProducte;




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
        if (casesProveides == null) casesProveides = new List<Peça>();
        if (visualitzacioProducte == null) visualitzacioProducte = new List<VisualitzacioProducte>();
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
        Peça.ProducteExtret[] productes = productors[index].Extraccio.productesExtrets;
        visualitzacioProducte.Clear();

        Informacio.Unitat[] infoProductes = productors[index].Extraccio.Subestat.InformacioMostrar(null, productors[index].Extraccio, true);

        Debug.Log($"{productes.Length} productes");
        for (int i = 0; i < productes.Length; i++)
        {
            if (productes[i].gastat)
            {
                visualitzacioProducte.Add(new VisualitzacioProducte(null, null, null, -1));
                continue;
            }

            Peça proveida = BuscarCasaDesproveida(productors[index], productes[i].producte, out int indexNecessitat);
            if (proveida != null)
            {
                resoldre.Nivell.GuanyarExperiencia(1);
                casesProveides.Add(proveida);
                productes[i].gastat = true;
            }
            visualitzacioProducte.Add(new VisualitzacioProducte(productors[index].Extraccio, productors[index], proveida, indexNecessitat));
        }


        //Crear icones
        Debug.Log($"{visualitzacioProducte.Count} visualitzacions");
        //Animar
        for (int i = 0; i < visualitzacioProducte.Count; i++)
        {
            visualitzacions.Produccio(
                                    infoProductes[i].gameObject,
                                    visualitzacioProducte[i].productor,
                                    i,
                                    visualitzacioProducte[i].casa,
                                    visualitzacioProducte[i].indexNecessitat);
        }

        index++;
        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
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









    struct VisualitzacioProducte
    {
        public VisualitzacioProducte(Peça extractor, Peça productor, Peça casa, int indexNecessitat)
        {
            this.extractor = extractor;
            this.productor = productor;
            this.casa = casa;
            this.indexNecessitat = indexNecessitat;
        }
        public Peça extractor;
        public Peça productor;
        public Peça casa;
        public int indexNecessitat;
    }

}
