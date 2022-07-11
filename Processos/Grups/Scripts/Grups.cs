using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Grups")]
public class Grups : ScriptableObject
{
    //*************************************************************************************************
    //FALTA: Que al buscar grups, agafi els grups de les peces adjacents a la pe�a que proves. No tots
    //*************************************************************************************************

    [SerializeField] List<Grup> grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Estat estatCasa;
    //System.Action enFinalitzar;

    //INTERN
    List<Pe�a> veinsIguals;
    bool agrupada = false;
    int primerGrup = 0;
    Pe�a[] veins;

    int index;

    void OnEnable()
    {
        grups = new List<Grup>();
    }

    public void Agrupdar(Pe�a pe�a, System.Action<int> enFinalitzar)
    {
        if (pe�a == null)
            return;

        //this.enFinalitzar = enFinalitzar;
        if (veinsIguals == null) veinsIguals = new List<Pe�a>();
        else veinsIguals.Clear();

        veins = pe�a.VeinsPe�a;

        for (int v = 0; v < veins.Length; v++)
        {
            if (pe�a.Estat == veins[v].Estat) veinsIguals.Add(veins[v]);
        }


        if (veinsIguals.Count > 0)
        {
            //Buscar el grup de la primera pe�a, i agafeix aquesta al grup.
            //Si hi ha mes d'una pe�a, busca el grup de les peces restants i agrunta els grups amb els de la primera.
            agrupada = false;
            primerGrup = 0;
            for (int g = 0; g < grups.Count; g++)
            {
                if (grups[g].peces.Contains((Pe�a)veinsIguals[0]))
                {
                    //aquest es el grup del primer vei igual.
                    AfegirAGrup(pe�a, grups[g]);
                    agrupada = true;
                    primerGrup = g;
                    veinsIguals.RemoveAt(0);
                    break;
                }
            }

            if (agrupada)
            {
                if(veinsIguals.Count > 0)
                {
                    for (int g = 0; g < grups.Count; g++)
                    {
                        for (int v = 0; v < veinsIguals.Count; v++)
                        {
                            //Aquest son els grups dels altres veins iguals
                            if (grups[g].peces.Contains(veinsIguals[v]))
                            {
                                if (primerGrup.Equals(g))
                                    continue;

                                Debug.LogError($"Juntar grup {primerGrup} i {g}");
                                primerGrup = AjuntarGrups(grups[primerGrup], grups[g]);
                            }
                        }
                    }
                }
               
            }

        }
        else
        {
            //no ha trobat cap vei igual
            CrearNouGrup(pe�a);
        }

        Debug.Log($"Agrupdar estat: {pe�a.name}");
        enFinalitzar.Invoke(index);
        //proximitat.Add(pe�a, enFinalitzar);
    }


    void CrearNouGrup(Pe�a pe�a)
    {
        if(grups == null)
        {
            grups = new List<Grup>();
        }
        Grup tmp = new Grup
        {
            peces = new List<Pe�a>() { pe�a },
            estat = pe�a.Estat,
            esPoble = pe�a.EstatIgualA(estatCasa)
        };
        grups.Add(tmp);
    }

    void AfegirAGrup(Pe�a pe�a, Grup grup) 
    {
        grup.peces.Add(pe�a);

        /*proximitat.Add(pe�a);
        for (int i = 0; i < grup.peces.Count; i++)
        {
            proximitat.Add(grup.peces[i]);
        }*/
    }

    int AjuntarGrups(Grup desti, Grup seleccionat)
    {
        Pe�a[] pecesTmp = seleccionat.peces.ToArray();
        for (int i = 0; i < pecesTmp.Length; i++)
        {
            desti.peces.Add(pecesTmp[i]);
        }
        grups.Remove(seleccionat);

        /*for (int i = 0; i < desti.peces.Count; i++)
        {
            proximitat.Add(desti.peces[i]);
        }*/



        return grups.IndexOf(desti);
    }

    public Grup GrupOf(Pe�a pe�a)
    {
        int index = 0;
        for (int i = 0; i < grups.Count; i++)
        {
            if (grups[i].peces.Contains(pe�a))
            {
                index = i;
                break;
                
            }
        }
        return grups[index];
    }
    public List<Pe�a> Peces(int index) => grups[index].peces; 
}



[System.Serializable]
public class Grup
{
    public Estat estat;
    public List<Pe�a> peces;

    public bool esPoble = false;
}
