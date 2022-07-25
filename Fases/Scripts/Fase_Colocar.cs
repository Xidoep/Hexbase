using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Colocar")]
public class Fase_Colocar : Fase
{
    static bool permesColoarPeša = false;

    Grid grid;

    [Linia]
    [Nota("Amb el temps aix˛ s'ha de setejar desde fora",NoteType.Error)]
    [SerializeField] Estat pešaSeleccionada;



    public Estat PešaSeleccionada { set => pešaSeleccionada = value; }
    public static bool PermesColoarPeša => permesColoarPeša;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        //Prepara la peša inicial agafantla del pool de peces.
        permesColoarPeša = true;
    }


    public void CrearPeša(Vector2Int coordenada)
    {
        grid.CrearPeša(pešaSeleccionada, coordenada);
    }

    public override void Finalitzar()
    {
        onFinish?.Invoke();

        permesColoarPeša = false;
    }
}
