using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/SaveRefencies")]
public class Referencies : ScriptableObject
{
    public static Referencies Instance;

    [InlineEditor][SerializeField] SaveHex save;
    [InlineEditor] [SerializeField] CapturarPantalla capturarPantalla;
    [InlineEditor] [SerializeField] Nivell nivell;
    [InlineEditor] [SerializeField] Fase_Resoldre resoldre;

    [Linia]
    [ReadOnly] [SerializeField] EstatColocable[] colocables;
    [ReadOnly] [SerializeField] Estat[] estats;
    [ReadOnly] [SerializeField] Producte[] productes;
    [ReadOnly] [SerializeField] Tile[] tiles;
    [ReadOnly] [SerializeField] TileSetBase[] tilesets;
    [ReadOnly] [SerializeField] Connexio[] connexions;
    [ReadOnly, SerializeField] GameObject[] detalls;
    [ReadOnly, SerializeField] Recepta[] receptes;
    [ReadOnly, SerializeField] Output_GuanyarExperiencia[] guanyarExperiencies;

    public EstatColocable[] Colocables => colocables;
    public Estat[] Estats => estats;
    public Producte[] Productes => productes;
    public Tile[] Tiles => tiles;
    public TileSetBase[] Tilesets => tilesets;
    public Connexio[] Connexions => connexions;
    public GameObject[] Detalls => detalls;
    public Recepta[] Receptes => receptes;

    //INTERN
    //bool trobat;
    Connexio cTrobada;
    Estat eTrobat;
    Producte pTrobat;
    GameObject dTrobat;
    Output_GuanyarExperiencia gTrobat;

    public Connexio GetConnexio(string nom)
    {
        cTrobada = null;
        for (int i = 0; i < connexions.Length; i++)
        {
            if (connexions[i].name.Equals(nom))
            {
                cTrobada = connexions[i];
                break;
            }
        }
        if(cTrobada == null)
        {
            Debug.LogError($"La connexio amb el nom {nom} no existeix");
        }
        return cTrobada;
    }
    public Connexio GetConnexioNew(string nom)
    {
        cTrobada = null;
        for (int i = 0; i < connexions.Length; i++)
        {
            if (connexions[i].name.Equals(nom))
            {
                cTrobada = connexions[i];
                break;
            }
        }
        if (cTrobada == null)
        {
            Debug.LogError($"La connexio amb el nom {nom} no existeix");
        }
        return cTrobada;
    }
    public bool EstatsContains(string nom)
    {
        for (int i = 0; i < estats.Length; i++)
        {
            if (estats[i].name.Equals(nom))
                return true;
        }
        return false;
    }
    public Estat GetEstat(string nom)
    {
        eTrobat = null;
        for (int i = 0; i < estats.Length; i++)
        {
            if (estats[i].name.Equals(nom))
            {
                eTrobat = estats[i];
                break;
            }
        }
        if (eTrobat == null)
        {
            Debug.LogError($"l'estat amb el nom {nom} no existeix");
        }
        return eTrobat;
    }
    public bool ProductesContains(string nom)
    {
        for (int i = 0; i < productes.Length; i++)
        {
            if (productes[i].name.Equals(nom))
                return true;
        }
        return false;
    }
    public bool GuanyarExperienciaContains(string nom)
    {
        for (int i = 0; i < guanyarExperiencies.Length; i++)
        {
            if (guanyarExperiencies[i].name.Equals(nom))
                return true;
        }
        return false;
    }


    public Producte GetProducte(string nom)
    {
        pTrobat = null;
        for (int i = 0; i < productes.Length; i++)
        {
            if (productes[i].name.Equals(nom))
            {
                pTrobat = productes[i];
                break;
            }
        }
        if (pTrobat == null)
        {
            Debug.LogError($"el producte amb el nom {nom} no existeix");
        }
        return pTrobat;
    }
    public bool DetallsContains(string nom)
    {
        for (int i = 0; i < detalls.Length; i++)
        {
            if (detalls[i].name.Equals(nom))
                return true;
        }
        return false;
    }
    public GameObject GetDetall(string nom)
    {
        dTrobat = null;
        for (int i = 0; i < detalls.Length; i++)
        {
            if (detalls[i].name.Equals(nom))
            {
                dTrobat = detalls[i];
                break;
            }
        }
        if (dTrobat == null)
        {
            Debug.LogError($"el detall amb el nom {nom} no existeix");
        }
        return dTrobat;
    }
    public Output_GuanyarExperiencia GetGuanyarExperiencia(string nom)
    {
        gTrobat = null;
        for (int i = 0; i < guanyarExperiencies.Length; i++)
        {
            if (guanyarExperiencies[i].name.Equals(nom))
            {
                gTrobat = guanyarExperiencies[i];
                break;
            }
        }
        if (gTrobat == null)
        {
            Debug.LogError($"el guanyarExperiencies amb el nom {nom} no existeix");
        }
        return gTrobat;
    }


    private void OnEnable()
    {
        Instance = this; 
        capturarPantalla.OnCapturatRegistrar(save.AddCaptura);
        nivell.EnGuanyarExperiencia += save.GuardarExperiencia;
        nivell.EnPujarNivell += save.guardarNivell;
        resoldre.EnNetejar += save.NouArxiu;
    }
    private void OnDisable()
    {
        capturarPantalla.OnCapturatDesregistrar(save.AddCaptura);
        nivell.EnGuanyarExperiencia -= save.GuardarExperiencia;
        nivell.EnPujarNivell -= save.guardarNivell;
        resoldre.EnNetejar -= save.NouArxiu;
    }

    private void OnValidate() => Refresh();

    [Button(SdfIconType.ReplyFill)]
    public void Refresh()
    {
        //colocables = XS_Editor.LoadAllAssetsAtPath<EstatColocable>("Assets/XidoStudio/Hexbase/Peces/Estats/Colocables").ToArray();
        colocables = XS_Editor.LoadAllAssetsAtPathAndSubFolders<EstatColocable>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        productes = XS_Editor.LoadAllAssetsAtPath<Producte>("Assets/XidoStudio/Hexbase/Peces/Productes").ToArray();
        //tiles = XS_Editor.LoadAllAssetsAtPathAndSubFolders<Tile>("Assets/XidoStudio/Hexbase/Peces/Tiles/Tiles").ToArray();
        tiles = XS_Editor.LoadAllAssetsAtPathAndSubFolders<Tile>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        //tilesets = XS_Editor.LoadAllAssetsAtPath<TileSetBase>("Assets/XidoStudio/Hexbase/Peces/Estats/TileSets").ToArray();
        tilesets = XS_Editor.LoadAllAssetsAtPathAndSubFolders<TileSetBase>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        connexions = XS_Editor.LoadAllAssetsAtPath<Connexio>("Assets/XidoStudio/Hexbase/Peces/Connexio").ToArray();
        //connexions = XS_Editor.LoadAllAssetsAtPath<Connexio>("Assets/XidoStudio/Hexbase/Peces/Connexio/new").ToArray();
        detalls = XS_Editor.LoadAllAssetsAtPath<GameObject>("Assets/XidoStudio/Hexbase/Peces/Detalls").ToArray();
        receptes = XS_Editor.LoadAllAssetsAtPath<Recepta>("Assets/XidoStudio/Hexbase/Peces/Receptes").ToArray();
        guanyarExperiencies = XS_Editor.LoadAllAssetsAtPath<Output_GuanyarExperiencia>("Assets/XidoStudio/Hexbase/Sistemes/Processador/GunayarExperiencia").ToArray();
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");

    }
}
