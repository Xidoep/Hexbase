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

    [SerializeField] List<EstatColocable> peces;
    [Linia]
    //[SerializeField] Estat[] disponibles;
    [SerializeField] int inicial;

    bool iniciat = false;

    System.Action<EstatColocable> enAfegir;
    System.Action enTreure;

    int PecesPerNivell(int nivell) => (nivell / 2) * 10;

    public int Quantitat => peces.Count;
    public EstatColocable Peça(int index) => peces[index];
    public bool Iniciat => iniciat;
    public System.Action<EstatColocable> EnAfegir { get => enAfegir; set => enAfegir = value; }
    public System.Action EnTreure { get => enTreure; set => enTreure = value; }



    public void Iniciar()
    {
        if (peces == null) peces = new List<EstatColocable>();

        colocar.OnFinish += RemovePeça;
        nivell.EnPujarNivell += AddPecesPerNivell;
        enAfegir += save.AddToPila;
        enTreure += save.RemoveLastFromPila;
        
        peces = new List<EstatColocable>();

        AddPeces();

        iniciat = true;

        colocar.Seleccionar(peces[0]);
    }

    void AddPeces()
    {
        if (save.HiHaAlgunaPeça)
            Guardades();
        else Noves();


        void Guardades()
        {
            List<EstatColocable> estats = save.Pila;

            for (int i = 0; i < estats.Count; i++)
            {
                peces.Add(estats[i]);
            }
        }
        void Noves()
        {
            for (int i = 0; i < inicial; i++)
            {
                AddPeça();
            }
        }
    }
    public void AddPecesPerNivell(int nivell)
    {
        for (int i = 0; i < PecesPerNivell(nivell); i++)
        {
            AddPeça();
        }
    }

    void AddPeça()
    {
        //FALTA: Assegurar X cases?
        EstatColocable seleccionat = referencies.Colocables[Random.Range(0, referencies.Colocables.Length)];
        peces.Add(seleccionat);
        enAfegir?.Invoke(seleccionat);
    }
    

    public void RemovePeça()
    {
        peces.RemoveAt(0);

        if (peces.Count > 0)
            colocar.Seleccionar(peces[0]);

        enTreure?.Invoke();
    }

    public void Reset()
    {
        colocar.OnFinish -= RemovePeça;
        nivell.EnPujarNivell -= AddPecesPerNivell;
        enAfegir -= save.AddToPila;
        enTreure -= save.RemoveLastFromPila;
    }

    private void OnDisable()
    {
        peces = new List<EstatColocable>();
        iniciat = false;
        enAfegir = null;
        enTreure = null;
    }

}
