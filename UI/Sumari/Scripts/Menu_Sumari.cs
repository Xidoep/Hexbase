using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Menu_Sumari : MonoBehaviour
{
    [SerializeScriptableObject][SerializeField] Sumari sumari;
    [Space(20)]
    [SerializeField] Menu_Sumari_Coleccio necessitats;
    [SerializeField] Menu_Sumari_Coleccio produits;


    private void OnEnable()
    {
        sumari.EnMostrarNecessitats += necessitats.Crear;
        sumari.EnMostrarProduits += produits.Crear;
        sumari.Mostrar();
    }
    private void OnDisable()
    {
        sumari.EnMostrarNecessitats -= necessitats.Crear;
        sumari.EnMostrarProduits -= produits.Crear;
    }

}
