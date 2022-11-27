using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using XS_Utils;
#endif

[CreateAssetMenu(menuName = "Xido Studio/Hex/Tiles/Tile")]
public class Tile : ScriptableObject
{
    public void Setup(Object prefab, Posicions[] posicions)
    {
        this.prefab = (GameObject)prefab;
        this.posicions = posicions;
    }

    [SerializeField] GameObject prefab;
    [Linia]
    [Header("Connexions")]
    [SerializeField] Connexio exterior;
    [SerializeField] Connexio esquerra;
    [SerializeField] Connexio dreta;

    [SerializeField] Posicions[] posicions;
    //[Header("Punta")]
    //[SerializeField] Estat punta;

    public bool ConnexionsIguals => exterior == esquerra && esquerra == dreta;

    //public Estat Punta => punta;
    public GameObject Prefab => prefab;



    public Connexio Exterior(int orientacioFisica)
    {
        //return Abaix;
        switch (orientacioFisica)
        {
            default:
                return exterior;
            case 1:
                return dreta;
            case 2:
                return esquerra;
        }
    }
    public Connexio Esquerra(int orientacioFisica)
    {
        //return Esquerra;
        switch (orientacioFisica)
        {
            default:
                return dreta;
            case 1:
                return esquerra;
            case 2:
                return exterior;
        }
    }
    public Connexio Dreta(int orientacioFisica)
    {
        //return Dreta;
        switch (orientacioFisica)
        {
            default:
                return esquerra;
            case 1:
                return exterior;
            case 2:
                return dreta;
        }
    }
    //public bool ConnexionsIgualsA(Tile altre) => exterior == altre.exterior && esquerra == altre.esquerra && dreta == altre.dreta;


    [System.Serializable] 
    public struct Posicions
    {
        public Posicions(int orientacio, Vector2 posicio)
        {
            this.orientacio = orientacio;
            this.posicio = posicio;
        }
        public int orientacio;
        public Vector2 posicio;


    }



    //public virtual bool Comprovar(TilePotencial tile, int orientacioFisica, Connexio exterior, Connexio esquerra, Connexio dreta) => true;
}

#if UNITY_EDITOR
public class TileCreator
{
    [MenuItem("Assets/XidoStudio/New Tile")]
    static void CrearTileFromPrefab()
    {
        //Checking that you don't select an asset.
        if (Selection.activeObject == null)
        {
            Debugar.LogError("Select a Prefab to create a tile");
            return;
        }

        Object[] selection = new Object[Selection.objects.Length];
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            selection[i] = CrearTileFrom(Selection.objects[i]);
        }

        EditorUtility.FocusProjectWindow();
        Selection.objects = selection;
    }

    static Object CrearTileFrom(Object prefab)
    {
        
        

        //Get the folder of the actual selected object.
        string path = AssetDatabase.GetAssetPath(prefab);
        string name = prefab.name;
        string[] folder = path.Split(name);

        //Create Scriptable and sets it.
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.Setup(prefab,
            new Tile.Posicions[]
            {
                new Tile.Posicions(  0, new Vector2(    0,       0)),
                new Tile.Posicions(120, new Vector2(-0.5f,  0.866f)),
                new Tile.Posicions(240, new Vector2( 0.5f, -0.866f))
            });

        //Create an asset on the project and save it.
        AssetDatabase.CreateAsset(tile, $"{folder[0]}{name}.asset");
        AssetDatabase.SaveAssets();

        return tile;
    }
}
#endif