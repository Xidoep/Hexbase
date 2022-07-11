using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

//FALTA:
//-En contes de que inicial sigui una peça, crear un sistema/scriptable que crei X peces aleatories pel mapa i 1 o + al centre.

public class Grid : MonoBehaviour
{
    //***************************************************
    //Potser aixo també es podria convertir en un Scriptable. Així l'escena queda neta.
    //***************************************************


    const int GRID_SIZE = 200;

    [SerializeField] Fase processar;
    [Header("Prefasb")]
    [SerializeField] GameObject prefab_Ranura;
    [SerializeField] GameObject prefab_Peça;
    [Space(10)]
    [Header("Peces")]
    [SerializeField] Estat inicial; //Posar aixo en un scriptable que controli la peça que s'ha seleccionat. "Seleccio" o algo aixi
    [SerializeField] Estat desbloquejadora;

    Hexagon[,] grid;
    //List<Hexagon> gridDebug;


    public Hexagon[] Veins(Vector2Int coordenades) => grid.Veins(coordenades);
    public Peça[] VeinsPeça(Vector2Int coordenades) => grid.VeinsPeça(coordenades);

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

    public void CrearDesdeRanura(Estat tipus, Vector2Int coordenada)
    {
        processar.Iniciar(coordenada);
    }
    public void CrearPeçaDesbloquejadora(Vector2Int coordenada) 
    {
        CrearPeça(desbloquejadora, coordenada);
    } 
    /// <summary>
    /// Crea el prefab peça, declarant la peça i les coordenades.
    /// </summary>
    public void CrearPeça(Estat tipus, Vector2Int coordenada, bool desbloquejadora = false)
    {
        Peça peçaFisica = Instanciar(prefab_Peça, coordenada.x, coordenada.y).GetComponent<Peça>();

        peçaFisica.Setup(this, coordenada, tipus);
        grid.Set(peçaFisica);
        //peçaFisica.Iniciar();

        foreach (var coodVei in grid.VeinsCoordenades(peçaFisica.Coordenades))
        {
            CrearRanura(coodVei);
        }

        if(!desbloquejadora)
            processar.Iniciar(peçaFisica);
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
        //ranura.Iniciar();

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
    }

}





