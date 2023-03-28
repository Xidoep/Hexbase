using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/SaveRefencies")]
public class SaveReferencies : ScriptableObject
{
    public static SaveReferencies Instance;

    [SerializeField] SaveHex save;
    [SerializeField] public CapturarPantalla capturarPantalla;
    [SerializeField] public Visualitzacions visualitzacions;

    [Apartat("ESTATS")]
    [SerializeField] public Estat[] estats;
    [Header("SUBESTATS")]
    [SerializeField] public Subestat[] subestats;
    [Header("PRODUCTES")]
    [SerializeField] public Producte[] productes;
    [Header("TILES")]
    [SerializeField] public Tile[] tiles;

    private void OnEnable()
    {
        Instance = this; 
        capturarPantalla.OnCapturatRegistrar(save.AddCaptura);
    }
    private void OnDisable()
    {
        capturarPantalla.OnCapturatDesregistrar(save.AddCaptura);
    }

    private void OnValidate()
    {
        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        subestats = XS_Editor.LoadAllAssetsAtPath<Subestat>("Assets/XidoStudio/Hexbase/Peces/Subestats").ToArray();
        productes = XS_Editor.LoadAllAssetsAtPath<Producte>("Assets/XidoStudio/Hexbase/Peces/Productes").ToArray();
        tiles = XS_Editor.LoadAllAssetsAtPathAndSubFolders<Tile>("Assets/XidoStudio/Hexbase/Peces/Tiles/Tiles").ToArray();
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");
        if (visualitzacions == null) visualitzacions = XS_Editor.LoadAssetAtPath<Visualitzacions>("Assets/XidoStudio/Hexbase/Sistemes/Visualitzacions/Visualitzacions.asset");
    }
}
