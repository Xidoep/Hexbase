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
    [SerializeField] Queue<Peça> peces;
    List<Peça> comprovades;
    System.Action<List<Peça>> enFinalitzar;

    //INTERN
    //bool _iniciat;
    Peça _actual;
    //bool _canviar;
    List<Peça> veins;

    void OnEnable()
    {
        peces = new Queue<Peça>();
        //_iniciat = false;
        _actual = null;
        //_canviar = false;
    }
    public void Add(Peça peça, System.Action enFinalitzar = null)
    {
        if(!peces.Contains(peça)) peces.Enqueue(peça);

        //if not started start the proces.
        //if (enFinalitzar != null) this.enFinalitzar = enFinalitzar;
        //if (!_iniciat) Process();
    }

    void AddVeins(Peça peça)
    {
        veins = peça.VeinsPeça;

        for (int i = 0; i < veins.Count; i++)
        {
            //if (veins[i].EsPeça)
            Add(veins[i]);
        }
        //if (!_iniciat) Process();
    }

    public void Process(List<Peça> peces, System.Action<List<Peça>> enFinalitzar)
    {
        Debug.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Peça>(peces);
        comprovades = new List<Peça>();
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


