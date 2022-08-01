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
    List<Pe�a> comprovades;
    System.Action<List<Pe�a>> enFinalitzar;

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
    public void Add(Pe�a pe�a, System.Action enFinalitzar = null)
    {
        if(!peces.Contains(pe�a)) peces.Enqueue(pe�a);

        //if not started start the proces.
        //if (enFinalitzar != null) this.enFinalitzar = enFinalitzar;
        //if (!_iniciat) Process();
    }

    void AddVeins(Pe�a pe�a)
    {
        veins = pe�a.VeinsPe�a;

        for (int i = 0; i < veins.Count; i++)
        {
            //if (veins[i].EsPe�a)
            Add(veins[i]);
        }
        //if (!_iniciat) Process();
    }

    public void Process(List<Pe�a> peces, System.Action<List<Pe�a>> enFinalitzar)
    {
        Debug.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Pe�a>(peces);
        comprovades = new List<Pe�a>();
        this.enFinalitzar = enFinalitzar;
        Debug.Log("Process");
        Step();
    }

    void Step()
    {
        //_iniciat = true;

        if(peces.Count == 0)
        {
            //_iniciat = false;
            Debug.LogError("FINALITZAT!");
            enFinalitzar.Invoke(comprovades);
            return;
        }

        _actual = peces.Dequeue();
        //_canviar = false;

        Debug.LogError(_actual.name);
        for (int i = 0; i < _actual.Condicions.Length; i++)
        {
            if (_actual.Condicions[i].Comprovar(_actual))
            {
                
                pool.Add(_actual.Subestat.Punts);
                AddVeins(_actual);
                //_canviar = true;
                break;
            }
        }

        if (!comprovades.Contains(_actual)) comprovades.Add(_actual);

        XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }

}


