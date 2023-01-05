using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{

    public override void Mostrar(Peça peça, bool mostrarProveides = false)
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();
        */

        Debug.LogError("Extreure producte", peça);

        Amagar(peça);

        quantitat = peça.productesExtrets.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && peça.productesExtrets[i].gastat)
                continue;

            GameObject tmp = Instantiate(Prefab, peça.transform.position, Quaternion.identity, peça.transform);
            tmp.GetComponent<UI_InformacioPeça>().Setup(peça, i);
            tmp.transform.GetChild(0).transform.localPosition = DesplaçamentLateral(tmp.transform, quantitat, i);
            peça.productesExtrets[i].informacio = new Unitat(tmp);
            //ui.Add(peça.productesExtrets[i].informacio);
        }
        //return ui.ToArray();
    }

    public override void Amagar(Peça peça)
    {
        for (int i = 0; i < peça.productesExtrets.Length; i++)
        {
            if (peça.productesExtrets[i].informacio.gameObject == null)
                continue;

            Destroy(peça.productesExtrets[i].informacio.gameObject, 0.1f);
        }
    }
}
