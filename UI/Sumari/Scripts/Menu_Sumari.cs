using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Sumari : MonoBehaviour
{
    [SerializeScriptableObject][SerializeField] Sumari sumari;
    [Space(20)]
    [SerializeField] Coleccio necessitats;
    [SerializeField] Coleccio produits;

    [System.Serializable]
    public class Coleccio
    {
        [SerializeField] HorizontalLayoutGroup parent;
        [SerializeField] List<XS_Button> llista;
        [Space(20)]
        [SerializeField] UI_Producte prefab;

        void Borrar()
        {
            for (int i = llista.Count - 1; i >= 0; i--)
            {
                llista[i].Destroy();
            }
            llista.Clear();
        }

        public void Crear(List<Producte> productes)
        {
            if (llista == null) llista = new List<XS_Button>();
            Borrar();

            parent.spacing = -(Mathf.LerpUnclamped(10, 40, productes.Count / 15));
            for (int i = 0; i < productes.Count; i++)
            {
                llista.Add(Instantiate(prefab, parent.transform).Setup(productes[i], false).GetComponent<XS_Button>());
            }
        }
    }

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
