using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{
    [SerializeField] Grups grups;
    [SerializeField] UI_InformacioPe�a prefab;

    int quantitat = 0;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false) 
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        if (!pe�a.TeCasa)
            return ui.ToArray();
        */
        /*
        if (!((Pe�a)hexagon).TeCasa)
            return;

        Amagar(hexagon);

        quantitat = ((Pe�a)hexagon).CasesLength;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && ((Pe�a)hexagon).Cases[i].Necessitats[0].Proveit)
                continue;

            GameObject tmp = Instantiate(prefab.gameObject, hexagon.transform.position, Quaternion.identity, hexagon.transform);
            tmp.GetComponent<UI_InformacioPe�a>().Setup(((Pe�a)hexagon), i);
            tmp.transform.GetChild(0).transform.localPosition = Despla�amentLateral(tmp.transform, quantitat, i);
            ((Pe�a)hexagon).Cases[i].Necessitats[0].Informacio = new Unitat(tmp);
            Debug.LogError("Mostrar",hexagon);

            //ui.Add(pe�a.Casa.Necessitats[i].Informacio);
        }
        */
        //grups.ResaltarGrup(pe�a);
        //return ui.ToArray();
    }

    public override void Amagar(Hexagon hexagon)
    {
        /*
        for (int i = 0; i < ((Pe�a)hexagon).CasesLength; i++)
        {
            if (((Pe�a)hexagon).Cases[i].Necessitats[0].Informacio.gameObject == null)
                continue;

            Destroy(((Pe�a)hexagon).Cases[i].Necessitats[0].Informacio.gameObject, 0.1f);
        }
        Debug.LogError("Amagar", hexagon);
        */
        //grups.ReixarDeResaltar();
    }

}
