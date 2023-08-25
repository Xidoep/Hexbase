using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XS_Utils;

//FALTA:
//-En contes de que inicial sigui una peça, crear un sistema/scriptable que crei X peces aleatories pel mapa i 1 o + al centre.

[DefaultExecutionOrder(-5)]
public class Grid : MonoBehaviour
{
    public static Grid Instance;
    private void OnEnable()
    {
        Instance = this;
        UnityEngine.Random.InitState(0);
    }

    const int GRID_SIZE = 200;

    [SerializeScriptableObject][SerializeField] Fase inicial;
    [SerializeScriptableObject][SerializeField] Fase processar;
    [SerializeScriptableObject][SerializeField] Grups grups;
    [SerializeScriptableObject][SerializeField] Produccio produccio;

    [Apartat("PREFABS")]
    [SerializeField] GameObject prefab_Ranura;
    [SerializeField] GameObject prefab_Peça;

    Hexagon[,] grid;
    Peça simulada;
    Hexagon ranuraSimulada;

    Vector2Int nord = Vector2Int.one * 100;
    Vector2Int sud = Vector2Int.one * 100;
    Vector2Int est = Vector2Int.one * 100;
    Vector2Int oest = Vector2Int.one * 100;

    Action<Hexagon, Hexagon, Hexagon, Hexagon> enDimensionar;
    public Action<Hexagon, Hexagon, Hexagon, Hexagon> SetEnDimensionar { set => enDimensionar = value; }



    public void Start()
    {
        inicial.Iniciar();
    }

    public void Dimensionar(Hexagon hexagon)
    {
        if (hexagon.Coordenades.y < nord.y) nord = hexagon.Coordenades;
        if (hexagon.Coordenades.y > sud.y) sud = hexagon.Coordenades;
        if (hexagon.Coordenades.x > est.x) est = hexagon.Coordenades;
        if (hexagon.Coordenades.x < oest.x) oest = hexagon.Coordenades;

        enDimensionar.Invoke(
            grid[nord.x, nord.y],
            grid[sud.x, sud.y],
            grid[est.x, est.y],
            grid[oest.x, oest.y]);
    }

    public GameObject Prefab_Peça => prefab_Peça;
    public List<Hexagon> Veins(Vector2Int coordenades) => grid.Veins(coordenades);
    public List<Peça> VeinsPeça(Vector2Int coordenades) => grid.VeinsPeça(coordenades);
    public List<Vector2Int> VeinsCoordenades(Vector2Int coordenades) => grid.VeinsCoordenades(coordenades);
    public Vector2Int Centre => new Vector2Int(GRID_SIZE / 2, GRID_SIZE / 2);
    public void Set(Hexagon hexagon) 
    {
        grid.Set(hexagon);
        Dimensionar(hexagon);
    }
    public Hexagon Get(Vector2Int coordenada) => grid.Get(coordenada);
    public Hexagon Get(int x, int y) => grid.Get(x, y);
    public void Netejar(Vector2Int coordenada) => grid.Set(null, coordenada);
    public bool EstaBuida(Vector2Int coordenada) => grid.EstaBuida(coordenada);


    public void CrearGrid()
    {
        if (grid != null)
            return;

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

    public void CrearDesdeRanura(EstatColocable tipus, Vector2Int coordenada)
    {
        processar.Iniciar(coordenada);
    }

    /// <summary>
    /// Crea el prefab peça, declarant la peça i les coordenades.
    /// </summary>
    public void CrearPeça(EstatColocable tipus, Vector2Int coordenada, bool processar = true)
    {
        if (simulada != null) 
        {
            grid.Set(null, coordenada.x, coordenada.y);
            Destroy(simulada.gameObject);
        } 

        Peça peçaFisica = Instanciar(prefab_Peça, coordenada.x, coordenada.y).GetComponent<Peça>();

        peçaFisica.Setup(this, coordenada, tipus, tipus.Estat);
        Set(peçaFisica);

        /*foreach (var coodVei in grid.VeinsCoordenades(peçaFisica.Coordenades))
        {
            CrearRanura(coodVei);
        }*/
        
        if(processar)
            this.processar.Iniciar(peçaFisica);

    }


    public Peça SimularInici(EstatColocable estat, Vector2Int coordenada)
    {
        ranuraSimulada = grid.Get(coordenada);
        simulada = Instanciar(estat.Prefab.gameObject, coordenada).AddComponent<Peça>();
        //simulada = Instanciar(estat.Prefag, coordenada).AddComponent<Peça>();
        simulada.Setup(this, coordenada, estat, estat.Estat);
        grid.Set(simulada); //Es salta el Dimensionament
        return simulada;
    }
    public void SimularFinal(Peça peça)
    {
        Vector2Int coordenada = peça.Coordenades;

        if(ranuraSimulada != null)
            grid.Set(ranuraSimulada, coordenada);

    }

    public void CrearRanura(Vector2Int coordenada)
    {
        if (coordenada.IsOutOfRange(GRID_SIZE))
            return;

        if (grid.Get(coordenada.x,coordenada.y) != null)
            return;

        Ranura ranura = Instanciar(prefab_Ranura, coordenada.x, coordenada.y).GetComponent<Ranura>();
        ranura.Setup(this, coordenada, null, null);
        Set(ranura);
    }

    public Hexagon CrearBoto(Vector2Int coordenada, GameObject boto)
    {
        Hexagon hex = Instanciar(boto, coordenada.x, coordenada.y).GetComponent<Hexagon>();
        hex.Setup(this, coordenada, null, null);
        grid.Set(hex); //Es salta el Dimensionament
        return hex;
    }


    [ContextMenu("Resetejar")]
    public void Resetejar()
    {
        grups.Resetejar();
        produccio.Resetejar();

        Debugar.Log("Resetejar");
        Vector2Int coordenada = new Vector2Int(0,0);
        for (var x = 0; x < grid.GetLength(0); x++)
        {
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                coordenada = new Vector2Int(x, y);
                if(grid[x,y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                }
                if (!grid.EstaBuida(coordenada))
                {
                    grid.Set(null, coordenada.x, coordenada.y);
                }
            }
        }

        nord = Vector2Int.one * 100;
        sud = Vector2Int.one * 100;
        est = Vector2Int.one * 100;
        oest = Vector2Int.one * 100;
    }

    public GameObject Instanciar(GameObject prefab, Vector2Int coordenada) => Instanciar(prefab, coordenada.x, coordenada.y);
    GameObject Instanciar(GameObject prefab, int x, int y)
    {
        GameObject tmp = Instantiate(prefab, transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(x, y);
        return tmp;
    }

}





