using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Subestat
{
    [Apartat("INFROMACIO")]
    [SerializeField] GameObject uiNecessitat_prefab;

    List<GameObject> necessitats;

    public override GameObject[] MostrarInformacio(Pe�a pe�a)
    {
        necessitats = new List<GameObject>();
        if(pe�a.CasesCount > 0)
        {
            for (int i = 0; i < pe�a.Cases.Count; i++)
            {
                necessitats.Add(Instantiate(uiNecessitat_prefab, pe�a.transform.position, Quaternion.identity, pe�a.transform).GetComponent<UI_Necessitat>().Setup(pe�a.Cases[i]));
            }
        }
        return necessitats.ToArray();
    }


    new void OnValidate()
    {
        base.OnValidate();
    }
}
