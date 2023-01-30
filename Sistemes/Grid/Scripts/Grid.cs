using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XS_Utils;

//FALTA:
//-En contes de que inicial sigui una pe�a, crear un sistema/scriptable que crei X peces aleatories pel mapa i 1 o + al centre.

public class Grid : MonoBehaviour
{
    //***************************************************
    //Potser aixo tamb� es podria convertir en un Scriptable. Aix� l'escena queda neta.
    //***************************************************
    [SerializeField] GridLayout gridLayout;

    const int GRID_SIZE = 200;

    [SerializeField] Fase inicial;
    [SerializeField] Fase processar;
    [SerializeField] Grups grups;
    [SerializeField] Produccio produccio;
    [SerializeField] SaveHex save;
    [SerializeField] CamaraGestio camaraGestio;

    [Apartat("PREFABS")]
    [SerializeField] GameObject prefab_Ranura;
    [SerializeField] GameObject prefab_Pe�a;
    [SerializeField] GameObject prefab_Boto;
    [SerializeField] GameObject prefab_Simulacio;

    [Apartat("ESTATS")]
    [SerializeField] Estat[] estats;

    [Apartat("SUBESTATS")]
    [SerializeField] Subestat[] subestats;

    //[Linia]
    //[Header("PECES")]
    //[Nota("Aix� haur� d'anar en una Fase 'Inicial', que s'encarregar� de crear el mon abans de tu comen�ar a jugar.", NoteType.Error)]
    //[SerializeField] Estat inicial; //Posar aixo en un scriptable que controli la pe�a que s'ha seleccionat. "Seleccio" o algo aixi

    Hexagon[,] grid;
    Pe�a simulada;
    Hexagon ranuraSimulada;

    [Apartat("DIMENCIONS")]
    [SerializeField] Vector2Int nord = Vector2Int.one * 100;
    [SerializeField] Vector2Int sud = Vector2Int.one * 100;
    [SerializeField] Vector2Int est = Vector2Int.one * 100;
    [SerializeField] Vector2Int oest = Vector2Int.one * 100;

    [SerializeField] List<XS_GPU.Grafic> grafics;

    //INTERN
    Estat eTrobat;
    Subestat sTrobat;

    [ContextMenu("Prova")]
    void ProvaGridLayout()
    {
        for (int x = -2; x < 5; x++)
        {
            for (int y = -2; y < 5; y++)
            {
                Instantiate(prefab_Boto, gridLayout.CellToLocal(new Vector3Int(x, y, 0)), Quaternion.identity);
            }
        }
    }

    public Estat Estat(string nom) 
    {
        eTrobat = null;
        for (int i = 0; i < estats.Length; i++)
        {
            if(estats[i].name == nom)
            {
                eTrobat = estats[i];
                break;
            }
        }
        return eTrobat;
        //return (Estat)estats.Where<Estat>(x => x.name == nom) as Estat;
    }
    public Subestat Subestat(string nom) 
    {
        sTrobat = null;
        for (int i = 0; i < subestats.Length; i++)
        {
            if (subestats[i].name == nom)
            {
                sTrobat = subestats[i];
                break;
            }
        }
        return sTrobat;
        //return (Subestat)estats.Where<Estat>(x => x.name == nom);
    } 

    public void Start()
    {
        inicial.Iniciar();
    }

    public void Dimensionar(Pe�a pe�a)
    {
        if (pe�a.Coordenades.y < nord.y) nord = pe�a.Coordenades;
        if (pe�a.Coordenades.y > sud.y) sud = pe�a.Coordenades;
        if (pe�a.Coordenades.x > est.x) est = pe�a.Coordenades;
        if (pe�a.Coordenades.x < oest.x) oest = pe�a.Coordenades;

        camaraGestio.SetDimensions(
            grid[nord.x, nord.y],
            grid[sud.x, sud.y],
            grid[est.x, est.y],
            grid[oest.x, oest.y]);
    }

    private void LateUpdate()
    {
        XS_GPU.Render();
        grafics = XS_GPU.grafics;
    }

    //List<Hexagon> gridDebug;

    public GameObject Prefab_Pe�a => prefab_Pe�a;
    public List<Hexagon> Veins(Vector2Int coordenades) => grid.Veins(coordenades);
    public List<Pe�a> VeinsPe�a(Vector2Int coordenades) => grid.VeinsPe�a(coordenades);
    public List<Vector2Int> VeinsCoordenades(Vector2Int coordenades) => grid.VeinsCoordenades(coordenades);
    public Vector2Int Centre => new Vector2Int(GRID_SIZE / 2, GRID_SIZE / 2);
    public void Set(Pe�a pe�a) => grid.Set(pe�a);
    public Hexagon Get(Vector2Int coordenada) => grid.Get(coordenada);
    public Hexagon Get(int x, int y) => grid.Get(x, y);
    public void Netejar(Vector2Int coordenada) => grid.Set(null, coordenada);
    public bool EstaBuida(Vector2Int coordenada) => grid.EstaBuida(coordenada);
    /*private void OnEnable()
    {
        CrearGrid();
        CrearPe�a(inicial, new Vector2Int(GRID_SIZE / 2, GRID_SIZE / 2));
    }*/

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

    public void CrearDesdeRanura(Estat tipus, Vector2Int coordenada)
    {
        processar.Iniciar(coordenada);
    }

    /// <summary>
    /// Crea el prefab pe�a, declarant la pe�a i les coordenades.
    /// </summary>
    public void CrearPe�a(Estat tipus, Vector2Int coordenada, bool processar = true)
    {
        if (simulada != null) 
        {
            grid.Set(null, coordenada.x, coordenada.y);
            Destroy(simulada.gameObject);
        } 

        Pe�a pe�aFisica = Instanciar(prefab_Pe�a, coordenada.x, coordenada.y).GetComponent<Pe�a>();

        pe�aFisica.Setup(this, coordenada, tipus, tipus.SubestatInicial);
        grid.Set(pe�aFisica);
        //pe�aFisica.Iniciar();

        foreach (var coodVei in grid.VeinsCoordenades(pe�aFisica.Coordenades))
        {
            CrearRanura(coodVei);
        }

        if(processar)
            this.processar.Iniciar(pe�aFisica);

        //save.Add(pe�aFisica, grups);
    }


    public Pe�a SimularInici(Estat estat, Vector2Int coordenada)
    {
        ranuraSimulada = grid.Get(coordenada);

        simulada = new GameObject("simulada").AddComponent<Pe�a>();
        //simulada = Instanciar(prefab_Simulacio, coordenada.x, coordenada.y).GetComponent<Pe�a>();
        simulada.Setup(this, coordenada, estat, estat.SubestatInicial);
        grid.Set(simulada);
        return simulada;
    }
    public void SimularFinal(Pe�a pe�a)
    {
        Vector2Int coordenada = pe�a.Coordenades;
        if(pe�a != null)
            Destroy(pe�a.gameObject);

        if(ranuraSimulada != null)
            grid.Set(ranuraSimulada, coordenada);

        //Debugar.LogError("Borrar Simulacio");
    }

    public void CrearRanura(Vector2Int coordenada)
    {
        if (coordenada.IsOutOfRange(GRID_SIZE))
            return;

        if (grid.Get(coordenada.x,coordenada.y) != null)
            return;

        Ranura ranura = Instanciar(prefab_Ranura, coordenada.x, coordenada.y).GetComponent<Ranura>();
        ranura.Setup(this, coordenada, null, null);
        grid.Set(ranura);

    }
    public Hexagon CrearBoto(Vector2Int coordenada, GameObject boto)
    {
        Hexagon hex = Instanciar(boto, coordenada.x, coordenada.y).GetComponent<Hexagon>();
        hex.Setup(this, coordenada, null, null);
        grid.Set(hex);
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

        //camaraGestio.ResetDimensions(transform);
    }

    public GameObject Instanciar(GameObject prefab, Vector2Int coordenada) => Instanciar(prefab, coordenada.x, coordenada.y);
    GameObject Instanciar(GameObject prefab, int x, int y)
    {
        GameObject tmp = Instantiate(prefab, transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(x, y);
        return tmp;
    }

    private void OnValidate()
    {
        //if (prefab_Ranura == null) prefab_Ranura = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Pe�a/Prefabs/Ranura.prefab");
        //if (prefab_Pe�a == null) prefab_Pe�a = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Pe�a/Prefabs/Pe�a.prefab");

        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        subestats = XS_Editor.LoadAllAssetsAtPath<Subestat>("Assets/XidoStudio/Hexbase/Peces/Subestats").ToArray();
    }

}





