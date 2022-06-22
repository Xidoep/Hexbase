using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Proximitat")]
public class Proximitat : ScriptableObject
{
    //ACTIVADORS
    //1.- Si estas aprop d'1 o + peces d'un tipus. i d'un nivell concret o de qualsevol.
    //2.- Si formen part d'un grup de X o +.
    //3.- Com l'1, pero en tot el grup.
    [SerializeField] Queue<Hexagon> peces;
    System.Action enFinalitzar;

    bool _iniciat;
    Hexagon _actual;
    bool _canviar;
    void OnEnable()
    {
        peces = new Queue<Hexagon>();
        _iniciat = false;
        _actual = null;
        _canviar = false;
    }
    public void Add(Hexagon pe�a, System.Action enFinalitzar = null)
    {
        if(!peces.Contains(pe�a)) peces.Enqueue(pe�a);

        //if not started start the proces.
        if (enFinalitzar != null) this.enFinalitzar = enFinalitzar;
        if (!_iniciat) Process();
    }
    void AddVeins(Hexagon pe�a)
    {
        for (int i = 0; i < pe�a.Veins.Length; i++)
        {
            if (pe�a.Veins[i].EsPe�a)
                Add(pe�a.Veins[i]);
        }
        if (!_iniciat) Process();
    }

    void Process()
    {
        Debug.Log("Process");
        _iniciat = true;

        if(peces.Count == 0)
        {
            _iniciat = false;
            Debug.LogError("FINALITZAT!");
            enFinalitzar.Invoke();
            return;
        }

        _actual = peces.Dequeue();
        _canviar = false;

        Debug.LogError(_actual.name);
        for (int i = 0; i < _actual.Subestat.Condicions.Length; i++)
        {
            if (_actual.Subestat.Condicions[i].Comprovar(_actual))
            {
                AddVeins(_actual);
                _canviar = true;
                break;
            }
        }

        XS_Coroutine.StartCoroutine_Ending(0.01f, Process);
    }

}


