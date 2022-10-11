using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPe�a = false;
    static bool bloquejat = false;

    Grid grid;

    [Linia]
    [SerializeField] PoolPeces poolPeces;
    [Linia]
    [Nota("If not null, will take this insead of from pool")]
    [SerializeField] Estat debug;


    public static bool PermesColoarPe�a => permesColoarPe�a;
    public static bool Bloquejat => bloquejat;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la pe�a inicial agafantla del pool de peces.
        permesColoarPe�a = true;

        //poolPeces.Get
    }

    public void Seleccionar(Estat estat) 
    {
        Debug.LogError($"SELECT {estat.name}");
        debug = estat;
    } 
    public void CrearPe�a(Vector2Int coordenada)
    {
        grid.CrearPe�a(debug == null ? poolPeces.Get : debug, coordenada);
    }

    public override void Finalitzar()
    {
        OnFinish_Invocar();

        permesColoarPe�a = false;
    }

    public static void Bloquejar() => Bloquejar(true);
    public static void Desbloquejar() => Bloquejar(false);
    public static void Bloquejar(bool bloquejar) => bloquejat = bloquejar;
}
