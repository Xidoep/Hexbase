using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Estat casa;
    [SerializeField] Detall_Tiles_Estat detall_Tiles;

    [Nota("Només per debugging",NoteType.Warning)]
    [SerializeField] List<Peça> cases;

    //INTERN
    int[] veins;

    private void OnEnable()
    {
        cases = new List<Peça>();
    }

    public void Proces(List<Peça> peces)
    {
        Debug.LogError("--------------REPOBLAR---------------");
        for (int p = 0; p < peces.Count; p++)
        {
            if (!peces[p].EstatIgualA(casa))
                continue;

            AfegirCasa(peces[p], 1 + detall_Tiles.Get(peces[p]).Length);
        }
    }

    public void AfegirCasa(Peça peça, int cases)
    {
        if (cases > peça.CasesCount) //S'han de crear tantes cases com fagi falta
        {
            for (int i = peça.CasesCount; i < cases; i++)
            {
                peça.AddCasa();
                //Crear
            }
        }
        else if(cases < peça.CasesCount)
        {
            for (int i = cases; i < peça.CasesCount; i++)
            {
                peça.RemoveCasa();
                //Remove
            }
        } //S'han de borrar tantes cases com faci falta;

        if (!this.cases.Contains(peça)) this.cases.Add(peça);
    }
}


