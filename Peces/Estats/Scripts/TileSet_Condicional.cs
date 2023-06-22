using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/TileSet/Condicional")]
public class TileSet_Condicional : TileSetBase
{
    public override void Setup()
    {
        condicions = new Condicio[0];
    }
    [SerializeField, HorizontalGroup] Condicio[] condicions;

    Condicio coincidida;

    public override TilesPossibles[] Tiles(Pe�a pe�a) => Coincidencia(pe�a).TileSet.Tiles;
    public override Connexio[] ConnexionsNules(Pe�a pe�a) => Coincidencia(pe�a).TileSet.ConnexionsNules;
    public override ConnexioEspesifica[] ConnexionsEspesifiques(Pe�a pe�a) => Coincidencia(pe�a).TileSet.ConnexionsEspesifiques;
    public override Connexio[] ConnexioinsPossibles(Pe�a pe�a = null) => Coincidencia(pe�a).TileSet.ConnexionsPossibles;

    [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)] public Condicio[] Condicions { get => condicions; set => condicions = value; }

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
        public Condicio Setup()
        {
            tileSet = new TileSet().Setup();
            return this;
        }

        [SerializeField, HideInInspector]string id;
        [BoxGroup("$id", centerLabel: true), HorizontalGroup("$id/Split"), SerializeField] 
        List<Estat> subestats;
        [BoxGroup("$id", centerLabel: true), HorizontalGroup("$id/Split", PaddingLeft = 20), SerializeField, LabelText("$Simbols"), LabelWidth(30)] 
        int quantitat;


        [SerializeField] 
        TileSet tileSet;

        [HideInEditorMode] public bool mes;
        [HideInEditorMode] public bool igual;
        [HideInEditorMode] public bool menys;

        string Simbols => $"{(mes ? ">" : "")}{(menys ? "<" : "")}{(igual ? "=" : "")}";

        int contats;
        List<Pe�a> veins;

        public TileSet TileSet => tileSet;

        public void SetCondicions(string id, bool mes, bool igual, bool menys, int quantitat)
        {
            this.id = id;
            if (subestats == null) subestats = new List<Estat>();
            this.mes = mes;
            this.igual = igual;
            this.menys = menys;
            this.quantitat = quantitat;
        }
        public void AddEstat(Estat estat) 
        {
            if (subestats == null) subestats = new List<Estat>();
            subestats.Add(estat);


        } 

        public bool Comprovar(Pe�a pe�a)
        {
            veins = pe�a.VeinsPe�a;
            contats = 0;
            for (int i = 0; i < veins.Count; i++)
            {
                if (subestats.Contains(veins[i].Subestat)) contats++;
            }

            if ((igual && !mes && !menys) && contats == quantitat) return true;
            else if((!igual && mes && !menys) && contats > quantitat) return true;
            else if((!igual && !mes && menys) && contats < quantitat) return true;
            else if((igual && mes && !menys) && contats >= quantitat) return true;
            else if((igual && !mes && menys) && contats <= quantitat) return true;
            else if ((igual && mes && menys)) return true;
            else return false;
            /*if (igual && !mes && !menys)
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

            return false;*/
        }
    }

    protected void OnValidate()
    {
        for (int i = 0; i < condicions.Length; i++)
        {
            condicions[i].TileSet.SetConnexionsPossibles();
            condicions[i].TileSet.SetOrdenar();
        }
    }


}
