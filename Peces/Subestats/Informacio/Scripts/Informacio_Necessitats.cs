using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{
    [SerializeField] Grups grups;
    [SerializeField] UI_InformacioPe�a prefab;

    int quantitat = 0;
    public override void Mostrar(Pe�a pe�a, bool mostrarProveides = false) 
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        if (!pe�a.TeCasa)
            return ui.ToArray();
        */

        if (!pe�a.TeCasa)
            return;

        Amagar(pe�a);

        quantitat = pe�a.Casa.Necessitats.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && pe�a.Casa.Necessitats[i].Proveit)
                continue;

            GameObject tmp = Instantiate(prefab.gameObject, pe�a.transform.position, Quaternion.identity, pe�a.transform);
            tmp.GetComponent<UI_InformacioPe�a>().Setup(pe�a, i);
            tmp.transform.GetChild(0).transform.localPosition = Despla�amentLateral(tmp.transform, quantitat, i);
            pe�a.Casa.Necessitats[i].Informacio = new Unitat(tmp);
            Debug.LogError("Mostrar",pe�a);

            //ui.Add(pe�a.Casa.Necessitats[i].Informacio);
        }

        //grups.ResaltarGrup(pe�a);
        //return ui.ToArray();
    }

    public override void Amagar(Pe�a pe�a)
    {
        for (int i = 0; i < pe�a.Casa.Necessitats.Length; i++)
        {
            if (pe�a.Casa.Necessitats[i].Informacio.gameObject == null)
                continue;

            Destroy(pe�a.Casa.Necessitats[i].Informacio.gameObject, 0.1f);
        }
        Debug.LogError("Amagar", pe�a);
        //grups.ReixarDeResaltar();
    }

}
