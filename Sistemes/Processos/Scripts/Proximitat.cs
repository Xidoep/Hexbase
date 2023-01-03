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
    Queue<Peça> peces;
    List<Peça> comprovades;
    List<Peça> canviades;
    System.Action<List<Peça>, List<Peça>> enFinalitzar;
    Peça _actual;

    bool simulant = false;
    Peça simulada;
    List<Peça> simulacioComprovar;
    

    Grid grid;

    void OnEnable()
    {
        peces = new Queue<Peça>();
        _actual = null;
    }
    public void Add(Peça peça)
    {
        List<Peça> tmp = GetPecesToComprovar(peça);
        for (int i = 0; i < tmp.Count; i++)
        {
            if (!peces.Contains(tmp[i])) peces.Enqueue(tmp[i]);
        }
    }

    public List<Peça> GetPecesToComprovar(Peça peça)
    {
        List<Peça> tmp = new List<Peça>();

        List<Peça> veins = peça.VeinsPeça;
        for (int i = 0; i < veins.Count; i++)
        {
            if (!tmp.Contains(veins[i])) tmp.Add(veins[i]);
        }

        List<Peça> grup = grups.Peces(peça);
        if(grup != null)
        {
            for (int i = 0; i < grup.Count; i++)
            {
                if (!tmp.Contains(grup[i])) tmp.Add(grup[i]);
            }

            List<Peça> veinsGrupAmbCami = grups.VeinsAmbCami(peça);
            for (int i = 0; i < veinsGrupAmbCami.Count; i++)
            {
                if (!tmp.Contains(veinsGrupAmbCami[i])) tmp.Add(veinsGrupAmbCami[i]);
            }
        }

      

        return tmp;
    }

    public void Process(List<Peça> peces, System.Action<List<Peça>, List<Peça>> enFinalitzar, bool canviar = true)
    {
        Debugar.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Peça>(peces);
        comprovades = new List<Peça>();
        canviades = new List<Peça>();
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

        Debugar.LogError($"Comprovar peça {_actual.gameObject.name} que {_actual.Subestat.name} te {_actual.Condicions.Length} condicions");
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
        Debugar.LogError($"COMPROVAR POTENCIAL RANURA {simulada.Coordenades} que te {grid.VeinsPeça(coordenada).Count} veines------------");
        simulacioComprovar = new List<Peça>() { simulada };
        simulacioComprovar.AddRange(grid.VeinsPeça(coordenada));

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


    void MostrarInformacio(List<Peça> comprovades, List<Peça> canviades)
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

    void GunayarExperienciaIVisualitzarSiCal(Peça peça, int experiencia)
    {
        resoldre.Nivell.GuanyarExperiencia(experiencia);
        if (experiencia > 0) 
        {
            visualitzacions.AddGuanyarPunts(peça.transform.position);
            //visualitzacions.GuanyarPunts(peça.transform.position, 1.5f);
        }
    }

}


