using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byProducte")]
public class Condicio_Producte : Condicio
{
    [Apartat("CONDICIO PRODUCTE")]
    [SerializeField] Subestat subestat;

    //INTERN
    List<Pe�a> myVeins;

    public override bool Comprovar(Pe�a pe�a, Grups grups, Estat cami, bool canviar, System.Action<Pe�a, bool> enConfirmar, System.Action<Pe�a, int> enCanviar)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;


        myVeins = GetVeinsAcordingToOptions(pe�a, grups, cami);

        for (int i = 0; i < myVeins.Count; i++)
        {
            if (myVeins[i].SubestatIgualA(subestat) && myVeins[i].LLiure)
            {
                enConfirmar.Invoke(pe�a, canviar);
                if (canviar)
                {
                    myVeins[i].Ocupar(pe�a);
                    Canviar(pe�a, enCanviar);
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
