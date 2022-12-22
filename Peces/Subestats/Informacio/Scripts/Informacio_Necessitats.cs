using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{

    public override Unitat[] Mostrar(Peça peça, bool mostrarProveides = false) 
    {
        if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        if (!peça.TeCasa)
            return ui.ToArray();

        quantitat = peça.Casa.Necessitats.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && peça.Casa.Necessitats[i].Proveit)
                continue;

            GameObject tmp = Instantiate(Prefab, peça.transform.position, Quaternion.identity, peça.transform);
            tmp.GetComponent<UI_InformacioPeça>().Setup(peça, i);
            tmp.transform.GetChild(0).transform.position = peça.transform.position + DesplaçamentLateral(quantitat, i);

            ui.Add(new Unitat(tmp,i));
        }

        return ui.ToArray();
    }

    //S'ha de mostrar quan?
    //Quan les necessitats es modifiquen de qualsevol manera.
    //Quan una casa es proveida. Desapareix amb una animacio xula.
    //Quan se'n crea una de nova ha de tenir una animacio especial i potser un so

}
