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
    Queue<Pe�a> peces;
    List<Pe�a> comprovades;
    List<Canvis> canviades;
    System.Action<List<Pe�a>, List<Canvis>> enFinalitzar;
    Pe�a _actual;
    float stepTime = 0.1f;


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

        List<Pe�a> grup = grups.Peces(grups.Grup, pe�a);
        if(grup != null)
        {
            for (int i = 0; i < grup.Count; i++)
            {
                if (!tmp.Contains(grup[i])) tmp.Add(grup[i]);
            }

            List<Pe�a> veinsGrupAmbCami = grups.VeinsAmbCami(grups.Grup, pe�a);
            for (int i = 0; i < veinsGrupAmbCami.Count; i++)
            {
                if (!tmp.Contains(veinsGrupAmbCami[i])) tmp.Add(veinsGrupAmbCami[i]);
            }
        }

      

        return tmp;
    }

    public void Process(List<Pe�a> peces, System.Action<List<Pe�a>, List<Canvis>> enFinalitzar, bool canviar = true)
    {
        Debugar.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Pe�a>(peces);
        comprovades = new List<Pe�a>();
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
    public void ProcessReceptes(List<Pe�a> peces, System.Action<List<Pe�a>, List<Canvis>> enFinalitzar, bool canviar = true)
    {
        this.peces = new Queue<Pe�a>(peces);
        comprovades = new List<Pe�a>();
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

        List<Pe�a> veins = _actual.VeinsPe�a;
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

    void MarcarComCanviada(Pe�a pe�a, bool canviar, int experiencia)
    {
        //Debugar.LogError("CANVIAR");
        if (canviar)
        {
            //_actual.Condicions[i].Canviar(_actual, GunayarExperienciaIVisualitzarSiCal);
            canviades.Add(new Canvis(pe�a, experiencia));
            Add(_actual);
        }
        else
        {
            if (!canviades.Contains(new Canvis(pe�a, experiencia)))
            {
                canviades.Add(new Canvis(pe�a, experiencia));
                Add(_actual);
            }
        }
    }

    /*void GunayarExperienciaIVisualitzarSiCal(Pe�a pe�a, int experiencia)
    {
        resoldre.Nivell.GuanyarExperiencia(experiencia);
        if (experiencia > 0) 
        {
            visualitzacions.AddGuanyarPunts(pe�a.transform.position);
            //canviades.Add(new Canvis(pe�a, experiencia));
            //visualitzacions.GuanyarPunts(pe�a.transform.position, 1.5f);
        }
    }*/


    public struct Canvis
    {
        public Canvis(Pe�a pe�a, int experiencia)
        {
            this.pe�a = pe�a;
            this.experiencia = experiencia;
        }
        Pe�a pe�a;
        int experiencia;

        public Pe�a Pe�a => pe�a;
        public int Experiencia => experiencia;
    }
}


