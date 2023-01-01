using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{

    public override Unitat[] Mostrar(Pe�a pe�a, bool mostrarProveides = false) 
    {
        if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        if (!pe�a.TeCasa)
            return ui.ToArray();

        quantitat = pe�a.Casa.Necessitats.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && pe�a.Casa.Necessitats[i].Proveit)
                continue;

            GameObject tmp = Instantiate(Prefab, pe�a.transform.position, Quaternion.identity, pe�a.transform);
            tmp.GetComponent<UI_InformacioPe�a>().Setup(pe�a, i);
            tmp.transform.GetChild(0).transform.localPosition = Despla�amentLateral(tmp.transform, quantitat, i);
            //Debug.LogError(tmp.transform.GetChild(0).name);

            ui.Add(new Unitat(tmp,i));
        }

        return ui.ToArray();
    }

}
