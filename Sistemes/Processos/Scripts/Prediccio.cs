using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Prediccio")]
public class Prediccio : ScriptableObject
{
    [SerializeField] FasesControlador controlador;
    [SerializeField] Fase_Colocar colocar;
    //[SerializeField] Visualitzacions visualitzacions;
    [Space(10)]
    [SerializeField] Grups grups;
    [SerializeField] Proximitat proximitat;
    [SerializeField] Repoblar repoblar;
    [Space(10)]
    [SerializeField] Subestat casa;
    [SerializeField] Estat cami;


    [Apartat("Debug")]
    [SerializeField] Pe�a pe�aSimulada;
    [SerializeField] bool predint = false;
    [SerializeField] List<Pe�a> pecesPerComprovar;

    [SerializeField] List<Grup> grupsSimulats;

    bool amagarInformacioBuffer = false;

    System.Action onStartPrediccio;
    System.Action onEndPrediccio;

    public System.Action OnStartPrediccio { get => onStartPrediccio; set => onStartPrediccio = value; }
    public System.Action OnEndPrediccio { get => onEndPrediccio; set => onEndPrediccio = value; }

    bool Predint
    {
        get
        {
            return predint;
        }
        set
        {
            predint = value;
            if (predint)
                onStartPrediccio?.Invoke();
        }
    }


    private void OnEnable()
    {
        predint = false;
        pe�aSimulada = null;
    }

    public void Predir(Vector2Int coordenada)
    {
        Debug.Log("Ara no prediu, perque no m'interessa");
        return;

        if (!controlador.EstaEnFase(colocar))
        {
            Debugar.LogError("INTENTAR PREDIR FORA DE LA FASE COLOCAR!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Debugar.LogError($"--------------PREDIR ({coordenada})---------------");

        if (!Predint)
        {
            Predint = true;

            CrearCopiaDeGrups();

            CrearPe�aSimulada(coordenada);

            BuscarPecesPerComprovar();

            grups.Agrupdar(grupsSimulats, pe�aSimulada, SimularProximitat);
        }
        else
        {
            Debugar.LogError("predint...");

            //grups.RecuperaVersioAnterior();
            Grid.Instance.SimularFinal(pe�aSimulada);
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
    void CrearPe�aSimulada(Vector2Int coordenada)
    {
        pe�aSimulada = Grid.Instance.SimularInici(colocar.Seleccionada, coordenada);
    }
    void BuscarPecesPerComprovar()
    {
        pecesPerComprovar = new List<Pe�a>() { pe�aSimulada };

        List<Pe�a> veinsDeLaSimulada = pe�aSimulada.VeinsPe�a;
        pecesPerComprovar.AddRange(veinsDeLaSimulada);

        for (int i = 0; i < veinsDeLaSimulada.Count; i++)
        {
            List<Pe�a> veinsDelGrupDelVei = grups.Veins(grups.Grup, veinsDeLaSimulada[i]);
            for (int v = 0; v < veinsDelGrupDelVei.Count; v++)
            {
                if (!pecesPerComprovar.Contains(veinsDelGrupDelVei[v])) pecesPerComprovar.Add(veinsDelGrupDelVei[v]);
            }

            List<Pe�a> veinsDelGrupDelVeiContemplantCamins = grups.VeinsAmbCami(grups.Grup, veinsDeLaSimulada[i]);
            for (int v = 0; v < veinsDelGrupDelVeiContemplantCamins.Count; v++)
            {
                if (!pecesPerComprovar.Contains(veinsDelGrupDelVeiContemplantCamins[v])) pecesPerComprovar.Add(veinsDelGrupDelVeiContemplantCamins[v]);
            }
        }

        /*for (int c = 0; c < pe�aSimulada.Condicions.Length; c++)
        {
            List<Pe�a> veinsAcordingToOptions = pe�aSimulada.Condicions[c].GetVeinsAcordingToOptions(pe�aSimulada, grups, cami);
            for (int v = 0; v < veinsAcordingToOptions.Count; v++)
            {
                if (!pecesPerComprovar.Contains(veinsAcordingToOptions[v])) pecesPerComprovar.Add(veinsAcordingToOptions[v]);
            }
        }*/
    }

    void SimularProximitat() => proximitat.Process(pecesPerComprovar, MostrarCanvis, false);

    void MostrarCanvis(List<Pe�a> comprovades, List<Proximitat.Canvis> canviades)
    {
        if (amagarInformacioBuffer)
        {
            amagarInformacioBuffer = false;

            ResetCanvis(comprovades, canviades);


            Debugar.LogError("FINALITZAT! (PREDICCIONS)");
            return;
        }

        MostrarCanvis(canviades);

        if (pe�aSimulada.SubestatIgualA(casa))
        {
            MostrarMesHabitants(canviades);
        }
        else
        {
            MostrarMenysHabitants(canviades);
        }

        MostrarConnexions(grups);

        ResetCanvis(comprovades, canviades);
        Predint = false;
        Debugar.LogError("FINALITZAT! (PREDICCIONS)");
    }


    void MostrarCanvis(List<Proximitat.Canvis> canviades)
    {
        for (int i = 0; i < canviades.Count; i++)
        {
            //visualitzacions.PredirCanvi(canviades[i].Pe�a.Coordenades);
            Debugar.LogError($"***Mostrar Canvis a {canviades[i].Pe�a.gameObject.name}***");
        }
    }
    void MostrarMesHabitants(List<Proximitat.Canvis> canviades)
    {
        for (int i = 0; i < pecesPerComprovar.Count; i++)
        {
            if (pecesPerComprovar[i].SubestatIgualA(casa))
            {
                bool contains = false;
                for (int c = 0; c < canviades.Count; c++)
                {
                    if(canviades[c].Pe�a == pecesPerComprovar[i])
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    if (pe�aSimulada.VeinsPe�a.Contains(pecesPerComprovar[i]))
                    {
                        //visualitzacions.PredirMesHabitants(pecesPerComprovar[i].Coordenades);
                        Debugar.LogError($"***Mostrar + Needs a {pecesPerComprovar[i].gameObject.name}***");
                    }
                }
            }
        }
    }
    void MostrarMenysHabitants(List<Proximitat.Canvis> canviades)
    {
        for (int i = 0; i < canviades.Count; i++)
        {
            //En el cas que: Jo NO vulgui colocar una casa, i alguna de les peces canviades fos una casa.
            if (canviades[i].Pe�a.SubestatIgualA(casa))
            {
                //visualitzacions.PredirMenysHabitants(canviades[i].Pe�a.Coordenades);
                Debugar.LogError($"***Mostrar - Needs a {canviades[i].Pe�a.gameObject.name}***");
            }
        }
    }
    void MostrarConnexions(Grups grups)
    {
        for (int i = 0; i < grups.ConnexionsFetes.Count; i++)
        {
            //visualitzacions.PredirConnexio(grups.ConnexionsFetes[i].Coordenades);
            Debugar.LogError($"***Mostrar Connexio a {grups.ConnexionsFetes[i].gameObject.name}***");
        }
        grups.ConnexionsFetes.Clear();
    }





    void ResetCanvis(List<Pe�a> comprovades, List<Proximitat.Canvis> canviades)
    {
        for (int i = 0; i < comprovades.Count; i++)
        {
            if (comprovades[i].EstaConnectat) comprovades[i].DesocuparPerPrediccio();
        }
        for (int i = 0; i < canviades.Count; i++)
        {
            if (canviades[i].Pe�a.EstaConnectat) canviades[i].Pe�a.DesocuparPerPrediccio();
        }

        Grid.Instance.SimularFinal(pe�aSimulada);
        Predint = false;
    }

    public void AmagarInformacioMostrada()
    {
        if (!Predint)
        {
            //visualitzacions.AmagarPrediccions();
        }
        else
            amagarInformacioBuffer = true;
        Debugar.LogError("***Si hi ha informacio mostrada, s'esborra***");
        //simulant = false;
        if (pe�aSimulada != null)
        {
            Destroy(pe�aSimulada.gameObject);
            onEndPrediccio?.Invoke();
        }
    }
    public void FinalitzacioFor�ada()
    {
        //grid.SimularFinal(simulada);
        Predint = false;
        AmagarInformacioMostrada();
    }
}
