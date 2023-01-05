using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byOcupat")]
public class Condicio_Ocupat : Condicio
{

    List<Peça> myVeins;

    bool extractorAssignat;

    public override bool Comprovar(Peça peça, Grups grups, Estat cami, bool canviar, System.Action<Peça, bool> enConfirmar, System.Action<Peça, int> enCanviar)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        myVeins = GetVeinsAcordingToOptions(peça, grups, cami);

        /*extractorAssignat = false;
        for (int i = 0; i < myVeins.Count; i++)
        {
            if(myVeins[i].Extraccio == peça)
            {
                Debug.LogError($"=>CONDICIO OCUPAT. La peça ({myVeins[i].gameObject.name}) em te com extractor");

                enConfirmar.Invoke(peça, canviar);
                if (canviar)
                    Canviar(peça, enCanviar);

                extractorAssignat = true;
                break;
            }
        }
        return extractorAssignat;*/
        if (peça.Ocupat) 
        {
            Debug.LogError("=>CONDICIO OCUPAT. Per tant el camp queda ocupat...");

            enConfirmar.Invoke(peça, canviar);

            if (canviar)
                Canviar(peça, enCanviar);

            return true;
        }
        else return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();
    }
}
