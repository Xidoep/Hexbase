using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{

    public override void Mostrar(Pe�a pe�a, bool mostrarProveides = false)
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();
        */

        Debug.LogError("Extreure producte", pe�a);

        Amagar(pe�a);

        quantitat = pe�a.productesExtrets.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && pe�a.productesExtrets[i].gastat)
                continue;

            GameObject tmp = Instantiate(Prefab, pe�a.transform.position, Quaternion.identity, pe�a.transform);
            tmp.GetComponent<UI_InformacioPe�a>().Setup(pe�a, i);
            tmp.transform.GetChild(0).transform.localPosition = Despla�amentLateral(tmp.transform, quantitat, i);
            pe�a.productesExtrets[i].informacio = new Unitat(tmp);
            //ui.Add(pe�a.productesExtrets[i].informacio);
        }
        //return ui.ToArray();
    }

    public override void Amagar(Pe�a pe�a)
    {
        for (int i = 0; i < pe�a.productesExtrets.Length; i++)
        {
            if (pe�a.productesExtrets[i].informacio.gameObject == null)
                continue;

            Destroy(pe�a.productesExtrets[i].informacio.gameObject, 0.1f);
        }
    }
}
