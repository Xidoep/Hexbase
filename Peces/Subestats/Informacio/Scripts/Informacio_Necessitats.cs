using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{
    [SerializeField] Grups grups;
    [SerializeField] UI_InformacioPeça prefab;

    int quantitat = 0;
    public override void Mostrar(Peça peça, bool mostrarProveides = false) 
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        if (!peça.TeCasa)
            return ui.ToArray();
        */

        if (!peça.TeCasa)
            return;

        Amagar(peça);

        quantitat = peça.Casa.Necessitats.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && peça.Casa.Necessitats[i].Proveit)
                continue;

            GameObject tmp = Instantiate(prefab.gameObject, peça.transform.position, Quaternion.identity, peça.transform);
            tmp.GetComponent<UI_InformacioPeça>().Setup(peça, i);
            tmp.transform.GetChild(0).transform.localPosition = DesplaçamentLateral(tmp.transform, quantitat, i);
            peça.Casa.Necessitats[i].Informacio = new Unitat(tmp);
            Debug.LogError("Mostrar",peça);

            //ui.Add(peça.Casa.Necessitats[i].Informacio);
        }

        //grups.ResaltarGrup(peça);
        //return ui.ToArray();
    }

    public override void Amagar(Peça peça)
    {
        for (int i = 0; i < peça.Casa.Necessitats.Length; i++)
        {
            if (peça.Casa.Necessitats[i].Informacio.gameObject == null)
                continue;

            Destroy(peça.Casa.Necessitats[i].Informacio.gameObject, 0.1f);
        }
        Debug.LogError("Amagar", peça);
        //grups.ReixarDeResaltar();
    }

}
