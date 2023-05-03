using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Sumari : MonoBehaviour
{
    [SerializeScriptableObject][SerializeField] Sumari sumari;
    [Space(20)]
    [SerializeField] UI_Producte prefab_UIProducte;
    [SerializeField] Transform parentNecessitats;
    [SerializeField] Transform parentProductes;
    [Space(20)]
    [SerializeField] List<GameObject> necessitats;
    [SerializeField] List<GameObject> productes;



    private void OnEnable()
    {
        sumari.EnMostrar += Mostrar;
        sumari.Mostrar();
    }
    private void OnDisable()
    {
        sumari.EnMostrar -= Mostrar;
    }



    void Mostrar(List<Producte> necessitats, List<Producte> productes)
    {
        BorrarAnteriors();
        CrearNous(necessitats, productes);
    }

    void BorrarAnteriors()
    {
        if (necessitats == null) necessitats = new List<GameObject>();
        if (productes == null) productes = new List<GameObject>();

        for (int i = necessitats.Count - 1; i >= 0; i--)
        {
            Destroy(necessitats[i]);
        }
        for (int i = productes.Count - 1; i >= 0; i--)
        {
            Destroy(productes[i]);
        }

        necessitats.Clear();
        productes.Clear();
    }
    void CrearNous(List<Producte> necessitats, List<Producte> productes)
    {

        for (int i = 0; i < necessitats.Count; i++)
        {
            this.necessitats.Add(Instantiate(prefab_UIProducte, parentNecessitats).Setup(necessitats[i], false));
        }
        for (int i = 0; i < productes.Count; i++)
        {
            this.productes.Add(Instantiate(prefab_UIProducte, parentProductes).Setup(productes[i], false));
        }
    }
}
