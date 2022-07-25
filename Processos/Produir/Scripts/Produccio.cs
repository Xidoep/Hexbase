using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Produccio")]
public class Produccio : ScriptableObject
{
    [SerializeField] Grups grups;

    [SerializeField] List<Peça> productors;
    System.Action enFinalitzar;

    //INTERN
    int index;
    bool Finalitzat => index == productors.Count;


    void OnEnable()
    {
        productors = new List<Peça>();
    }
    public void AddProductor(Peça peça)
    {
        if (!productors.Contains(peça)) productors.Add(peça);
    }




    public void Process(System.Action enFinalitzar)
    {
        Debug.LogError("--------------PRODUCCIO---------------");
        index = 0;
        this.enFinalitzar = enFinalitzar;
        CleanAllNeeds();
        Step();
    }

    void Step()
    {
        if (productors.Count == 0 || Finalitzat)
        {
            enFinalitzar.Invoke();
            return;
        }

        

        RepartimentEquitatiu(productors[index]);
        //productors[index].Produir();

        index++;
        XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }

    private void CleanAllNeeds()
    {
        for (int po = 0; po < grups.Pobles.Count; po++)
        {
            for (int pe = 0; pe < grups.Pobles[po].Peces.Count; pe++)
            {
                for (int c = 0; c < grups.Pobles[po].Peces[pe].CasesCount; c++)
                {
                    grups.Pobles[po].Peces[pe].Cases[c].Clean();
                }
            }
            
        }
    }

    void RepartimentEquitatiu(Peça productor)
    {
        List<Peça> poble = grups.Peces(productor.Treballador.Grup);

        bool proveit = false;
        
        Recurs[] recursos = productor.Subestat.Recursos(productor);
        Debug.LogError($"Donar {recursos.Length} Recursos a {poble.Count} peces");

        for (int r = 0; r < recursos.Length; r++)
        {
            proveit = false;
            for (int p = 0; p < poble.Count; p++)
            {
                for (int c = 0; c < poble[p].CasesCount; c++)
                {
                    if (poble[p].Cases[c].Proveir(recursos[r]))
                    {
                        proveit = true;
                        Debug.LogError($"Donat un recuros a la casa {c} de la peça {poble[p].gameObject.name}");
                        break;
                    }
                }
                if (proveit)
                    break;
            }

            if (!proveit)
            {
                //*****************************************************************************************************************************
                //Primer, haig de buscar tots els camins que arribin fins al poble/grup.
                //Per cada cami: Buscar totes les peces casa que veines del cami, que no formin part del poble/grup inicial.
                //Per cada poble/cami trobat al llarg del cami: Iniciar el mateix procediment que he fet amb el propi poble/grup inicial, pero amb aquests pobles/grups trobats.
                //*****************************************************************************************************************************

                Debug.LogError("Sobra aquest recurs. haig de buscar un poble connectat per enviar els recursos.");
            }
        }
       
    }
}
