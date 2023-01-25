using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Condicional")]
public class TileSet_Condicional : TileSetBase
{
    [SerializeField] Condicio[] condicions;

    Condicio coincidida;

    public override TilesPossibles[] Tiles(Pe�a pe�a) => Coincidencia(pe�a).TileSet.Tiles;
    public override Connexio[] ConnexionsNules(Pe�a pe�a) => Coincidencia(pe�a).TileSet.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Pe�a pe�a) => Coincidencia(pe�a).TileSet.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Pe�a pe�a = null) => Coincidencia(pe�a).TileSet.ConnexionsPossibles;



    Condicio Coincidencia(Pe�a pe�a)
    {
        for (int i = 0; i < condicions.Length; i++)
        {
            if (condicions[i].Comprovar(pe�a))
            {
                coincidida = condicions[i];
                //Debug.LogError($"Seleccionar condicio {i}", pe�a);
                break;
            }
        }
        return coincidida;
    }

    [System.Serializable]
    public class Condicio
    {
        [SerializeField] string id;
        [SerializeField] TileSet tileSet;
        [Linia]
        [SerializeField] List<Subestat> subestats;
        [SerializeField] int quantitat;
        [SerializeField] bool igual;
        [SerializeField] bool mes;
        [SerializeField] bool menys;

        int contats;
        List<Pe�a> veins;

        public TileSet TileSet => tileSet;

        public bool Comprovar(Pe�a pe�a)
        {
            veins = pe�a.VeinsPe�a;
            contats = 0;
            for (int i = 0; i < veins.Count; i++)
            {
                if (subestats.Contains(veins[i].Subestat)) contats++;
            }
            if (igual && !mes && !menys)
            {
                if (contats == quantitat) return true;
            }
            else if (mes)
            {
                if (igual)
                    if (contats >= quantitat) return true;
                    else if (contats > quantitat) return true;
            }
            else if (menys)
            {
                if (igual)
                    if (contats <= quantitat) return true;
                    else if (contats < quantitat) return true;
            }
            else if (igual && mes && menys) return true;

            return false;
        }

        public void Setup() => tileSet.Setup();
    }

    protected void OnValidate()
    {
        for (int i = 0; i < condicions.Length; i++)
        {
            condicions[i].Setup();
        }
        List<Connexio> tmpConnexions = new List<Connexio>();
    }


}
