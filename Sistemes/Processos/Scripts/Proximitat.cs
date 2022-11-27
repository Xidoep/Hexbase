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
    [SerializeField] Queue<Pe�a> peces;
    [SerializeField] Grups grups;
    [SerializeField] Estat cami;
    List<Pe�a> comprovades;
    List<Pe�a> canviades;
    System.Action<List<Pe�a>, List<Pe�a>> enFinalitzar;
    //System.Action<List<Pe�a>> enFinalitzar;

    //INTERN
    //bool _iniciat;
    Pe�a _actual;
    //bool _canviar;
    List<Pe�a> veins;

    void OnEnable()
    {
        peces = new Queue<Pe�a>();
        //_iniciat = false;
        _actual = null;
        //_canviar = false;
    }
    public void Add(Pe�a pe�a)
    {
        List<Pe�a> tmp = GetPecesToComprovar(pe�a);
        for (int i = 0; i < tmp.Count; i++)
        {
            if (!peces.Contains(tmp[i])) peces.Enqueue(tmp[i]);
        }

       

        /*List<Pe�a> veins = pe�a.VeinsPe�a;
        for (int i = 0; i < veins.Count; i++)
        {
            if (!peces.Contains(veins[i])) peces.Enqueue(veins[i]);
        }

        List<Pe�a> grup = grups.Peces(pe�a);
        for (int i = 0; i < grup.Count; i++)
        {
            if (!peces.Contains(grup[i])) peces.Enqueue(grup[i]);
        }

        List<Pe�a> veinsGrupAmbCami = grups.VeinsAmbCami(pe�a);
        for (int i = 0; i < veinsGrupAmbCami.Count; i++)
        {
            if (!peces.Contains(veinsGrupAmbCami[i])) peces.Enqueue(veinsGrupAmbCami[i]);
        }*/
        //if not started start the proces.
        //if (enFinalitzar != null) this.enFinalitzar = enFinalitzar;
        //if (!_iniciat) Process();
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
        for (int i = 0; i < grup.Count; i++)
        {
            if (!tmp.Contains(grup[i])) tmp.Add(grup[i]);
        }

        List<Pe�a> veinsGrupAmbCami = grups.VeinsAmbCami(pe�a);
        for (int i = 0; i < veinsGrupAmbCami.Count; i++)
        {
            if (!tmp.Contains(veinsGrupAmbCami[i])) tmp.Add(veinsGrupAmbCami[i]);
        }
        return tmp;
    }

    public void Process(List<Pe�a> peces, System.Action<List<Pe�a>, List<Pe�a>> enFinalitzar)
    //public void Process(List<Pe�a> peces, System.Action<List<Pe�a>> enFinalitzar)
    {
        Debugar.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Pe�a>(peces);
        comprovades = new List<Pe�a>();
        canviades = new List<Pe�a>();
        this.enFinalitzar = enFinalitzar;
        Debugar.Log("Process");
        Step();
    }

    void Step()
    {
        //_iniciat = true;

        if(peces.Count == 0)
        {
            //_iniciat = false;
            Debugar.LogError("FINALITZAT!");
            enFinalitzar.Invoke(comprovades, canviades);
            //enFinalitzar.Invoke(comprovades);
            return;
        }

        _actual = peces.Dequeue();
        //_canviar = false;

        Debugar.LogError(_actual.name);
        for (int i = 0; i < _actual.Condicions.Length; i++)
        {
            if (_actual.Condicions[i].Comprovar(_actual, this, grups, cami))
            {

                //pool.Add(_actual.Subestat.Punts);
                Add(_actual);
                canviades.Add(_actual);
                //_canviar = true;
                break;
            }
        }

        if (!comprovades.Contains(_actual)) comprovades.Add(_actual);

        Step();
        //XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }

}


