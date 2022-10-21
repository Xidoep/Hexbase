using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Subestat casa;
    [SerializeField] Detall_Tiles_Estat detall_Tiles;

    [Nota("Nom�s per debugging",NoteType.Warning)]
    [SerializeField] List<Pe�a> cases;

    //INTERN
    List<Pe�a> veins;
    int casesVeines = 0;

    private void OnEnable()
    {
        cases = new List<Pe�a>();
    }

    public void Proces(List<Pe�a> peces)
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
            //AfegirCasa(peces[p], 1 + detall_Tiles.Get(peces[p]).Length);
            AfegirCasa(peces[p], 1 + casesVeines);
        }
    }

    public void AfegirCasa(Pe�a pe�a, int cases)
    {
        if (cases > pe�a.CasesCount) //S'han de crear tantes cases com fagi falta
        {
            for (int i = pe�a.CasesCount; i < cases; i++)
            {
                pe�a.AddCasa();
                //Crear
            }
        }
        else if(cases < pe�a.CasesCount)
        {
            for (int i = cases; i < pe�a.CasesCount; i++)
            {
                pe�a.RemoveCasa();
                //Remove
            }
        } //S'han de borrar tantes cases com faci falta;

        if (!this.cases.Contains(pe�a)) this.cases.Add(pe�a);
    }
}


