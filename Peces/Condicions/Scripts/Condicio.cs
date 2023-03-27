using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

/// <summary>
/// La base de les condicions.
/// Cada un d'aquesta te 
/// </summary>
public abstract class Condicio : ScriptableObject
{
    [Tooltip("L'subestat al que canviar� quan es compleixi la condici�.")] [SerializeField] protected Subestat objectiu;


    [Apartat("OPCIONS")]
    [SerializeField] protected bool cami;
    [SerializeField] protected bool grup;

    [Apartat("OPCIONS")]
    [SerializeField] protected int punts;
    // [Apartat("REFERENCIES (autosetting)")]
    // [SerializeField] protected Estat refCami;
    // [SerializeField] protected Grups refGrups;

    //INTERN
    List<Pe�a> veins;
    List<Pe�a> connectatsACami;


    //public Subestat Objectiu => objectiu;

    /// <summary>
    /// Funcio virtual sobreescrite per la resta de condicions, que comprova si es compleix la condicio.
    /// Es cridada desde Proximitat.
    /// 
    /// IMPORTANT: Les funcions subscrites han de cridar Canviar quan el resultat sigui positiu.
    /// </summary>
    public abstract bool Comprovar(Pe�a pe�a, Grups grups, Estat cami, bool canviar, System.Action<Pe�a, bool, int> enConfirmar, System.Action<Pe�a, int> enCanviar);


    /// <summary>
    /// Canviar es la funcio comuna que es crida a al confirmar la condicio, a la funcio Comprovar.
    /// </summary>
    public void Canviar(Pe�a pe�a, System.Action<Pe�a, int> enCanviar) 
    {
        Debugar.LogError($"[{pe�a.SubestatNom}] >>> Changed to >>> [{objectiu.name}]");
        enCanviar?.Invoke(pe�a, punts);
        pe�a.CanviarSubestat(objectiu);
    }


    public List<Pe�a> GetVeinsAcordingToOptions(Pe�a pe�a, Grups _grup, Estat _cami)
    {
        if (veins == null) veins = new List<Pe�a>();
        else veins.Clear();

        if (cami)
        {
            if (grup) veins = _grup.Veins(_grup.Grup, pe�a);
            else veins = VeinsAmbCami(pe�a, _grup,_cami);
        }
        else
        {
            if (grup) veins = _grup.Veins(_grup.Grup, pe�a);
            else veins = Veins(pe�a);
        }
        return veins;
    }
    List<Pe�a> Veins(Pe�a pe�a) => pe�a.VeinsPe�a;
    List<Pe�a> VeinsAmbCami(Pe�a pe�a, Grups _grup, Estat _cami) => VeinsAmbCami(Veins(pe�a), _grup, _cami);
    List<Pe�a> VeinsAmbCami(List<Pe�a> veins, Grups _grup, Estat _cami)
    {
        if (connectatsACami == null) connectatsACami = new List<Pe�a>();
        else connectatsACami.Clear();

        connectatsACami.AddRange(veins);

        //Debug.LogError("UTILITZAR CAMI");
        for (int v = 0; v < veins.Count; v++)
        {
            if (veins[v].EstatIgualA(_cami))
            {
                List<Pe�a> veinsDelCami = _grup.Veins(_grup.Grup, veins[v]);
                connectatsACami.AddRange(veinsDelCami);
            }
        }

        return connectatsACami;
    }

    public void OnValidate()
    {
        if (objectiu == null) Debugar.LogError($"La condicio {this.name} no the Objectiu!");
    }
}
