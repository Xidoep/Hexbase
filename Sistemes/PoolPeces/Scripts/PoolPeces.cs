using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Pool")]
public class PoolPeces : ScriptableObject
{
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Fase_Resoldre resoldre;

    [SerializeField] List<Estat> peces;
    [Linia]
    [SerializeField] Estat[] disponibles;
    [SerializeField] int inicial;

    bool iniciat = false;

    System.Action<Estat> enAfegir;
    System.Action enTreure;

    public int Quantitat => peces.Count;
    public Estat Peça(int index) => peces[index];
    public bool Iniciat => iniciat;
    public System.Action<Estat> EnAfegir { get => enAfegir; set => enAfegir = value; }
    public System.Action EnTreure { get => enTreure; set => enTreure = value; }



    public void Iniciar()
    {
        if (peces == null) peces = new List<Estat>();

        peces = new List<Estat>();

        AddPeces(inicial);

        iniciat = true;

        colocar.Seleccionar(peces[0]);

        colocar.OnFinish += RemovePeça;
        resoldre.Nivell.EnPujarNivell += AddPecesPerNivell;

    }

    //(nivell / 2) * 10
    void AddPeces(int peces)
    {
        for (int i = 0; i < peces; i++)
        {
            AddPeça();
        }
    }
    public void AddPecesPerNivell(int nivell, int experiencia)
    {
        for (int i = 0; i < (nivell / 2) * 10; i++)
        {
            AddPeça();
        }
    }

    Estat AddPeça()
    {
        //FALTA: Assegurar X cases?
        Estat seleccionat = disponibles[Random.Range(0, disponibles.Length)];
        peces.Add(seleccionat);
        enAfegir?.Invoke(seleccionat);
        return seleccionat;
    }

    public void RemovePeça()
    {
        peces.RemoveAt(0);

        if(peces.Count == 0)
            colocar.Seleccionar(peces[0]);

        enTreure?.Invoke();
    }



    private void OnDisable()
    {
        peces = new List<Estat>();
        iniciat = false;
        enAfegir = null;
        enTreure = null;
    }

    private void OnValidate()
    {
        disponibles = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
    }
}
