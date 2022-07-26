using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Procesar")]
public class Fase_Processar : Fase
{
    Grid grid;

    [Linia]
    [Header("PROCESSOS")]
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [SerializeField] Produccio produccio;
    [Linia]
    [Header("SEG�ENT FASE")]
    [SerializeField] Fase colocar;

    [Linia]
    [Nota("Estats per desbloquejar el WFC")]
    [SerializeField] Estat[] desbloquejadores;

    [Linia]
    [SerializeField] Estat cami;

    float startTime;
    Pe�a pe�a;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        pe�a = (Pe�a)arg;

        Debug.LogError(pe�a);



        startTime = Time.realtimeSinceStartup;
        WFC();

    }


    void WFC()
    {
        WaveFunctionColapse.UltimaPe�aCreada = pe�a;
        pe�a.Actualitzar();

        WaveFunctionColapse.Process(pe�a, Agrupar, CrearPe�aDesbloquejadora);
    }

    void Agrupar()
    {
        grups.Agrupdar(pe�a, Proximitat);
    }

    
    void Proximitat(int index)
    {

        //if (proximitats == null) proximitats = new List<Pe�a>();
        //else proximitats.Clear();

        List<Pe�a> proximitats = new List<Pe�a>();
        proximitats.Add(pe�a);

        List<Pe�a> grup = grups.Peces(index);
        for (int i = 0; i < grup.Count; i++)
        {
            if(!proximitats.Contains(grup[i])) proximitats.Add(grup[i]);
        }

        Pe�a[] veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Length; i++)
        {
            if (!proximitats.Contains(veins[i])) proximitats.Add(veins[i]);
        }

        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EstatIgualA(cami))
            {
                List<Pe�a> _cami = grups.Peces(veins[i].Grup);
                for (int c = 0; c < _cami.Count; c++)
                {
                    if (!proximitats.Contains(_cami[c])) proximitats.Add(_cami[c]);
                }
                List<Pe�a> veinsCami = grups.Veins(veins[i].Grup);
                for (int vc = 0; vc < veinsCami.Count; vc++)
                {
                    if (!proximitats.Contains(veinsCami[vc])) proximitats.Add(veinsCami[vc]);
                }
            }
        }
        //List<Pe�a> cami = grups.Peces(index);

       

        //Es repoble abans pels camps, que a proximitat busquen una casa disponible.
        repoblar.Proces(proximitats);

        proximitat.Process(proximitats, Produir);
    }

    void Produir(List<Pe�a> peces)
    {
        //El segon repoblar �s perque les cases es creen segons les peces que tenen al voltant,
        //per tant s'han de mirar despres que aquestes haguin "canviat".
        repoblar.Proces(peces);

        produccio.Process(FinalitzarProcessos);
    }

    void FinalitzarProcessos()
    {
        //repoblar.Proces(peces);
        Debug.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", this);
        colocar.Iniciar();
    }




    void CrearPe�aDesbloquejadora(Vector2Int coordenada)
    {
        grid.CrearPe�a(desbloquejadores[Random.Range(0,desbloquejadores.Length)], coordenada, true);
    }


    public override void Finalitzar()
    {
        onFinish?.Invoke();
    }
}
