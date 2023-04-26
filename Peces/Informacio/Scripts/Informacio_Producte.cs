using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Producte")]
public class Informacio_Producte : Informacio
{
    [SerializeField] UI_InformacioPeça prefab;

    int quantitat = 0;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false)
    {
        /*if (ui == null)
            ui = new List<Unitat>();
        else ui.Clear();
        */

        Debug.LogError("Extreure producte", hexagon);

        if (!((Peça)hexagon).Connectat)
            return;

        Amagar(hexagon);

        quantitat = ((Peça)hexagon).ProductesExtrets.Length;
        for (int i = 0; i < quantitat; i++)
        {
            if (!mostrarProveides && ((Peça)hexagon).ProductesExtrets[i].gastat)
                continue;

            GameObject tmp = Instantiate(prefab.gameObject, hexagon.transform.position, Quaternion.identity, hexagon.transform);
            tmp.GetComponent<UI_InformacioPeça>().Setup(((Peça)hexagon), i);
            tmp.transform.GetChild(0).transform.localPosition = DesplaçamentLateral(tmp.transform, quantitat, i);
            ((Peça)hexagon).ProductesExtrets[i].informacio = new Unitat(tmp);
            //ui.Add(peça.productesExtrets[i].informacio);
        }
        //return ui.ToArray();
    }

    public override void Amagar(Hexagon hexagon)
    {
        for (int i = 0; i < ((Peça)hexagon).ProductesExtrets.Length; i++)
        {
            if (((Peça)hexagon).ProductesExtrets[i].informacio.gameObject == null)
                continue;

            Destroy(((Peça)hexagon).ProductesExtrets[i].informacio.gameObject, 0.1f);
        }
    }
}
