using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Prediccio")]
public class Prediccio : ScriptableObject
{
    [SerializeField] FasesControlador controlador;
    [SerializeField] Fase_Colocar colocar;
    [Space(10)]
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [Space(10)]
    [SerializeField] Subestat casa;
    [SerializeField] Estat cami;

    [Apartat("Debug")]
    [SerializeField] Peça simulada;
    [SerializeField] bool simulant = false;
    [SerializeField] List<Peça> simulacioComprovar;

    [SerializeField] List<Grup> grupsSimulats;

    Grid grid;


    private void OnEnable()
    {
        simulant = false;
    }

    public void Predir(Vector2Int coordenada)
    {
        if (!controlador.Es(colocar))
        {
            Debugar.LogError("INTENTAR PREDIR FORA DE LA FASE COLOCAR!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Debugar.LogError($"--------------PREDIR ({coordenada})---------------");
        if (grid == null) grid = FindObjectOfType<Grid>();


        if (!simulant)
        {
            Debugar.LogError("no simulant...");
            simulant = true;

            grupsSimulats = new List<Grup>();
            for (int i = 0; i < grups.Grup.Count; i++)
            {
                grupsSimulats.Add(new Grup(grups.Grup[i]));
            }


            simulada = grid.SimularInici(colocar.Seleccionada, coordenada);
            simulacioComprovar = new List<Peça>() { simulada };
            for (int c = 0; c < simulada.Condicions.Length; c++)
            {
                List<Peça> veinsAcordingToOptions = simulada.Condicions[c].GetVeinsAcordingToOptions(simulada, grups, cami);
                for (int v = 0; v < veinsAcordingToOptions.Count; v++)
                {
                    if (!simulacioComprovar.Contains(veinsAcordingToOptions[v])) simulacioComprovar.Add(veinsAcordingToOptions[v]);
                }
            }

            grups.Agrupdar(grupsSimulats, simulada, SimularProximitat);
        }
        else
        {
            Debugar.LogError("simulant...");

            //grups.RecuperaVersioAnterior();
            grid.SimularFinal(simulada);
            grups.Interrompre();
        }
    }
    void SimularProximitat() => proximitat.Process(simulacioComprovar, MostrarCanvis, false);

    void MostrarCanvis(List<Peça> comprovades, List<Peça> canviades)
    {
        for (int i = 0; i < canviades.Count; i++)
        {
            Debugar.LogError($"***Mostrar Canvis a {canviades[i].gameObject.name}***");
        }

        if(simulada.SubestatIgualA(casa))
        {
            for (int i = 0; i < simulacioComprovar.Count; i++)
            {
                //En el cas que: Jo vulgui colocar una casa, i la peça que coloco o una peça veina també sigui una casa i no canvii.
                if (simulacioComprovar[i].SubestatIgualA(casa) && !canviades.Contains(simulacioComprovar[i]))
                {
                    Debugar.LogError($"***Mostrar + Needs a {simulacioComprovar[i].gameObject.name}***");
                }
            }
        }
        else
        {
            for (int i = 0; i < canviades.Count; i++)
            {
                //En el cas que: Jo NO vulgui colocar una casa, i alguna de les peces canviades fos una casa.
                if (canviades[i].SubestatIgualA(casa))
                {
                    Debugar.LogError($"***Mostrar - Needs a {canviades[i].gameObject.name}***");
                }
            }

        }

        for (int i = 0; i < canviades.Count; i++)
        {
            if (canviades[i].Ocupat) canviades[i].Desocupar();
        }

        //grups.RecuperaVersioAnterior();
        grid.SimularFinal(simulada);
        simulant = false;
        Debugar.LogError("FINALITZAT! (PREDICCIONS)");
    }

    public void AmagarInformacioMostrada(Vector2Int coordenada)
    {
        Debugar.LogError("***Si hi ha informacio mostrada, s'esborra***");
        //simulant = false;
    }
    public void FinalitzacioForçada()
    {
        //grid.SimularFinal(simulada);
        simulant = false;
    }
}
