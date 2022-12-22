using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{

    public override Unitat[] Mostrar(Pe�a pe�a, bool mostrarProveides = false)
    {
        if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();

        quantitat = pe�a.productesExtrets.Length;
        for (int i = 0; i < quantitat; i++)
        {
            GameObject tmp = Instantiate(Prefab, pe�a.transform.position, Quaternion.identity, pe�a.transform);
            tmp.GetComponent<UI_InformacioPe�a>().Setup(pe�a, i);
            tmp.transform.GetChild(0).transform.position = pe�a.transform.position + Despla�amentLateral(quantitat, i);

            ui.Add(new Unitat(tmp, i));
        }
        return ui.ToArray();
    }

}
