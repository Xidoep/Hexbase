using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{
    //INTERN
    List<GameObject> productes;

    public override GameObject[] Mostrar(Pe�a pe�a)
    {
        if (productes == null)
            productes = new List<GameObject>();
        else productes.Clear();
        for (int i = 0; i < pe�a.Subestat.Estrategia.Numero; i++)
        {
            productes.Add(Instantiate(Prefab, pe�a.transform.position + Despla�amentLateral(i), Quaternion.Euler(0, 0, Rotacio(i)), pe�a.transform).GetComponent<UI_InformacioPe�a>().Setup(pe�a,0));
        }
        return productes.ToArray();
    }
}
