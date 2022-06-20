using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

//FALTA:
//-En contes de que inicial sigui una peça, crear un sistema/scriptable que crei X peces aleatories pel mapa i 1 o + al centre.

public class Grid : MonoBehaviour
{
    const int GRID_SIZE = 200;

    [Header("Prefasb")]
    [SerializeField] GameObject prefab_Ranura;
    [SerializeField] GameObject prefab_Peça;
    [SerializeField] ColeccioTiles coleccio;
    [Space(10)]
    [Header("Peces")]
    [SerializeField] EstatPeça inicial; //Posar aixo en un scriptable que controli la peça que s'ha seleccionat. "Seleccio" o algo aixi
    [SerializeField] EstatPeça desbloquejadora;
    [Space(10)]
    [SerializeField] EstatPeça seleccionada;

    Hexagon[,] grid;
    List<Hexagon> gridDebug;


    public ColeccioTiles Coleccio => coleccio;
    public EstatPeça Seleccionada { set => seleccionada = value; }

    public Hexagon[] Veins(Vector2Int coordenades) => grid.Veins(coordenades);

    private void OnEnable()
    {
        CrearGrid();
        CrearPeça(inicial, new Vector2Int(GRID_SIZE / 2, GRID_SIZE / 2));
    }

    void CrearGrid()
    {
        //Posiciona el grid perque la pece central sigui al centre del mon.
        transform.position = -GridExtensions.GetWorldPosition(GRID_SIZE / 2, GRID_SIZE / 2);
        #region DEBUG
        foreach (var item in GetComponentsInChildren<Hexagon>())
        {
            DestroyImmediate(item.gameObject);
        }
        #endregion
        grid = new Hexagon[GRID_SIZE, GRID_SIZE];
    }


    public void CrearPeçaDesbloquejadora(Vector2Int coordenada) => CrearPeça(desbloquejadora, coordenada);
    public void CrearPeça(Vector2Int coordenada) => CrearPeça(seleccionada, coordenada);
    /// <summary>
    /// Crea el prefab peça, declarant la peça i les coordenades.
    /// </summary>
    void CrearPeça(EstatPeça tipus, Vector2Int coordenada)
    {
        Peça peçaFisica = Instanciar(prefab_Peça, coordenada.x, coordenada.y).GetComponent<Peça>();

        peçaFisica.Setup(this, coordenada, tipus);
        grid.Set(peçaFisica);
        peçaFisica.Iniciar();

        foreach (var coodVei in grid.VeinsCoordenades(peçaFisica.Coordenades))
        {
            CrearRanura(coodVei);
        }

        #region DEBUG
        if (grid != null)
        {
            gridDebug = new List<Hexagon>();
            for (int _x = 0; _x < grid.GetLength(0); _x++)
            {
                for (int _y = 0; _y < grid.GetLength(1); _y++)
                {
                    gridDebug.Add(grid[_y, _x]);
                }
            }
        }
        #endregion
    }
    void CrearRanura(Vector2Int coordenada)
    {
        if (coordenada.IsOutOfRange(GRID_SIZE))
            return;

        if (!grid.Buida(coordenada))
            return;

        //GameObject tile = Instantiate(prefab_Ranura, transform);
        //tile.transform.localPosition = GridExtensions.GetWorldPosition(coordenada.x, coordenada.y);
        
        Ranura ranura = Instanciar(prefab_Ranura, coordenada.x, coordenada.y).GetComponent<Ranura>();
        //Ranura ranura = tile.GetComponent<Ranura>();
        ranura.Setup(this, coordenada, null);
        grid.Set(ranura);
        ranura.Iniciar();

    }


    GameObject Instanciar(GameObject prefab, int x, int y)
    {
        GameObject tmp = Instantiate(prefab, transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(x, y);
        return tmp;
    }

    private void OnValidate()
    {
        if (prefab_Ranura == null) prefab_Ranura = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Peça/Prefabs/Ranura.prefab");
        if (prefab_Peça == null) prefab_Peça = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Peça/Prefabs/Peça.prefab");
        if (coleccio == null) coleccio = XS_Editor.LoadAssetAtPath<ColeccioTiles>("Assets/XidoStudio/Hexbase/Tiles/Colleccio/_Col·leccio Tiles.asset");
    }

}





