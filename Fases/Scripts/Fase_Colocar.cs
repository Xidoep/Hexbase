using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPeça = false;

    Grid grid;

    [Linia]
    [Nota("Amb el temps això s'ha de setejar desde fora",NoteType.Error)]
    [SerializeField] Estat peçaSeleccionada;



    public Estat PeçaSeleccionada { set => peçaSeleccionada = value; }
    public static bool PermesColoarPeça => permesColoarPeça;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la peça inicial agafantla del pool de peces.
        permesColoarPeça = true;
    }


    public void CrearPeça(Vector2Int coordenada)
    {
        grid.CrearPeça(peçaSeleccionada, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        permesColoarPeça = false;
    }
}
