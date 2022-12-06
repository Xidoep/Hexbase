using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Subestat
{
    [Apartat("INFROMACIO")]
    [SerializeField] GameObject uiNecessitat_prefab;

    //INTERNES
    List<GameObject> necessitats;
    List<Casa> casesNoProveides;

    public override GameObject[] MostrarInformacio(Pe�a pe�a)
    {
        necessitats = new List<GameObject>();
        List<Casa> casesNoProveides = new List<Casa>();
        if (pe�a.CasesCount == 0)
            return necessitats.ToArray();

        for (int i = 0; i < pe�a.Cases.Count; i++)
        {
            if (!pe�a.Cases[i].Proveit) casesNoProveides.Add(pe�a.Cases[i]);
        }

        for (int i = 0; i < casesNoProveides.Count; i++)
        {
            //if (!pe�a.Cases[i].Proveit)
            necessitats.Add(Instantiate(uiNecessitat_prefab, pe�a.transform.position + Despla�amentLateral(i), Quaternion.identity, pe�a.transform).GetComponent<UI_Necessitat>().Setup(pe�a.Cases[i], Rotacio(i)));
        }

        return necessitats.ToArray();
    }


    new void OnValidate()
    {
        base.OnValidate();
    }

    Vector3 Despla�amentLateral(int i)
    {
        return ((Vector3.right * (((i + 1) / 2) * 0.5f)) * (i % 2 == 0 ? 1 : -1)) 
            + (Vector3.down * (i * i * 0.025f));
    }
    float Rotacio(int i)
    {
        return (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);
    }
}
