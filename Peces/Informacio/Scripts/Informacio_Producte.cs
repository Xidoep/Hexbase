using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{
    [SerializeField] UI_InformacioPe�a prefab;

    int quantitat = 0;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false)
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();
        */

        Debug.LogError("Extreure producte", hexagon);

        if (!((Pe�a)hexagon).Ocupat)
            return;

        Amagar(hexagon);

        quantitat = ((Pe�a)hexagon).ExtreureProducte.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && ((Pe�a)hexagon).ExtreureProducte[i].gastat)
                continue;

            GameObject tmp = Instantiate(prefab.gameObject, hexagon.transform.position, Quaternion.identity, hexagon.transform);
            tmp.GetComponent<UI_InformacioPe�a>().Setup(((Pe�a)hexagon), i);
            tmp.transform.GetChild(0).transform.localPosition = Despla�amentLateral(tmp.transform, quantitat, i);
            ((Pe�a)hexagon).ExtreureProducte[i].informacio = new Unitat(tmp);
            //ui.Add(pe�a.productesExtrets[i].informacio);
        }
        //return ui.ToArray();
    }

    public override void Amagar(Hexagon hexagon)
    {
        for (int i = 0; i < ((Pe�a)hexagon).ExtreureProducte.Length; i++)
        {
            if (((Pe�a)hexagon).ExtreureProducte[i].informacio.gameObject == null)
                continue;

            Destroy(((Pe�a)hexagon).ExtreureProducte[i].informacio.gameObject, 0.1f);
        }
    }
}
