using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

//FALTA:
//-En contes de que inicial sigui una pe�a, crear un sistema/scriptable que crei X peces aleatories pel mapa i 1 o + al centre.

public class Grid : MonoBehaviour
{
    const int GRID_SIZE = 200;

    [Header("Prefasb")]
    [SerializeField] GameObject prefab_Ranura;
    [SerializeField] GameObject prefab_Pe�a;
    [SerializeField] ColeccioTiles coleccio;
    [Space(10)]
    [Header("Peces")]
    [SerializeField] EstatPe�a inicial; //Posar aixo en un scriptable que controli la pe�a que s'ha seleccionat. "Seleccio" o algo aixi
    [SerializeField] EstatPe�a desbloquejadora;
    [Space(10)]
    [SerializeField] EstatPe�a seleccionada;

    Hexagon[,] grid;
    List<Hexagon> gridDebug;


    public ColeccioTiles Coleccio => coleccio;
    public EstatPe�a Seleccionada { set => seleccionada = value; }

    public Hexagon[] Veins(Vector2Int coordenades) => grid.Veins(coordenades);

    private void OnEnable()
    {
        CrearGrid();
        CrearPe�a(inicial, new Vector2Int(GRID_SIZE / 2, GRID_SIZE / 2));
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


    public void CrearPe�aDesbloquejadora(Vector2Int coordenada) => CrearPe�a(desbloquejadora, coordenada);
    public void CrearPe�a(Vector2Int coordenada) => CrearPe�a(seleccionada, coordenada);
    /// <summary>
    /// Crea el prefab pe�a, declarant la pe�a i les coordenades.
    /// </summary>
    void CrearPe�a(EstatPe�a tipus, Vector2Int coordenada)
    {
        Pe�a pe�aFisica = Instanciar(prefab_Pe�a, coordenada.x, coordenada.y).GetComponent<Pe�a>();

        pe�aFisica.Setup(this, coordenada, tipus);
        grid.Set(pe�aFisica);
        pe�aFisica.Iniciar();

        foreach (var coodVei in grid.VeinsCoordenades(pe�aFisica.Coordenades))
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
        if (prefab_Ranura == null) prefab_Ranura = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Pe�a/Prefabs/Ranura.prefab");
        if (prefab_Pe�a == null) prefab_Pe�a = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Pe�a/Prefabs/Pe�a.prefab");
        if (coleccio == null) coleccio = XS_Editor.LoadAssetAtPath<ColeccioTiles>("Assets/XidoStudio/Hexbase/Tiles/Colleccio/_Col�leccio Tiles.asset");
    }

}





