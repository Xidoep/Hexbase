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
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Visualitzacions visualitzacions;

    //INTERN
    Queue<Peça> peces;
    List<Peça> comprovades;
    List<Peça> canviades;
    System.Action<List<Peça>, List<Peça>> enFinalitzar;
    Peça _actual;

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
        for (int i = 0; i < grup.Count; i++)
        {
            if (!tmp.Contains(grup[i])) tmp.Add(grup[i]);
        }

        List<Peça> veinsGrupAmbCami = grups.VeinsAmbCami(peça);
        for (int i = 0; i < veinsGrupAmbCami.Count; i++)
        {
            if (!tmp.Contains(veinsGrupAmbCami[i])) tmp.Add(veinsGrupAmbCami[i]);
        }
        return tmp;
    }

    public void Process(List<Peça> peces, System.Action<List<Peça>, List<Peça>> enFinalitzar)
    {
        Debugar.LogError("--------------PROXIMITAT---------------");
        this.peces = new Queue<Peça>(peces);
        comprovades = new List<Peça>();
        canviades = new List<Peça>();
        this.enFinalitzar = enFinalitzar;
        Debugar.Log("Process");
        Step();
    }

    void Step()
    {
        if(peces.Count == 0)
        {
            Debugar.LogError("FINALITZAT!");
            enFinalitzar.Invoke(comprovades, canviades);
            return;
        }

        _actual = peces.Dequeue();

        for (int i = 0; i < _actual.Condicions.Length; i++)
        {
            if (_actual.Condicions[i].Comprovar(_actual, this, grups, cami, GunayarExperienciaIVisualitzarSiCal))
            {
                Add(_actual);
                canviades.Add(_actual);
                break;
            }
        }

        if (!comprovades.Contains(_actual)) comprovades.Add(_actual);

        Step();
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


