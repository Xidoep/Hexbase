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
    public void Add(Hexagon peça)
    {
        if(!peces.Contains(peça)) peces.Enqueue(peça);
        
        //if not started start the proces.
        if (!_iniciat) Process();
    }
    void AddVeins(Hexagon peça)
    {
        for (int i = 0; i < peça.Veins.Length; i++)
        {
            if (peça.Veins[i].EsPeça)
                Add(peça.Veins[i]);
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
            return;
        }

        _actual = peces.Dequeue();
        _canviar = false;

        Debug.LogError(_actual.name);
        for (int i = 0; i < _actual.Subestat.Condicions.Length; i++)
        {
            Debug.LogError($"{_actual.name} - {_actual.Subestat.Condicions[i].Comprovar(_actual)}", _actual);
            if (_actual.Subestat.Condicions[i].Comprovar(_actual))
            {
                AddVeins(_actual);
                _canviar = true;
                break;
            }
        }

        XS_Coroutine.StartCoroutine_Ending(1, Process);
    }

}


