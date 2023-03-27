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
    [Tooltip("L'subestat al que canviarà quan es compleixi la condiciò.")] [SerializeField] protected Subestat objectiu;


    [Apartat("OPCIONS")]
    [SerializeField] protected bool cami;
    [SerializeField] protected bool grup;

    [Apartat("OPCIONS")]
    [SerializeField] protected int punts;
    // [Apartat("REFERENCIES (autosetting)")]
    // [SerializeField] protected Estat refCami;
    // [SerializeField] protected Grups refGrups;

    //INTERN
    List<Peça> veins;
    List<Peça> connectatsACami;


    //public Subestat Objectiu => objectiu;

    /// <summary>
    /// Funcio virtual sobreescrite per la resta de condicions, que comprova si es compleix la condicio.
    /// Es cridada desde Proximitat.
    /// 
    /// IMPORTANT: Les funcions subscrites han de cridar Canviar quan el resultat sigui positiu.
    /// </summary>
    public abstract bool Comprovar(Peça peça, Grups grups, Estat cami, bool canviar, System.Action<Peça, bool, int> enConfirmar, System.Action<Peça, int> enCanviar);


    /// <summary>
    /// Canviar es la funcio comuna que es crida a al confirmar la condicio, a la funcio Comprovar.
    /// </summary>
    public void Canviar(Peça peça, System.Action<Peça, int> enCanviar) 
    {
        Debugar.LogError($"[{peça.SubestatNom}] >>> Changed to >>> [{objectiu.name}]");
        enCanviar?.Invoke(peça, punts);
        peça.CanviarSubestat(objectiu);
    }


    public List<Peça> GetVeinsAcordingToOptions(Peça peça, Grups _grup, Estat _cami)
    {
        if (veins == null) veins = new List<Peça>();
        else veins.Clear();

        if (cami)
        {
            if (grup) veins = _grup.Veins(_grup.Grup, peça);
            else veins = VeinsAmbCami(peça, _grup,_cami);
        }
        else
        {
            if (grup) veins = _grup.Veins(_grup.Grup, peça);
            else veins = Veins(peça);
        }
        return veins;
    }
    List<Peça> Veins(Peça peça) => peça.VeinsPeça;
    List<Peça> VeinsAmbCami(Peça peça, Grups _grup, Estat _cami) => VeinsAmbCami(Veins(peça), _grup, _cami);
    List<Peça> VeinsAmbCami(List<Peça> veins, Grups _grup, Estat _cami)
    {
        if (connectatsACami == null) connectatsACami = new List<Peça>();
        else connectatsACami.Clear();

        connectatsACami.AddRange(veins);

        //Debug.LogError("UTILITZAR CAMI");
        for (int v = 0; v < veins.Count; v++)
        {
            if (veins[v].EstatIgualA(_cami))
            {
                List<Peça> veinsDelCami = _grup.Veins(_grup.Grup, veins[v]);
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
