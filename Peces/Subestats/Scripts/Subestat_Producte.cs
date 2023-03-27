using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat
{
    //[Apartat("SUBESTAT PRODUCTE")]
    public override Subestat Setup(Pe�a pe�a)
    {
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

        return base.Setup(pe�a);
    }

    public override bool EsProducte => true;
    /*
        [SerializeField] GameObject uiProducte_prefab;



        List<GameObject> productes;




        public override GameObject[] MostrarInformacio(Pe�a pe�a)
        {
            productes = new List<GameObject>();
            //for (int i = 0; i < estrategia.Numero; i++)
            //{
            //    productes.Add(Instantiate(uiProducte_prefab, pe�a.transform.position + Despla�amentLateral(i), Quaternion.Euler(0, 0, Rotacio(i)), pe�a.transform).GetComponent<UI_Producte>().Setup(pe�a,0));
            //}
            return productes.ToArray();
        }



        Vector3 Despla�amentLateral(int i) => ((Camera.main.transform.right * (((i + 1) / 2) * 0.5f)) * (i % 2 == 0 ? 1 : -1)) + (Vector3.down * (i * i * 0.025f));
        float Rotacio(int i) => (((i + 1) / 2) * 15f) * (i % 2 == 0 ? -1 : 1);

        new void OnValidate()
        {
            base.OnValidate();
        }
    */
}
