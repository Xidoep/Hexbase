using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Subestat casa;
    [SerializeField] Detall_Tiles_Estat detall_Tiles;
    [SerializeField] Producte necessitatInicial;

    [Nota("Nom�s per debugging",NoteType.Warning)]
    [SerializeField] List<Pe�a> cases;

    //INTERN
    List<Pe�a> veins;
    int casesVeines = 0;

    public Producte NecessitatInicial => necessitatInicial;

    private void OnEnable()
    {
        cases = new List<Pe�a>();
    }

    public void Proces(List<Pe�a> peces, System.Action enFinalitzar)
    {
        Debugar.LogError("--------------REPOBLAR---------------");
        List<int> cases = new List<int>();
        for (int p = 0; p < peces.Count; p++)
        {
            if (!peces[p].SubestatIgualA(casa))
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPe�a;
            
            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].SubestatIgualA(casa)) casesVeines++;
            }

            //Afegeix la seva Casa, i utilitza del DETALL_TILES de les cases veines per afegir mes cases.
            CanviarNecessitats(peces[p], 1 + casesVeines);
        }

        if (enFinalitzar != null) enFinalitzar.Invoke();
    }

    public void CanviarNecessitats(Pe�a pe�a, int necessitats)
    {
        if (necessitats > pe�a.Casa.Necessitats.Length) //S'han de crear tantes cases com fagi falta
        {
            for (int i = pe�a.Casa.Necessitats.Length; i < necessitats; i++)
            {
                pe�a.Casa.AfegirNecessitat(necessitatInicial);
            }
        }
        else if(necessitats < pe�a.Casa.Necessitats.Length)
        {
            for (int i = necessitats; i < pe�a.Casa.Necessitats.Length; i++)
            {
                pe�a.Casa.TreureNecessitat();
            }
        } //S'han de borrar tantes cases com faci falta;

        if (!this.cases.Contains(pe�a)) this.cases.Add(pe�a);
    }
}


