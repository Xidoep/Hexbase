using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Proximitat")]
public class Proximitat : ScriptableObject
{
    [SerializeField] PoolPeces pool;
    //ACTIVADORS
    //1.- Si estas aprop d'1 o + peces d'un tipus. i d'un nivell concret o de qualsevol.
    //2.- Si formen part d'un grup de X o +.
    //3.- Com l'1, pero en tot el grup.
    [SerializeField] Grups grups;
    [SerializeField] Estat cami;

    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Fase_Resoldre resoldre;
    
    [SerializeField] Visualitzacions visualitzacions;


    //INTERN
    Queue<Pe�a> peces;
    List<Pe�a> comprovades;
    List<Pe�a> canviades;
    System.Action<List<Pe�a>, List<Pe�a>> enFinalitzar;
    Pe�a _actual;

    bool simulant = false;
    Pe�a simulada;
    List<Pe�a> simulacioComprovar;
    

    Grid grid;

    void OnEnable()
    {
        peces = new Queue<Pe�a>();
        _actual = null;
    }
    public void Add(Pe�a pe�a)
    {
        List<Pe�a> tmp = GetPecesToComprovar(pe�a);
        for (int i = 0; i < tmp.Count; i++)
        {
            if (!peces.Contains(tmp[i])) peces.Enqueue(tmp[i]);
        }
    }

    public List<Pe�a> GetPecesToComprovar(Pe�a pe�a)
    {
        List<Pe�a> tmp = new List<Pe�a>();

        List<Pe�a> veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Count; i++)
        {
            if (!tmp.Contains(veins[i])) tmp.Add(veins[i]);
        }

        List<Pe�a> grup = grups.Peces(pe�a);
        if(grup != null)
        {
            for (int i = 0; i < grup.Count; i++)
            {
                if (!tmp.Contains(grup[i])) tmp.Add(grup[i]);
            }

            List<Pe�a> veinsGrupAmbCami = grups.VeinsAmbCami(pe�a);
            for (int i = 0; i < veinsGrupAmbCami.Count; i++)
            {
                if (!tmp.Contains(veinsGrupAmbCami[i])) tmp.Add(veinsGrupAmbCami[i]);
            }
        }

      

        return tmp;
    }

    public void Process(List<Pe�a> peces, System.Action<List<Pe�a>, List<Pe�a>> enFinalitzar, bool canviar = true)
    {
        Debugar.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Pe�a>(peces);
        comprovades = new List<Pe�a>();
        canviades = new List<Pe�a>();
        this.enFinalitzar = enFinalitzar;
        Debugar.Log("Process");
        /*for (int i = 0; i < peces.Count; i++)
        {
            Debugar.LogError($"{peces[i].gameObject.name}");
        }*/
        Step(canviar);
    }

    void Step(bool canviar)
    {
        //Debugar.LogError($"Step {peces.Count}");
        if (peces.Count == 0)
        {
            Debugar.LogError("FINALITZAT!");
            enFinalitzar.Invoke(comprovades, canviades);
            return;
        }

        _actual = peces.Dequeue();

        if(_actual == null)
        {
            Debugar.LogError("DESTRUIDA!");
            Step(canviar);
        }

        Debugar.LogError($"Comprovar pe�a {_actual.gameObject.name} que {_actual.Subestat.name} te {_actual.Condicions.Length} condicions");
        for (int i = 0; i < _actual.Condicions.Length; i++)
        {
            //Debugar.LogError($"Condicio {i}?");
            if (_actual.Condicions[i].Comprovar(_actual, grups, cami, null))
            {
                //Debugar.LogError("CANVIAR");
                if (canviar) 
                { 
                    _actual.Condicions[i].Canviar(_actual, GunayarExperienciaIVisualitzarSiCal);
                    Add(_actual);
                    canviades.Add(_actual);
                }
                else
                {
                    if (!canviades.Contains(_actual))
                    {
                        canviades.Add(_actual);
                        Add(_actual);
                    }
                }
                
                break;
            }
        }

        if (!comprovades.Contains(_actual)) comprovades.Add(_actual);

        Step(canviar);
    }

    public void PossiblesCombinacions(Vector2Int coordenada)
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        if(simulant)
            grid.SimularFinal(simulada.Coordenades);

        simulada = grid.SimularInici(colocar.Seleccionada, coordenada);
        Debugar.LogError($"COMPROVAR POTENCIAL RANURA {simulada.Coordenades} que te {grid.VeinsPe�a(coordenada).Count} veines------------");
        simulacioComprovar = new List<Pe�a>() { simulada };
        simulacioComprovar.AddRange(grid.VeinsPe�a(coordenada));

        //grups.Agrupdar(simulada, SimulacioProcessar);

        if (!simulant)
        {
            simulant = true;
            grups.Agrupdar(simulada, SimulacioProcessar);
        }
        else
        {
            //grups.RecuperaVersioAnterior();
            grups.Interrompre();
            grups.Agrupdar(simulada, SimulacioProcessar);
        }
  
    }

    void SimulacioProcessar() => Process(simulacioComprovar, MostrarInformacio, false);


    void MostrarInformacio(List<Pe�a> comprovades, List<Pe�a> canviades)
    {
        
        for (int i = 0; i < canviades.Count; i++)
        {
            Debugar.LogError($"***Mostrar Info  de {canviades[i].gameObject.name}***");
        }
        grups.RecuperaVersioAnterior();
        grid.SimularFinal(simulada.Coordenades);
        simulant = false;
        Debugar.LogError("----------------------------POTENCIALS");
    }

   
    public void AmagarInformacioMostrada(Vector2Int coordenada)
    {
        Debugar.LogError("***Si hi ha informacio mostrada, s'esborra***");
    }

    void GunayarExperienciaIVisualitzarSiCal(Pe�a pe�a, int experiencia)
    {
        resoldre.Nivell.GuanyarExperiencia(experiencia);
        if (experiencia > 0) 
        {
            visualitzacions.AddGuanyarPunts(pe�a.transform.position);
            //visualitzacions.GuanyarPunts(pe�a.transform.position, 1.5f);
        }
    }

}


