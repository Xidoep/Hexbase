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
    [Header("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    [Linia]
    [Nota("Estats per desbloquejar el WFC")]
    [SerializeField] Estat[] desbloquejadores;

    [Linia]
    [SerializeField] Estat cami;

    float startTime;
    Peça peça;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        peça = (Peça)arg;

        Debug.LogError(peça);



        startTime = Time.realtimeSinceStartup;
        WFC();

    }


    void WFC()
    {
        WaveFunctionColapse.UltimaPeçaCreada = peça;
        peça.Actualitzar();

        WaveFunctionColapse.Process(peça, Agrupar, CrearPeçaDesbloquejadora);
    }

    void Agrupar()
    {
        grups.Agrupdar(peça, Proximitat);
    }

    
    void Proximitat(int index)
    {

        //if (proximitats == null) proximitats = new List<Peça>();
        //else proximitats.Clear();

        List<Peça> proximitats = new List<Peça>();
        proximitats.Add(peça);

        List<Peça> grup = grups.Peces(index);
        for (int i = 0; i < grup.Count; i++)
        {
            if(!proximitats.Contains(grup[i])) proximitats.Add(grup[i]);
        }

        Peça[] veins = peça.VeinsPeça;
        for (int i = 0; i < veins.Length; i++)
        {
            if (!proximitats.Contains(veins[i])) proximitats.Add(veins[i]);
        }

        for (int i = 0; i < veins.Length; i++)
        {
            if (veins[i].EstatIgualA(cami))
            {
                List<Peça> _cami = grups.Peces(veins[i].Grup);
                for (int c = 0; c < _cami.Count; c++)
                {
                    if (!proximitats.Contains(_cami[c])) proximitats.Add(_cami[c]);
                }
                List<Peça> veinsCami = grups.Veins(veins[i].Grup);
                for (int vc = 0; vc < veinsCami.Count; vc++)
                {
                    if (!proximitats.Contains(veinsCami[vc])) proximitats.Add(veinsCami[vc]);
                }
            }
        }
        //List<Peça> cami = grups.Peces(index);

       

        //Es repoble abans pels camps, que a proximitat busquen una casa disponible.
        repoblar.Proces(proximitats);

        proximitat.Process(proximitats, Produir);
    }

    void Produir(List<Peça> peces)
    {
        //El segon repoblar és perque les cases es creen segons les peces que tenen al voltant,
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




    void CrearPeçaDesbloquejadora(Vector2Int coordenada)
    {
        grid.CrearPeça(desbloquejadores[Random.Range(0,desbloquejadores.Length)], coordenada, true);
    }


    public override void Finalitzar()
    {
        onFinish?.Invoke();
    }
}
