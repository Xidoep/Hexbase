using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/SaveRefencies")]
public class Referencies : ScriptableObject
{
    public static Referencies Instance;

    [SerializeScriptableObject] [SerializeField] SaveHex save;
    [SerializeScriptableObject] [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeScriptableObject] [SerializeField] Nivell nivell;
    [SerializeScriptableObject] [SerializeField] Fase_Resoldre resoldre;

    [Linia]
    [SerializeField] EstatColocable[] estats;
    [SerializeField] Subestat[] subestats;
    [SerializeField] Producte[] productes;
    [SerializeField] Tile[] tiles;

    public EstatColocable[] Estats => estats;
    public Subestat[] Subestats => subestats;
    public Producte[] Productes => productes;
    public Tile[] Tiles => tiles;


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

    private void OnValidate()
    {
        estats = XS_Editor.LoadAllAssetsAtPath<EstatColocable>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        subestats = XS_Editor.LoadAllAssetsAtPath<Subestat>("Assets/XidoStudio/Hexbase/Peces/Subestats").ToArray();
        productes = XS_Editor.LoadAllAssetsAtPath<Producte>("Assets/XidoStudio/Hexbase/Peces/Productes").ToArray();
        tiles = XS_Editor.LoadAllAssetsAtPathAndSubFolders<Tile>("Assets/XidoStudio/Hexbase/Peces/Tiles/Tiles").ToArray();
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");
    }
}
