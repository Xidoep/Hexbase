using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Prediccio")]
public class Prediccio : ScriptableObject
{
    [SerializeField] FasesControlador controlador;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Visualitzacions visualitzacions;
    [Space(10)]
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [Space(10)]
    [SerializeField] Subestat casa;
    [SerializeField] Estat cami;

    [Apartat("Debug")]
    [SerializeField] Peça peçaSimulada;
    [SerializeField] bool predint = false;
    [SerializeField] List<Peça> pecesPerComprovar;

    [SerializeField] List<Grup> grupsSimulats;

    bool amagarInformacioBuffer = false;

    private void OnEnable()
    {
        predint = false;
    }

    public void Predir(Vector2Int coordenada)
    {
        if (!controlador.Es(colocar))
        {
            Debugar.LogError("INTENTAR PREDIR FORA DE LA FASE COLOCAR!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Debugar.LogError($"--------------PREDIR ({coordenada})---------------");

        if (!predint)
        {
            predint = true;

            CrearCopiaDeGrups();

            CrearPeçaSimulada(coordenada);

            BuscarPecesPerComprovar();

            grups.Agrupdar(grupsSimulats, peçaSimulada, SimularProximitat);
        }
        else
        {
            Debugar.LogError("simulant...");

            //grups.RecuperaVersioAnterior();
            Grid.Instance.SimularFinal(peçaSimulada);
            grups.Interrompre();
        }
    }

    void CrearCopiaDeGrups()
    {
        grupsSimulats = new List<Grup>();
        for (int i = 0; i < grups.Grup.Count; i++)
        {
            grupsSimulats.Add(new Grup(grups.Grup[i]));
        }
    }
    void CrearPeçaSimulada(Vector2Int coordenada)
    {
        peçaSimulada = Grid.Instance.SimularInici(colocar.Seleccionada, coordenada);
    }
    void BuscarPecesPerComprovar()
    {
        pecesPerComprovar = new List<Peça>() { peçaSimulada };

        List<Peça> veinsDeLaSimulada = peçaSimulada.VeinsPeça;
        pecesPerComprovar.AddRange(veinsDeLaSimulada);

        for (int i = 0; i < veinsDeLaSimulada.Count; i++)
        {
            List<Peça> veinsDelGrupDelVei = grups.Veins(grups.Grup, veinsDeLaSimulada[i]);
            for (int v = 0; v < veinsDelGrupDelVei.Count; v++)
            {
                if (!pecesPerComprovar.Contains(veinsDelGrupDelVei[v])) pecesPerComprovar.Add(veinsDelGrupDelVei[v]);
            }

            List<Peça> veinsDelGrupDelVeiContemplantCamins = grups.VeinsAmbCami(grups.Grup, veinsDeLaSimulada[i]);
            for (int v = 0; v < veinsDelGrupDelVeiContemplantCamins.Count; v++)
            {
                if (!pecesPerComprovar.Contains(veinsDelGrupDelVeiContemplantCamins[v])) pecesPerComprovar.Add(veinsDelGrupDelVeiContemplantCamins[v]);
            }
        }

        for (int c = 0; c < peçaSimulada.Condicions.Length; c++)
        {
            List<Peça> veinsAcordingToOptions = peçaSimulada.Condicions[c].GetVeinsAcordingToOptions(peçaSimulada, grups, cami);
            for (int v = 0; v < veinsAcordingToOptions.Count; v++)
            {
                if (!pecesPerComprovar.Contains(veinsAcordingToOptions[v])) pecesPerComprovar.Add(veinsAcordingToOptions[v]);
            }
        }
    }

    void SimularProximitat() => proximitat.Process(pecesPerComprovar, MostrarCanvis, false);

    void MostrarCanvis(List<Peça> comprovades, List<Peça> canviades)
    {
        if (amagarInformacioBuffer)
        {
            amagarInformacioBuffer = false;

            ResetCanvis(comprovades, canviades);


            Debugar.LogError("FINALITZAT! (PREDICCIONS)");
            return;
        }

        MostrarCanvis(canviades);

        if (peçaSimulada.SubestatIgualA(casa))
        {
            MostrarMesHabitants(canviades);
        }
        else
        {
            MostrarMenysHabitants(canviades);
        }

        MostrarConnexions(grups);

        ResetCanvis(comprovades, canviades);
        predint = false;
        Debugar.LogError("FINALITZAT! (PREDICCIONS)");
    }


    void MostrarCanvis(List<Peça> canviades)
    {
        for (int i = 0; i < canviades.Count; i++)
        {
            visualitzacions.PredirCanvi(canviades[i].Coordenades);
            Debugar.LogError($"***Mostrar Canvis a {canviades[i].gameObject.name}***");
        }
    }
    void MostrarMesHabitants(List<Peça> canviades)
    {
        for (int i = 0; i < pecesPerComprovar.Count; i++)
        {
            if (pecesPerComprovar[i].SubestatIgualA(casa) && !canviades.Contains(pecesPerComprovar[i]))
            {
                if (peçaSimulada.VeinsPeça.Contains(pecesPerComprovar[i]))
                {
                    visualitzacions.PredirMesHabitants(pecesPerComprovar[i].Coordenades);
                    Debugar.LogError($"***Mostrar + Needs a {pecesPerComprovar[i].gameObject.name}***");
                }
            }
        }
    }
    void MostrarMenysHabitants(List<Peça> canviades)
    {
        for (int i = 0; i < canviades.Count; i++)
        {
            //En el cas que: Jo NO vulgui colocar una casa, i alguna de les peces canviades fos una casa.
            if (canviades[i].SubestatIgualA(casa))
            {
                visualitzacions.PredirMenysHabitants(canviades[i].Coordenades);
                Debugar.LogError($"***Mostrar - Needs a {canviades[i].gameObject.name}***");
            }
        }
    }
    void MostrarConnexions(Grups grups)
    {
        for (int i = 0; i < grups.ConnexionsFetes.Count; i++)
        {
            visualitzacions.PredirConnexio(grups.ConnexionsFetes[i].Coordenades);
            Debugar.LogError($"***Mostrar Connexio a {grups.ConnexionsFetes[i].gameObject.name}***");
        }
        grups.ConnexionsFetes.Clear();
    }





    void ResetCanvis(List<Peça> comprovades, List<Peça> canviades)
    {
        for (int i = 0; i < comprovades.Count; i++)
        {
            if (comprovades[i].Ocupat) comprovades[i].DesocuparPerPrediccio();
        }
        for (int i = 0; i < canviades.Count; i++)
        {
            if (canviades[i].Ocupat) canviades[i].DesocuparPerPrediccio();
        }

        Grid.Instance.SimularFinal(peçaSimulada);
        predint = false;
    }

    public void AmagarInformacioMostrada(Vector2Int coordenada)
    {
        if (!predint)
            visualitzacions.AmagarPrediccions();
        else
            amagarInformacioBuffer = true;
        Debugar.LogError("***Si hi ha informacio mostrada, s'esborra***");
        //simulant = false;
    }
    public void FinalitzacioForçada()
    {
        //grid.SimularFinal(simulada);
        predint = false;
    }
}
