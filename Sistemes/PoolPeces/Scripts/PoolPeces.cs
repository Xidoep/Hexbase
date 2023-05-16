using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Pool")]
public class PoolPeces : ScriptableObject
{


    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Nivell nivell;
    [SerializeField] SaveHex save;
    [SerializeField] Referencies referencies;

    [SerializeField] List<Estat> peces;
    [Linia]
    //[SerializeField] Estat[] disponibles;
    [SerializeField] int inicial;

    bool iniciat = false;

    System.Action<Estat> enAfegir;
    System.Action enTreure;

    int PecesPerNivell(int nivell) => (nivell / 2) * 10;

    public int Quantitat => peces.Count;
    public Estat Pe�a(int index) => peces[index];
    public bool Iniciat => iniciat;
    public System.Action<Estat> EnAfegir { get => enAfegir; set => enAfegir = value; }
    public System.Action EnTreure { get => enTreure; set => enTreure = value; }



    public void Iniciar()
    {
        if (peces == null) peces = new List<Estat>();

        colocar.OnFinish += RemovePe�a;
        nivell.EnPujarNivell += AddPecesPerNivell;
        enAfegir += save.AddToPila;
        enTreure += save.RemoveLastFromPila;
        
        peces = new List<Estat>();

        AddPeces();

        iniciat = true;

        colocar.Seleccionar(peces[0]);
    }

    void AddPeces()
    {
        if (save.HiHaAlgunaPe�a)
            Guardades();
        else Noves();


        void Guardades()
        {
            List<Estat> estats = save.Pila;

            for (int i = 0; i < estats.Count; i++)
            {
                peces.Add(estats[i]);
            }
        }
        void Noves()
        {
            for (int i = 0; i < inicial; i++)
            {
                AddPe�a();
            }
        }
    }
    public void AddPecesPerNivell(int nivell)
    {
        for (int i = 0; i < PecesPerNivell(nivell); i++)
        {
            AddPe�a();
        }
    }

    void AddPe�a()
    {
        //FALTA: Assegurar X cases?
        Estat seleccionat = referencies.Estats[Random.Range(0, referencies.Estats.Length)];
        peces.Add(seleccionat);
        enAfegir?.Invoke(seleccionat);
    }
    

    public void RemovePe�a()
    {
        peces.RemoveAt(0);

        if (peces.Count > 0)
            colocar.Seleccionar(peces[0]);

        enTreure?.Invoke();
    }

    public void Reset()
    {
        colocar.OnFinish -= RemovePe�a;
        nivell.EnPujarNivell -= AddPecesPerNivell;
        enAfegir -= save.AddToPila;
        enTreure -= save.RemoveLastFromPila;
    }

    private void OnDisable()
    {
        peces = new List<Estat>();
        iniciat = false;
        enAfegir = null;
        enTreure = null;
    }

}
