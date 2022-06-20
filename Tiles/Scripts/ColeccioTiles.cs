using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Coleccio Tiles")]
public class ColeccioTiles : ScriptableObject
{
    #region INSTANCE
    static ColeccioTiles Instance;
    private void OnEnable()
    {
        Instance = this;
    }
    #endregion

    [field: SerializeReference]
    [SerializeField] Tile[] tiles;
    [SerializeField] Connexio[] connexios;
    [SerializeField] Peça desbloquejadora;

    public Tile[] Tiles => tiles;
    public static Connexio[] Connexios => Instance.connexios;

    private void OnValidate()
    {
        tiles = XS_Editor.LoadAllAssetsAtPath<Tile>("Assets/XidoStudio/Hexbase/Tiles").ToArray();
        connexios = XS_Editor.LoadAllAssetsAtPath<Connexio>("Assets/XidoStudio/Hexbase/Connexio").ToArray();
    }
}
