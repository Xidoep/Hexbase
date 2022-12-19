using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{

    public override GameObject[] Mostrar(Pe�a pe�a) 
    {
        if (ui == null)
            ui = new List<GameObject>();
        else ui.Clear();

        if (!pe�a.TeCasa)
            return ui.ToArray();

        for (int i = 0; i < pe�a.Casa.Necessitats.Length; i++)
        {
            ui.Add(Instantiate(Prefab, pe�a.transform.position + Despla�amentLateral(i), Quaternion.identity, pe�a.transform).GetComponent<UI_InformacioPe�a>().Setup(pe�a, i));
        }

        return ui.ToArray();
    }

}
