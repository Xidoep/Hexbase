using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permet colocar peces al grid. Iniciat per Fase_Iniciar, i Fase_Resoldre si cal.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPe�a = false;
    static bool bloquejat = false;

    [SerializeField] Estat seleccionada;
    

    Grid grid;


    //PROPERTIES
    public static bool PermesColoarPe�a => permesColoarPe�a;
    public static bool Bloquejat => bloquejat;
    public Estat Seleccionada => seleccionada;



    public override void Inicialitzar()
    {
        if (grid == null) 
            grid = FindObjectOfType<Grid>();


        //Prepara la pe�a inicial agafantla del pool de peces.
        permesColoarPe�a = true;
        OnFinish += BloqujarColocacio;
        //poolPeces.Get
    }
    void BloqujarColocacio()
    {
        permesColoarPe�a = false;
        OnFinish -= BloqujarColocacio;
    }

    public void Seleccionar(Estat estat) 
    {
        Debug.LogError($"SELECT {estat.name}");
        seleccionada = estat;
    } 
    public void CrearPe�a(Vector2Int coordenada)
    {
        grid.CrearPe�a(seleccionada, coordenada);
    }

    public static void Bloquejar() => Bloquejar(true);
    public static void Desbloquejar() => Bloquejar(false);
    public static void Bloquejar(bool bloquejar) => bloquejat = bloquejar;
}
