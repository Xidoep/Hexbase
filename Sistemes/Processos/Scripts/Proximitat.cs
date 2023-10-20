using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Proximitat")]
public class Proximitat : ScriptableObject
{
    //[SerializeScriptableObject] [SerializeField] PoolPeces pool;

    [SerializeScriptableObject] [SerializeField] Grups grups;
    //[SerializeScriptableObject] [SerializeField] Estat cami;

    //[SerializeScriptableObject] [SerializeField] Fase_Colocar colocar;
    //[SerializeScriptableObject] [SerializeField] Fase_Resoldre resoldre;

    //[SerializeScriptableObject] [SerializeField] Visualitzacions visualitzacions;


    //INTERN
    Queue<Peça> peces;
    List<Peça> comprovades;
    List<Canvis> canviades;
    System.Action<List<Peça>, List<Canvis>> enFinalitzar;
    Peça _actual;
    float stepTime = 0.1f;


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

        List<Peça> grup = grups.Peces(grups.Grup, peça);
        if(grup != null)
        {
            for (int i = 0; i < grup.Count; i++)
            {
                if (!tmp.Contains(grup[i])) tmp.Add(grup[i]);
            }

            List<Peça> veinsGrupAmbCami = grups.VeinsAmbCami(grups.Grup, peça);
            for (int i = 0; i < veinsGrupAmbCami.Count; i++)
            {
                if (!tmp.Contains(veinsGrupAmbCami[i])) tmp.Add(veinsGrupAmbCami[i]);
            }
        }

      

        return tmp;
    }

    public void Process(List<Peça> peces, System.Action<List<Peça>, List<Canvis>> enFinalitzar, bool canviar = true)
    {
        Debugar.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Peça>(peces);
        comprovades = new List<Peça>();
        canviades = new List<Canvis>();
        this.enFinalitzar = enFinalitzar;
        _actual = null;
        Debugar.Log("Process");
        /*for (int i = 0; i < peces.Count; i++)
        {
            Debugar.LogError($"{peces[i].gameObject.name}");
        }*/
        Step(canviar);
    }
    public void ProcessReceptes(List<Peça> peces, System.Action<List<Peça>, List<Canvis>> enFinalitzar, bool canviar = true)
    {
        this.peces = new Queue<Peça>(peces);
        comprovades = new List<Peça>();
        canviades = new List<Canvis>();
        this.enFinalitzar = enFinalitzar;
        _actual = null;
        Debugar.Log("Process");

        StepRecepta(canviar);
    }

    void StepRecepta(bool canviar)
    {
        if (peces.Count == 0)
        {
            Debugar.LogError("FINALITZAT! (PROXIMITAT)");
            enFinalitzar.Invoke(comprovades, canviades);
            return;
        }

        _actual = peces.Dequeue();

        if (_actual == null)
        {
            StepRecepta(canviar);
        }

        List<Peça> veins = _actual.VeinsPeça;
        Debug.Log($"Processar {_actual.name}");
        Processador.Proces confirmacio = _actual.processador.IntentarProcessar(_actual, new List<object>(veins), true);
        if (confirmacio.confirmat)
        //if (_actual.processador.IntentarProcessar(_actual, new List<object>(veins), true))
        {
            Debug.Log("La recepta s'ha complert!");

            MarcarComCanviada(_actual, canviar, confirmacio.experiencia);
        }


        if (!comprovades.Contains(_actual)) comprovades.Add(_actual);
        //StepRecepta(canviar);


        if (confirmacio.confirmat)
        {
            XS_Coroutine.StartCoroutine_Ending(stepTime, StepRecepta, canviar);
        }
        else
        {
            StepRecepta(canviar);
        }
    }

    void Step(bool canviar)
    {
        /*
        //Debugar.LogError($"Step {peces.Count}");
        if (peces.Count == 0)
        {
            Debugar.LogError("FINALITZAT! (PROXIMITAT)");
            enFinalitzar.Invoke(comprovades, canviades);
            return;
        }

        _actual = peces.Dequeue();

        if(_actual == null)
        {
            Debugar.LogError("DESTRUIDA!");
            Step(canviar);
        }

        for (int i = 0; i < _actual.Condicions.Length; i++)
        {
            if (_actual.Condicions[i].Comprovar(_actual, grups, cami, canviar, MarcarComCanviada, GunayarExperienciaIVisualitzarSiCal))
            {
                break;
            }
        }

        if (!comprovades.Contains(_actual)) comprovades.Add(_actual);

        Step(canviar);
        */
    }

    void MarcarComCanviada(Peça peça, bool canviar, int experiencia)
    {
        //Debugar.LogError("CANVIAR");
        if (canviar)
        {
            //_actual.Condicions[i].Canviar(_actual, GunayarExperienciaIVisualitzarSiCal);
            canviades.Add(new Canvis(peça, experiencia));
            Add(_actual);
        }
        else
        {
            if (!canviades.Contains(new Canvis(peça, experiencia)))
            {
                canviades.Add(new Canvis(peça, experiencia));
                Add(_actual);
            }
        }
    }

    /*void GunayarExperienciaIVisualitzarSiCal(Peça peça, int experiencia)
    {
        resoldre.Nivell.GuanyarExperiencia(experiencia);
        if (experiencia > 0) 
        {
            visualitzacions.AddGuanyarPunts(peça.transform.position);
            //canviades.Add(new Canvis(peça, experiencia));
            //visualitzacions.GuanyarPunts(peça.transform.position, 1.5f);
        }
    }*/


    public struct Canvis
    {
        public Canvis(Peça peça, int experiencia)
        {
            this.peça = peça;
            this.experiencia = experiencia;
        }
        Peça peça;
        int experiencia;

        public Peça Peça => peça;
        public int Experiencia => experiencia;
    }
}


