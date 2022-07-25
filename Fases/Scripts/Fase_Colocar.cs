using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPe�a = false;

    Grid grid;

    [Linia]
    [Nota("Amb el temps aix� s'ha de setejar desde fora",NoteType.Error)]
    [SerializeField] Estat pe�aSeleccionada;



    public Estat Pe�aSeleccionada { set => pe�aSeleccionada = value; }
    public static bool PermesColoarPe�a => permesColoarPe�a;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la pe�a inicial agafantla del pool de peces.
        permesColoarPe�a = true;
    }


    public void CrearPe�a(Vector2Int coordenada)
    {
        grid.CrearPe�a(pe�aSeleccionada, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        permesColoarPe�a = false;
    }
}
