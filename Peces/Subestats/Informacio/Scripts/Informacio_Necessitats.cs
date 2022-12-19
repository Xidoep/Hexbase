using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{

    public override GameObject[] Mostrar(Peça peça) 
    {
        if (ui == null)
            ui = new List<GameObject>();
        else ui.Clear();

        if (!peça.TeCasa)
            return ui.ToArray();

        for (int i = 0; i < peça.Casa.Necessitats.Length; i++)
        {
            ui.Add(Instantiate(Prefab, peça.transform.position + DesplaçamentLateral(i), Quaternion.identity, peça.transform).GetComponent<UI_InformacioPeça>().Setup(peça, i));
        }

        return ui.ToArray();
    }

}
