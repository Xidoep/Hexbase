using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byOcupat")]
public class Condicio_Ocupat : Condicio
{

    List<Pe�a> myVeins;

    bool extractorAssignat;

    public override bool Comprovar(Pe�a pe�a, Grups grups, Estat cami, bool canviar, System.Action<Pe�a, bool> enConfirmar, System.Action<Pe�a, int> enCanviar)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        myVeins = GetVeinsAcordingToOptions(pe�a, grups, cami);

        /*extractorAssignat = false;
        for (int i = 0; i < myVeins.Count; i++)
        {
            if(myVeins[i].Extraccio == pe�a)
            {
                Debug.LogError($"=>CONDICIO OCUPAT. La pe�a ({myVeins[i].gameObject.name}) em te com extractor");

                enConfirmar.Invoke(pe�a, canviar);
                if (canviar)
                    Canviar(pe�a, enCanviar);

                extractorAssignat = true;
                break;
            }
        }
        return extractorAssignat;*/
        if (pe�a.Ocupat) 
        {
            Debug.LogError("=>CONDICIO OCUPAT. Per tant el camp queda ocupat...");

            enConfirmar.Invoke(pe�a, canviar);

            if (canviar)
                Canviar(pe�a, enCanviar);

            return true;
        }
        else return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();
    }
}
