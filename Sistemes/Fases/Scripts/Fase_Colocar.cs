using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permet colocar peces al grid. Iniciat per Fase_Iniciar, i Fase_Resoldre si cal.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPeça = false;
    static bool bloquejat = false;

    [SerializeField] Estat seleccionada;
    [SerializeField] Estat perDefecte;



    //PROPERTIES
    public static bool PermesColoarPeça => permesColoarPeça;
    public static bool Bloquejat => bloquejat;
    public Estat Seleccionada => seleccionada;



    public override void FaseStart()
    {
        if (seleccionada == null)
            seleccionada = perDefecte;
        //Prepara la peça inicial agafantla del pool de peces.
        permesColoarPeça = true;
        OnFinish += BloqujarColocacio;
        //poolPeces.Get
    }
    void BloqujarColocacio()
    {
        permesColoarPeça = false;
        OnFinish -= BloqujarColocacio;
    }

    public void Seleccionar(Estat estat) 
    {
        Debug.LogError($"SELECT {estat.name}");
        seleccionada = estat;
    } 
    public void CrearPeça(Vector2Int coordenada)
    {
        Grid.Instance.CrearPeça(seleccionada, coordenada);
    }

    public static void Bloquejar() => Bloquejar(true);
    public static void Desbloquejar() => Bloquejar(false);
    public static void Bloquejar(bool bloquejar) => bloquejat = bloquejar;
}
