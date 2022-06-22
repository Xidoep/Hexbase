using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Grups")]
public class Grups : ScriptableObject
{
    //*************************************************************************************************
    //FALTA: Que al buscar grups, agafi els grups de les peces adjacents a la peça que proves. No tots
    //*************************************************************************************************

    [SerializeField] List<Grup> grups;
    [SerializeField] Proximitat proximitat;

    //INTERN
    List<Hexagon> veinsIguals;
    bool agrupada = false;
    int primerGrup = 0;
    void OnEnable()
    {
        grups = new List<Grup>();
    }

    public void Agrupdar(Peça peça)
    {
        if (peça == null)
            return;

        veinsIguals = new List<Hexagon>();
        for (int v = 0; v < peça.Veins.Length; v++)
        {
            if (peça.Estat == peça.Veins[v].Estat) veinsIguals.Add(peça.Veins[v]);
        }


        if (veinsIguals.Count > 0)
        {
            //Buscar el grup de la primera peça, i agafeix aquesta al grup.
            //Si hi ha mes d'una peça, busca el grup de les peces restants i agrunta els grups amb els de la primera.
            agrupada = false;
            primerGrup = 0;
            for (int g = 0; g < grups.Count; g++)
            {
                if (grups[g].peces.Contains((Peça)veinsIguals[0]))
                {
                    //aquest es el grup del primer vei igual.
                    AfegirAGrup(peça, grups[g]);
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
                            if (grups[g].peces.Contains((Peça)veinsIguals[v]))
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
            CrearNouGrup(peça);
        }

        Debug.Log($"Agrupdar estat: {peça.name}");
        proximitat.Add(peça);
    }
    void CrearNouGrup(Peça peça)
    {
        if(grups == null)
        {
            grups = new List<Grup>();
        }
        Grup tmp = new Grup();
        tmp.peces = new List<Peça>() { peça };
        tmp.estat = peça.Estat;
        grups.Add(tmp);
    }

    void AfegirAGrup(Peça peça, Grup grup) 
    {
        grup.peces.Add(peça);

        proximitat.Add(peça);
        for (int i = 0; i < grup.peces.Count; i++)
        {
            proximitat.Add(grup.peces[i]);
        }
    }

    int AjuntarGrups(Grup desti, Grup seleccionat)
    {
        Peça[] pecesTmp = seleccionat.peces.ToArray();
        for (int i = 0; i < pecesTmp.Length; i++)
        {
            desti.peces.Add(pecesTmp[i]);
        }
        grups.Remove(seleccionat);

        for (int i = 0; i < desti.peces.Count; i++)
        {
            proximitat.Add(desti.peces[i]);
        }



        return grups.IndexOf(desti);
    }

}



[System.Serializable]
public class Grup
{
    public EstatPeça estat;
    public List<Peça> peces;


}
