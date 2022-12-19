using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{

    public override GameObject[] Mostrar(Peça peça)
    {
        if (ui == null)
            ui = new List<GameObject>();
        else ui.Clear();
        for (int i = 0; i < peça.productesExtrets.Length; i++)
        {
            ui.Add(Instantiate(Prefab, peça.transform.position + DesplaçamentLateral(i), Quaternion.Euler(0, 0, Rotacio(i)), peça.transform).GetComponent<UI_InformacioPeça>().Setup(peça,i));
        }
        return ui.ToArray();
    }

}
