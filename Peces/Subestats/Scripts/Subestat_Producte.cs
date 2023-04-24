using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat
{
    //[Apartat("SUBESTAT PRODUCTE")]
    public override Subestat Setup(Pe�a pe�a)
    {
        base.Setup(pe�a);

        Producte[] productes = Estrategia.Produir(Producte);
        List<Pe�a.ProducteExtret> productesExtrets = new List<Pe�a.ProducteExtret>();
        for (int i = 0; i < productes.Length; i++)
        {
            productesExtrets.Add(new Pe�a.ProducteExtret()
            {
                producte = productes[i],
                gastat = false
            });
        }
        pe�a.SetProductesExtrets = productesExtrets.ToArray();

        pe�a.processador.AfegirRecepta(produccio, ExtreureProductes);


        return this;
    }

    [SerializeField] Recepta produccio;

    void ExtreureProductes(object productes)
    {
        /*
         * en el moment en que es crea la pe�a sense un productor apuntantlo, no s'haurien d'haver extret els productes...
         * Es en el moment en que es crea un productor enganxat, en que... el productor hauria de fer cumplir aquesta recepte
         * i extreure els productes aqui, i despres. borrar aquesta recepta de la pe�a. Si es vol!
         * Es pot NO borrar la recepte, aixi sempre pots extreure recursos d'una pe�a si ho vols aix�.
         * O inclos, pots voler canviar la recepta, per una altre. Com per exemple en un camp, que si despres hi poses un mol� al costat, produeix mes.
         * I en el moment de demanar informacio... clar, el retorn ser� de una quantitat de productes...
         * El subestat crida la funcio de canviar d'estat quan es compleix la recepta
         * La casa crida la funcio per donar punts o fer el que sigui...
         * En aquest cas, hauria de fer que extragu�s els productes i els guard�s a la pe�a, com es fa ara al Setup.
         * El problema d'aix� �s tenir en compte que ha d'extreure els productes abans d'arribar a la fase de produccio,
         * on els productors agafen el producte de les seves extraccions.
         * Perfecte, aix� ser� el que fer� dem�.
         */
    }

    public override bool EsProducte => true;

}
