using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byProducte")]
public class Condicio_Producte : Condicio
{
    [Apartat("CONDICIO PRODUCTE")]
    [SerializeField] Subestat subestat;

    //INTERN
    List<Peça> veins;
    List<Peça> veinsCami;

    public override bool Comprovar(Peça peça)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        

        if (cami)
        {
            veinsCami = VeinsAmbCami(peça);

            for (int i = 0; i < veinsCami.Count; i++)
            {
                if (veinsCami[i].SubestatIgualA(subestat) && veinsCami[i].LLiure)
                {
                    veinsCami[i].Ocupar(peça);
                    Canviar(peça);
                    return true;
                }
            }
        }
        else
        {
            veins = Veins(peça);

            for (int i = 0; i < veins.Count; i++)
            {
                if (veins[i].SubestatIgualA(subestat) && veins[i].LLiure)
                {
                    veins[i].Ocupar(peça);
                    Canviar(peça);
                    return true;
                }
            }
        }

        
        return false;
    }

    protected override void Setup()
    {
        base.Setup();

        cami = true;
        grup = false;
    }
}
