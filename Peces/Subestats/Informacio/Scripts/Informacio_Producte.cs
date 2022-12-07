using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{
    //INTERN
    List<GameObject> productes;

    public override GameObject[] Mostrar(Peça peça)
    {
        if (productes == null)
            productes = new List<GameObject>();
        else productes.Clear();
        for (int i = 0; i < peça.Subestat.Estrategia.Numero; i++)
        {
            productes.Add(Instantiate(Prefab, peça.transform.position + DesplaçamentLateral(i), Quaternion.Euler(0, 0, Rotacio(i)), peça.transform).GetComponent<UI_InformacioPeça>().Setup(peça,0));
        }
        return productes.ToArray();
    }
}
