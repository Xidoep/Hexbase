using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Estat casa;
    [SerializeField] Detall_Tiles_Estat detall_Tiles;

    [Nota("Nom�s per debugging",NoteType.Warning)]
    [SerializeField] List<Pe�a> cases;

    //INTERN
    int[] veins;

    private void OnEnable()
    {
        cases = new List<Pe�a>();
    }

    public void Proces(List<Pe�a> peces)
    {
        Debug.LogError("--------------REPOBLAR---------------");
        for (int p = 0; p < peces.Count; p++)
        {
            if (!peces[p].EstatIgualA(casa))
                continue;

            AfegirCasa(peces[p], 1 + detall_Tiles.Get(peces[p]).Length);
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


