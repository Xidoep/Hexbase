using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{

    public override GameObject[] Mostrar(Pe�a pe�a)
    {
        if (ui == null)
            ui = new List<GameObject>();
        else ui.Clear();
        for (int i = 0; i < pe�a.productesExtrets.Length; i++)
        {
            ui.Add(Instantiate(Prefab, pe�a.transform.position + Despla�amentLateral(i), Quaternion.Euler(0, 0, Rotacio(i)), pe�a.transform).GetComponent<UI_InformacioPe�a>().Setup(pe�a,i));
        }
        return ui.ToArray();
    }

}
