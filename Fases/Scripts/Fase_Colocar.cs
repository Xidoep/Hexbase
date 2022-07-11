using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    Grid grid;

    public static bool colocarPecesPermes = false;
    public bool debug;

    [SerializeField] Estat pe�aSeleccionada;
    public Estat Pe�aSeleccionada { set => pe�aSeleccionada = value; }

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la pe�a inicial agafantla del pool de peces.
        colocarPecesPermes = true;
        debug = colocarPecesPermes;
    }


    public void CrearPe�a(Vector2Int coordenada)
    {
        grid.CrearPe�a(pe�aSeleccionada, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        colocarPecesPermes = false;
        debug = colocarPecesPermes;
    }
}
