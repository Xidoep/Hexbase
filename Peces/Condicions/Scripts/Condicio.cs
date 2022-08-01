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

    [Apartat("REFERENCIES (autosetting)")]
    [SerializeField] protected Estat refCami;
    [SerializeField] protected Detall_Tiles_Estats refCaminables;
    [SerializeField] protected Grups refGrups;

    //INTERN
    List<Peça> connectatsACami;



    private void OnEnable() => Setup();
    private void OnValidate() => Setup();

    /// <summary>
    /// Funcio virtual sobreescrite per la resta de condicions, que comprova si es compleix la condicio.
    /// Es cridada desde Proximitat.
    /// 
    /// IMPORTANT: Les funcions subscrites han de cridar Canviar quan el resultat sigui positiu.
    /// </summary>
    public abstract bool Comprovar(Peça peça);


    /// <summary>
    /// Canviar es la funcio comuna que es crida a al confirmar la condicio, a la funcio Comprovar.
    /// </summary>
    protected void Canviar(Peça peça) 
    {
        Debug.LogError($"{peça.Estat.name} Changed to {objectiu.name}");
        peça.CanviarSubestat(objectiu);
    }



    protected List<Peça> Veins(Peça peça) => peça.VeinsPeça;
    protected List<Peça> VeinsAmbCami(Peça peça) => VeinsAmbCami(Veins(peça));
    protected List<Peça> VeinsAmbCami(List<Peça> veins)
    {
        if (connectatsACami == null) connectatsACami = new List<Peça>();
        else connectatsACami.Clear();

        connectatsACami.AddRange(veins);

        Debug.LogError("UTILITZAR CAMI");
        for (int v = 0; v < veins.Count; v++)
        {
            if (veins[v].EstatIgualA(refCami))
            {
                List<Peça> veinsDelCami = refGrups.Veins(veins[v].Grup);
                connectatsACami.AddRange(veinsDelCami);
            }
        }

        return connectatsACami;
    }

    protected virtual void Setup()
    {
        if (refCami == null) refCami = XS_Editor.LoadAssetAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats/CAMI.asset");
        if (refCaminables == null) refCaminables = XS_Editor.LoadAssetAtPath<Detall_Tiles_Estats>("Assets/XidoStudio/Hexbase/Peces/Detalls/Tiles_CAMINABLES.asset");
        if (refGrups == null) refGrups = XS_Editor.LoadAssetAtPath<Grups>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Grups.asset");
        if (objectiu == null) Debugar.LogError($"La condicio {this.name} no the Objectiu!");
    }
}
