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

    public EstatColocable[] Colocables => colocables;
    public Estat[] Estats => estats;
    public Producte[] Productes => productes;
    public Tile[] Tiles => tiles;
    public TileSetBase[] Tilesets => tilesets;
    public Connexio[] Connexions => connexions;


    //INTERN
    //bool trobat;
    Connexio cTrobada;
    Estat eTrobat;
    Producte pTrobat;



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

    private void OnValidate() => Refrex();

    public void Refrex()
    {
        colocables = XS_Editor.LoadAllAssetsAtPath<EstatColocable>("Assets/XidoStudio/Hexbase/Peces/Estats/Colocables").ToArray();
        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        productes = XS_Editor.LoadAllAssetsAtPath<Producte>("Assets/XidoStudio/Hexbase/Peces/Productes").ToArray();
        tiles = XS_Editor.LoadAllAssetsAtPathAndSubFolders<Tile>("Assets/XidoStudio/Hexbase/Peces/Tiles/Tiles").ToArray();
        tilesets = XS_Editor.LoadAllAssetsAtPath<TileSetBase>("Assets/XidoStudio/Hexbase/Peces/Estats/TileSets").ToArray();
        connexions = XS_Editor.LoadAllAssetsAtPath<Connexio>("Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats").ToArray();
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");

    }
}
