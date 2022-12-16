using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{
    //INTERN
    List<GameObject> necessitats;

    public override GameObject[] Mostrar(Pe�a pe�a) 
    {
        if (necessitats == null)
            necessitats = new List<GameObject>();
        else necessitats.Clear();

        if (!pe�a.TeCasa)
            return necessitats.ToArray();

        for (int i = 0; i < pe�a.Casa.Necessitats.Length; i++)
        {
            necessitats.Add(Instantiate(Prefab, pe�a.transform.position + Despla�amentLateral(i), Quaternion.identity, pe�a.transform).GetComponent<UI_InformacioPe�a>().Setup(pe�a, i));
        }

        return necessitats.ToArray();
    }

}
