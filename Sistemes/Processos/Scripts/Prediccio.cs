using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Prediccio")]
public class Prediccio : ScriptableObject
{
    [SerializeField] Fase_Colocar colocar;
    [Space(10)]
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [Space(10)]
    [SerializeField] Subestat casa;

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

            //grupsSimulats = new List<Grup>(grups.Grup);
            //grups.Grup.CopyTo(grupsSimulats.ToArray());

            simulada = grid.SimularInici(colocar.Seleccionada, coordenada);
            //Debugar.LogError($"COMPROVAR POTENCIAL RANURA {simulada.Coordenades} que te {grid.VeinsPeça(coordenada).Count} veines------------");
            simulacioComprovar = new List<Peça>() { simulada };
            simulacioComprovar.AddRange(grid.VeinsPeça(coordenada));

            grups.Agrupdar(grupsSimulats, simulada, SimularProximitat);
        }
        else
        {
            Debugar.LogError("simulant...");

            //grups.RecuperaVersioAnterior();
            grid.SimularFinal(simulada.Coordenades);
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


        //grups.RecuperaVersioAnterior();
        grid.SimularFinal(simulada.Coordenades);
        simulant = false;
        Debugar.LogError("FINALITZAT! (PREDICCIONS)");
    }

    public void AmagarInformacioMostrada(Vector2Int coordenada)
    {
        Debugar.LogError("***Si hi ha informacio mostrada, s'esborra***");
        //simulant = false;
    }
}
