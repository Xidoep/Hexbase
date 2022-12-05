using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Subestat
{
    [Apartat("INFROMACIO")]
    [SerializeField] GameObject uiNecessitat_prefab;

    List<GameObject> necessitats;

    public override GameObject[] MostrarInformacio(Peça peça)
    {
        necessitats = new List<GameObject>();
        if(peça.CasesCount > 0)
        {
            for (int i = 0; i < peça.Cases.Count; i++)
            {
                necessitats.Add(Instantiate(uiNecessitat_prefab, peça.transform.position, Quaternion.identity, peça.transform).GetComponent<UI_Necessitat>().Setup(peça.Cases[i]));
            }
        }
        return necessitats.ToArray();
    }


    new void OnValidate()
    {
        base.OnValidate();
    }
}
