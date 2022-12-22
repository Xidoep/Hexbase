using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{

    public override Unitat[] Mostrar(Peça peça, bool mostrarProveides = false)
    {
        if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        quantitat = peça.productesExtrets.Length;
        for (int i = 0; i < quantitat; i++)
        {
            GameObject tmp = Instantiate(Prefab, peça.transform.position, Quaternion.identity, peça.transform);
            tmp.GetComponent<UI_InformacioPeça>().Setup(peça, i);
            tmp.transform.GetChild(0).transform.position = peça.transform.position + DesplaçamentLateral(quantitat, i);

            ui.Add(new Unitat(tmp, i));
        }
        return ui.ToArray();
    }

}
