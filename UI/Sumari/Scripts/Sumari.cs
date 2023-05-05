using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Sumari")]
public class Sumari : ScriptableObject
{
    [SerializeField] Fase_Processar faseProcessar;
    [SerializeField] Grups grups;
    [SerializeField] Produccio produccio;

    //Aixo ha de ser una classe amb una opcio per resaltar la peça seleccionada.
    [SerializeField] List<Producte> necessitats;
    [SerializeField] List<Producte> produits;


    System.Action<List<Producte>> enMostrarNecessitats;
    System.Action<List<Producte>> enMostrarProduits;
    public System.Action<List<Producte>> EnMostrarNecessitats { get => enMostrarNecessitats; set => enMostrarNecessitats = value; }
    public System.Action<List<Producte>> EnMostrarProduits { get => enMostrarProduits; set => enMostrarProduits = value; }



    void OnEnable()
    {
        faseProcessar.OnFinish += Mostrar;
    }
    void OnDisable()
    {
        faseProcessar.OnFinish -= Mostrar;
    }



    public void Mostrar()
    {
        AgafarInformacio();
        enMostrarNecessitats?.Invoke(necessitats);
        enMostrarProduits?.Invoke(produits);
    }

    void AgafarInformacio()
    {
        necessitats = new List<Producte>();
        for (int g = 0; g < grups.Grup.Count; g++)
        {
            if (!grups.Grup[g].EsPoble)
                continue;

            for (int p = 0; p < grups.Grup[g].Peces.Count; p++)
            {
                for (int c = 0; c < grups.Grup[g].Peces[p].CasesLength; c++)
                {
                    for (int n = 0; n < grups.Grup[g].Peces[p].Cases[c].Necessitats.Count; n++)
                    {
                        necessitats.Add(grups.Grup[g].Peces[p].Cases[c].Necessitats[n]);
                    }
                }
            }
        }

        produits = new List<Producte>();
        for (int p = 0; p < produccio.Productors.Count; p++)
        {
            if (!produccio.Productors[p].Connectat)
                continue;

            for (int pe = 0; pe < produccio.Productors[p].Connexio.ProductesExtrets.Length; pe++)
            {
                if (produccio.Productors[p].Connexio.ProductesExtrets[pe].gastat)
                    continue;

                produits.Add(produccio.Productors[p].Connexio.ProductesExtrets[pe].producte);
            }
        }
    }

}
