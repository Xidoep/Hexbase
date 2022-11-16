using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Produccio")]
public class Produccio : ScriptableObject
{
    [SerializeField] Grups grups;
    //[SerializeField] PoolPeces pool;

    [Nota("Ara es mostra nom�s per debugar", NoteType.Warning)]
    [SerializeField] List<Pe�a> productors;
    
    
    [Apartat("ESTATS NECESSARIS")]
    [SerializeField] Estat cami;
    [SerializeField] Subestat casa;
    //PROPIETATS
    bool Finalitzat => index == productors.Count;

    //INTERN
    Action enFinalitzar;
    int index;
    List<Pe�a> veins;
    List<string> connexions;
    float stepTime = 0.1f;
    void OnEnable()
    {
        Resetejar();
    }
    public void AddProductor(Pe�a pe�a)
    {
        if (!productors.Contains(pe�a)) productors.Add(pe�a);
    }

    public void Resetejar()
    {
        productors = new List<Pe�a>();
    }


    public void Process(System.Action enFinalitzar)
    {
        Debugar.LogError("--------------PRODUCCIO---------------");
        index = 0;
        this.enFinalitzar = enFinalitzar;
        CleanAllNeeds();
        stepTime = 0.1f;
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
        XS_Coroutine.StartCoroutine_Ending(stepTime, Step);
    }

    private void CleanAllNeeds()
    {
        for (int po = 0; po < grups.Grup.Count; po++)
        {
            if (!grups.Grup[po].Peces[0].SubestatIgualA(casa))
                continue;

            for (int pe = 0; pe < grups.Grup[po].Peces.Count; pe++)
            {
                for (int c = 0; c < grups.Grup[po].Peces[pe].CasesCount; c++)
                {
                    grups.Grup[po].Peces[pe].Cases[c].Clean();
                }
            }
            
        }
    }

    void RepartimentEquitatiu(Pe�a productor)
    {
        //*********************************************************************************************
        //Canviar el sistema de produccio.
        //*********************************************************************************************

        //List<Pe�a> poble = grups.Peces(productor);
        bool proveit = false;

        Producte[] recursos = productor.ExtreureProducte();
        Debug.LogError($"Donar {recursos.Length} Recursos");

        for (int r = 0; r < recursos.Length; r++)
        {
            //proveit = false;
            //proveit = IntentarProveir(poble,proveit,recursos[r]);
            connexions = grups.GrupByPe�a(productor).connexionsId;
            for (int c = 0; c < connexions.Count; c++)
            {
                //veins = grups.Grup[connexions[c]].Peces;
                proveit = IntentarProveir(grups.GrupById(connexions[c]).Peces, proveit, recursos[r]);

                if (proveit)
                    break;
            }

            if (!proveit)
            {
                //OLD
                /*veins = grups.Veins(poble[0]);
                for (int v = 0; v < veins.Count; v++)
                {
                    if (veins[v].EstatIgualA(cami))
                    {
                        List<Pe�a> camiVeins = grups.Veins(veins[v]);
                        for (int c = 0; c < camiVeins.Count; c++)
                        {
                            if (poble.Contains(camiVeins[c]) || !camiVeins[c].EstatIgualA(poble[0].Estat))
                                continue;

                            List<Pe�a> altrePoble = grups.Peces(camiVeins[c]);
                            proveit = IntentarProveir(altrePoble, proveit, recursos[r]);
                        }
                    }
                }*/

                //LESS NEW
                /*veins = grups.VeinsAmbCami(productor);
                for (int i = 0; i < veins.Count; i++)
                {
                    if (poble.Contains(veins[i]) || !veins[i].SubestatIgualA(casa))
                        continue;

                    proveit = IntentarProveir(grups.Peces(veins[i]), proveit, recursos[r]);
                }*/

                //NEW
                

                Debug.LogError("Sobra aquest recurs. haig de buscar un poble connectat per enviar els recursos.");
            }
        }
       
    }

    bool IntentarProveir(List<Pe�a> poble, bool proveit, Producte recurs)
    {
        proveit = false;
        for (int p = 0; p < poble.Count; p++)
        {
            for (int c = 0; c < poble[p].CasesCount; c++)
            {
                if (poble[p].Cases[c].Proveir(recurs, TempsDeVisualitzacio))
                {
                    proveit = true;
                    //pool.Add(1);
                    Debug.LogError($"Donat un recuros a la casa {c} de la pe�a {poble[p].gameObject.name}", poble[p].gameObject);
                    return proveit;
                }
            }
            if (proveit) //???
                break;
        }

        ResetStepTime();
        return proveit;
    }

    void TempsDeVisualitzacio() => stepTime = 3;
    void ResetStepTime() => stepTime = 0.1f;
}
