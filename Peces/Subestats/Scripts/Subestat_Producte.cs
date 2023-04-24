using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat
{
    //[Apartat("SUBESTAT PRODUCTE")]
    public override Subestat Setup(Peça peça)
    {
        base.Setup(peça);

        Producte[] productes = Estrategia.Produir(Producte);
        List<Peça.ProducteExtret> productesExtrets = new List<Peça.ProducteExtret>();
        for (int i = 0; i < productes.Length; i++)
        {
            productesExtrets.Add(new Peça.ProducteExtret()
            {
                producte = productes[i],
                gastat = false
            });
        }
        peça.SetProductesExtrets = productesExtrets.ToArray();

        peça.processador.AfegirRecepta(produccio, ExtreureProductes);


        return this;
    }

    [SerializeField] Recepta produccio;

    void ExtreureProductes(object productes)
    {
        /*
         * en el moment en que es crea la peça sense un productor apuntantlo, no s'haurien d'haver extret els productes...
         * Es en el moment en que es crea un productor enganxat, en que... el productor hauria de fer cumplir aquesta recepte
         * i extreure els productes aqui, i despres. borrar aquesta recepta de la peça. Si es vol!
         * Es pot NO borrar la recepte, aixi sempre pots extreure recursos d'una peça si ho vols així.
         * O inclos, pots voler canviar la recepta, per una altre. Com per exemple en un camp, que si despres hi poses un molí al costat, produeix mes.
         * I en el moment de demanar informacio... clar, el retorn serà de una quantitat de productes...
         * El subestat crida la funcio de canviar d'estat quan es compleix la recepta
         * La casa crida la funcio per donar punts o fer el que sigui...
         * En aquest cas, hauria de fer que extragués els productes i els guardés a la peça, com es fa ara al Setup.
         * El problema d'això és tenir en compte que ha d'extreure els productes abans d'arribar a la fase de produccio,
         * on els productors agafen el producte de les seves extraccions.
         * Perfecte, això serà el que feré demà.
         */
    }

    public override bool EsProducte => true;

}
