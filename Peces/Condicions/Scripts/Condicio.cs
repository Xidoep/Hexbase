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

   // [Apartat("REFERENCIES (autosetting)")]
   // [SerializeField] protected Estat refCami;
   // [SerializeField] protected Grups refGrups;

    //INTERN
    List<Peça> veins;
    List<Peça> connectatsACami;



    private void OnEnable() => Setup();
    private void OnValidate() => Setup();

    /// <summary>
    /// Funcio virtual sobreescrite per la resta de condicions, que comprova si es compleix la condicio.
    /// Es cridada desde Proximitat.
    /// 
    /// IMPORTANT: Les funcions subscrites han de cridar Canviar quan el resultat sigui positiu.
    /// </summary>
    public abstract bool Comprovar(Peça peça, Proximitat proximitat, Grups grups, Estat cami);


    /// <summary>
    /// Canviar es la funcio comuna que es crida a al confirmar la condicio, a la funcio Comprovar.
    /// </summary>
    protected void Canviar(Peça peça) 
    {
        Debugar.LogError($"[{peça.Subestat.name}] >>> Changed to >>> [{objectiu.name}]");
        peça.CanviarSubestat(objectiu);
    }


    protected List<Peça> GetVeinsAcordingToOptions(Peça peça, Grups _grup, Estat _cami)
    {
        if (veins == null) veins = new List<Peça>();
        else veins.Clear();

        if (cami)
        {
            if (grup) veins = _grup.Veins(peça);
            else veins = VeinsAmbCami(peça,_grup,_cami);
        }
        else
        {
            if (grup) veins = _grup.Veins(peça);
            else veins = Veins(peça);
        }
        return veins;
    }
    protected List<Peça> Veins(Peça peça) => peça.VeinsPeça;
    protected List<Peça> VeinsAmbCami(Peça peça, Grups _grup, Estat _cami) => VeinsAmbCami(Veins(peça), _grup, _cami);
    protected List<Peça> VeinsAmbCami(List<Peça> veins, Grups _grup, Estat _cami)
    {
        if (connectatsACami == null) connectatsACami = new List<Peça>();
        else connectatsACami.Clear();

        connectatsACami.AddRange(veins);

        Debug.LogError("UTILITZAR CAMI");
        for (int v = 0; v < veins.Count; v++)
        {
            if (veins[v].EstatIgualA(_cami))
            {
                List<Peça> veinsDelCami = _grup.Veins(veins[v]);
                connectatsACami.AddRange(veinsDelCami);
            }
        }

        return connectatsACami;
    }

    protected virtual void Setup()
    {
      //  if (refCami == null) refCami = XS_Editor.LoadAssetAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats/CAMI.asset");
      //  if (refGrups == null) refGrups = XS_Editor.LoadAssetAtPath<Grups>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Grups.asset");
        if (objectiu == null) Debugar.LogError($"La condicio {this.name} no the Objectiu!");
    }
}
