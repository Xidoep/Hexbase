using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{
    [SerializeField] Grups grups;
    [SerializeField] UI_InformacioPeça prefab;

    int quantitat = 0;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false) 
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        if (!peça.TeCasa)
            return ui.ToArray();
        */
        /*
        if (!((Peça)hexagon).TeCasa)
            return;

        Amagar(hexagon);

        quantitat = ((Peça)hexagon).CasesLength;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && ((Peça)hexagon).Cases[i].Necessitats[0].Proveit)
                continue;

            GameObject tmp = Instantiate(prefab.gameObject, hexagon.transform.position, Quaternion.identity, hexagon.transform);
            tmp.GetComponent<UI_InformacioPeça>().Setup(((Peça)hexagon), i);
            tmp.transform.GetChild(0).transform.localPosition = DesplaçamentLateral(tmp.transform, quantitat, i);
            ((Peça)hexagon).Cases[i].Necessitats[0].Informacio = new Unitat(tmp);
            Debug.LogError("Mostrar",hexagon);

            //ui.Add(peça.Casa.Necessitats[i].Informacio);
        }
        */
        //grups.ResaltarGrup(peça);
        //return ui.ToArray();
    }

    public override void Amagar(Hexagon hexagon)
    {
        /*
        for (int i = 0; i < ((Peça)hexagon).CasesLength; i++)
        {
            if (((Peça)hexagon).Cases[i].Necessitats[0].Informacio.gameObject == null)
                continue;

            Destroy(((Peça)hexagon).Cases[i].Necessitats[0].Informacio.gameObject, 0.1f);
        }
        Debug.LogError("Amagar", hexagon);
        */
        //grups.ReixarDeResaltar();
    }

}
