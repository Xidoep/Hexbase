using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byProducte")]
public class Condicio_Producte : Condicio
{
    [Apartat("CONDICIO PRODUCTE")]
    [SerializeField] Subestat subestat;

    //INTERN
    List<Peça> myVeins;

    public override bool Comprovar(Peça peça, Proximitat proximitat, Grups grups, Estat cami, System.Action<Peça, int> enCanviar)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;


        myVeins = GetVeinsAcordingToOptions(peça, grups, cami);

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (myVeins[i].SubestatIgualA(subestat) && myVeins[i].LLiure)
            {
                myVeins[i].Ocupar(peça);
                Canviar(peça, enCanviar);
                return true;
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
