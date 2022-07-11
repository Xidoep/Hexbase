using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    Grid grid;

    public static bool colocarPecesPermes = false;
    public bool debug;

    [SerializeField] Estat peçaSeleccionada;
    public Estat PeçaSeleccionada { set => peçaSeleccionada = value; }

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la peça inicial agafantla del pool de peces.
        colocarPecesPermes = true;
        debug = colocarPecesPermes;
    }


    public void CrearPeça(Vector2Int coordenada)
    {
        grid.CrearPeça(peçaSeleccionada, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        colocarPecesPermes = false;
        debug = colocarPecesPermes;
    }
}
