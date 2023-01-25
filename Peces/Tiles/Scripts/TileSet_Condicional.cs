using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Condicional")]
public class TileSet_Condicional : TileSetBase
{
    [SerializeField] Condicio[] condicions;

    Condicio coincidida;

    public override TilesPossibles[] Tiles(Peça peça) => Coincidencia(peça).TileSet.Tiles;
    public override Connexio[] ConnexionsNules(Peça peça) => Coincidencia(peça).TileSet.ConnexionsNules;
    public override ConnexioEspesifica ConnexionsEspesifica(Peça peça) => Coincidencia(peça).TileSet.ConnexioEspesifica;
    public override Connexio[] ConnexioinsPossibles(Peça peça = null) => Coincidencia(peça).TileSet.ConnexionsPossibles;



    Condicio Coincidencia(Peça peça)
    {
        for (int i = 0; i < condicions.Length; i++)
        {
            if (condicions[i].Comprovar(peça))
            {
                coincidida = condicions[i];
                //Debug.LogError($"Seleccionar condicio {i}", peça);
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
        List<Peça> veins;

        public TileSet TileSet => tileSet;

        public bool Comprovar(Peça peça)
        {
            veins = peça.VeinsPeça;
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
