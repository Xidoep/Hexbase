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

    public override bool Comprovar(Peça peça, Grups grups, Estat cami, bool canviar, System.Action<Peça, bool> enConfirmar, System.Action<Peça, int> enCanviar)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;


        myVeins = GetVeinsAcordingToOptions(peça, grups, cami);

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (myVeins[i].SubestatIgualA(subestat) && myVeins[i].LLiure)
            {
                enConfirmar.Invoke(peça, canviar);
                if (canviar)
                {
                    myVeins[i].Ocupar(peça);
                    Canviar(peça, enCanviar);
                }

                return true;
            }
        }

        return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();

        cami = true;
        grup = false;
    }
}
