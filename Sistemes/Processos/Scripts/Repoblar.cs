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

    [Nota("Només per debugging",NoteType.Warning)]
    [SerializeField] List<Peça> cases;

    //INTERN
    List<Peça> veins;
    int casesVeines = 0;

    public Producte NecessitatInicial => necessitatInicial;

    private void OnEnable()
    {
        cases = new List<Peça>();
    }

    public void Proces(List<Peça> peces, System.Action enFinalitzar)
    {
        Debugar.LogError("--------------REPOBLAR---------------");
        List<int> cases = new List<int>();
        for (int p = 0; p < peces.Count; p++)
        {
            if (!peces[p].SubestatIgualA(casa))
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPeça;
            
            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].SubestatIgualA(casa)) casesVeines++;
            }

            //Afegeix la seva Casa, i utilitza del DETALL_TILES de les cases veines per afegir mes cases.
            CanviarNecessitats(peces[p], 1 + casesVeines);
        }

        if (enFinalitzar != null) enFinalitzar.Invoke();
    }

    public void CanviarNecessitats(Peça peça, int necessitats)
    {
        if (necessitats > peça.Casa.Necessitats.Length) //S'han de crear tantes cases com fagi falta
        {
            for (int i = peça.Casa.Necessitats.Length; i < necessitats; i++)
            {
                peça.Casa.AfegirNecessitat(necessitatInicial);
            }
        }
        else if(necessitats < peça.Casa.Necessitats.Length)
        {
            for (int i = necessitats; i < peça.Casa.Necessitats.Length; i++)
            {
                peça.Casa.TreureNecessitat();
            }
        } //S'han de borrar tantes cases com faci falta;

        if (!this.cases.Contains(peça)) this.cases.Add(peça);
    }
}


