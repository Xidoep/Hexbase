using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byHabitantDisponible")]
public class Condicio_CasaDisponible : Condicio
{
    [Linia]
    [Header("CASA DISPONIBLE")]
    [SerializeField] Estat casa;

    [Linia]
    [Header("CAMI")]
    [SerializeField] bool utilitzarCami;
    [SerializeField] Estat cami;

    [Linia]
    [Header("GRUP")]
    [SerializeField] bool utilitzarGrup;
    [SerializeField] Grups grups;

    //INTERN
    Pe�a[] veins;
    public override bool Comprovar(Pe�a pe�a)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;


        veins = pe�a.VeinsPe�a;

        for (int v = 0; v < veins.Length; v++)
        {
            if (veins[v].EstatIgualA(casa)) 
            {
                if (!utilitzarGrup)
                {
                    if (Seleccionar(pe�a, veins[v].TreballadorLLiure)) return true;
                    /*Casa casa = veins[i].TreballadorLLiure;
                    if (casa != null)
                    {
                        Canviar(pe�a);
                        casa.Ocupar(pe�a);
                        return true;
                    }*/
                }
                else
                {
                    List<Pe�a> grup = grups.Peces(veins[v].Grup);
                    for (int g = 0; g < grup.Count; g++)
                    {
                        if (Seleccionar(pe�a, grup[g].TreballadorLLiure)) return true;
                    }
                }
            }
        }

        if (utilitzarCami)
        {
            Debug.LogError("UTILITZAR CAMI");
            for (int v = 0; v < veins.Length; v++)
            {
                if (veins[v].EstatIgualA(cami))
                {
                    List<Pe�a> connectatsACami = grups.Veins(veins[v].Grup);
                    Debug.LogError($"- {connectatsACami.Count} cami");

                    for (int c = 0; c < connectatsACami.Count; c++)
                    {
                        Debug.LogError($"-- vei del cami {connectatsACami[c].gameObject.name}");
                        if (connectatsACami[c].EstatIgualA(casa))
                        {
                            if (!utilitzarGrup)
                            {
                                if (Seleccionar(pe�a, connectatsACami[c].TreballadorLLiure)) return true;
                            }
                            else
                            {
                                List<Pe�a> grup = grups.Peces(connectatsACami[c].Grup);
                                for (int g = 0; g < grup.Count; g++)
                                {
                                    if (Seleccionar(pe�a, grup[g].TreballadorLLiure)) return true;
                                }
                            }
                        }
                    }
                }
            }
        }


        return false;
    }

    bool Seleccionar(Pe�a pe�a, Casa casa)
    {
        if (casa != null)
        {
            Canviar(pe�a);
            casa.Ocupar(pe�a);
            return true;
        }
        else return true;
    }

    private void OnValidate()
    {
        if(grups == null) grups = (Grups)XS_Editor.LoadAssetAtPath<Grups>("Assets/XidoStudio/Hexbase/Processos/Grups/Grups.asset");
    }
}
