using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat
{
    [Apartat("RECURSOS")]
    [SerializeField] Producte producte;
    [SerializeField] EstrategiaDeProduccio estrategia;

    [Apartat("INFROMACIO")]
    [SerializeField] GameObject uiProducte_prefab;

    public Producte Producte => producte;

    List<GameObject> productes;

    public override Producte[] Produccio() => estrategia.Produir(producte);


    public override GameObject[] MostrarInformacio(Pe�a pe�a)
    {
        productes = new List<GameObject>();
        //for (int i = 0; i < estrategia.Numero; i++)
        //{
            productes.Add(Instantiate(uiProducte_prefab, pe�a.transform.position, Quaternion.identity, pe�a.transform).GetComponent<UI_Producte>().Setup(this));
        //}
        return productes.ToArray();
    }


    new void OnValidate()
    {
        base.OnValidate();
    }
}
