using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC")]
public class WaveFunctionColpaseScriptable : ScriptableObject
{
    //*********************************************************************
    //*********************************************************************
    //*********************************************************************
    //*********************************************************************
    //FALTARIA FER QUE FOS CONTENT AGNOSTIC
    //*********************************************************************
    //*********************************************************************
    //*********************************************************************
    //*********************************************************************


    //1.-Triar tots els tiles que formaran part del WFC.
    //Estaria be que només fossin els del voltant de la peça.
    //Osigui, els veins exteriors i els veins del veins.
    //Pero això pot portar a "colisions". [...]
    //Per tant, cambe agregarem els tiles de la peça peró amb una sola possiblitat.

    //2.- Propagacio:
    //Quan es selecciona un tile (en aquest cas seràn els tiles de la peça colocada).
    //S'han de "actualitzar" les possiblitats de tots els veins.
    //Si aquestes possiblitats canvien (osgui, que no son com al inici de la comprovacio),
    //Es "propaga" aquesta actualitzacio a tots els seus veins.
    //[veus? crec que aquesta era la part que em deixava]

    //3.- Lowest entropy:
    //Busca el tile amb el minim de possibiliats.
    //Si nomes te una solcio, la seleccionar, si en te mes d'una tries en random.


    Peça colocada;
    List<Peça> canviades;
    [SerializeField] bool iniciat;
    [SerializeField] WfcRegla[] regles;
    [SerializeField] int colisions = 0;
    [SerializeField] Possibilitats viables;
    [SerializeField] List<TilePotencial> pendents;
    //[SerializeField] Fase_Colocar colocar;
    //private void OnEnable() => input.OnPerformedAdd(StepManual);
    //private void OnDisable() => input.OnPerformedRemove(StepManual);

    [SerializeField] List<TilePotencial> propagables;

    [SerializeField] Possibilitats all;
    //[SerializeField] List<string> posibleMissingTiles;





    System.Action enFinalitzar;

    public static bool veureProces = false;
    //public static System.Random random;

    //INTERN
    TilePotencial actual;
    bool reglesAprovades;
    List<TilePotencial> lowestEntropy;
    Connexio[] connexios;
    bool haCanviat;
    Connexio[] cExterior;
    Connexio[] cEsquerra;
    Connexio[] cDreta;
    List<Possibilitat> toRemove;
    Possibilitat posibilitat;
    bool found;
    List<Connexio> _connexio;



    Connexio[] debugExterior;
    Connexio[] debugDreta;
    Connexio[] debugEsquerra;
    Possibilitats debugPossiblitats;
    bool debugMatch;
    string _debug;
    //List<Possibilitat> viables;


    public Possibilitats SetAll { set => all = value; }

    void OnEnable()
    {
        Random.InitState(1);
        colisions = 0;
        iniciat = false;
        //posibleMissingTiles = new List<string>();
    }

    //INICIACIO
    public void Iniciar_WFC(Peça peça, List<Peça> canviades, System.Action _enFinalitzar, bool reiniciat = false)
    {
        /*
        if (iniciat)
        {
            Debugar.LogError("--------------ADD to WFC---------------");
            peça.CrearTilesPotencials();
            peça.AssignarVeinsTiles(peça.Tiles);

            AgafarTilesPeçaMesVeins(peça);
            return;
        }
        */
        Debugar.LogError("--------------WFC---------------");
        colocada = peça;
        this.canviades = canviades;
        enFinalitzar = _enFinalitzar;

        pendents = new List<TilePotencial>();
        toRemove = new List<Possibilitat>();

        if (!reiniciat)
        {
            //Preparar els tiles
            peça.CrearTilesPotencials();
            peça.AssignarVeinsTiles(peça.Tiles);
        }
        
        //Agafar informacio
        AgafarTilesPeçaMesVeins(peça);
        AgafarTilesCanviadesSiNecessari(canviades);

        //Propacació inial on mirem les possiblitats de totes les peces.
        iniciat = true;
        Iniciar_Propagacio(pendents);
    }

    void AgafarTilesPeçaMesVeins(Peça peça)
    {
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (colisions > 0)
                peça.Tiles[i].Comodi(all);
            else peça.Tiles[i].Ambiguo();
            peça.Tiles[i].Ambiguo();

            if (peça.Tiles[i].Veins[0] == null)
                continue;

            peça.Tiles[i].Veins[0].Ambiguo();
            peça.Tiles[i].Veins[0].Veins[0] = peça.Tiles[i];
            peça.Tiles[i].Veins[0].Veins[1].Ambiguo();
            peça.Tiles[i].Veins[0].Veins[2].Ambiguo();
        }

        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            pendents.Add(peça.Tiles[i]);

            if (peça.Tiles[i].Veins[0] == null)
                continue;

            pendents.Add(peça.Tiles[i].Veins[0]);
            pendents.Add(peça.Tiles[i].Veins[0].Veins[1]);
            pendents.Add(peça.Tiles[i].Veins[0].Veins[2]);
        }

    }


    void AgafarTilesCanviadesSiNecessari(List<Peça> peces)
    {
        if (peces.Count == 0)
            return;

        for (int p = 0; p < peces.Count; p++)
        {
            AgafarTilesPeçaMesVeins(peces[p]);
        }
    }





    //MAIN STEPS
    void StepManual(InputAction.CallbackContext context) => Step();
    void PropagarManual(InputAction.CallbackContext context) => Propagar();


    void Step()
    {
        actual = TileWithTheLowestEntropy();
        pendents.Remove(actual);
        Debug.Log($"{actual.Peça.name}({actual.Orientacio})");
        actual.Escollir(colisions);
#if UNITY_EDITOR
        //Debug.Log($"Resoldre {actual.Peça.gameObject.name} amb el tile {actual.PossibilitatsVirtuals.Get(0).Tile.name}");
#endif
        if (pendents.Count == 0)
        {
            reglesAprovades = true;
            for (int r = 0; r < regles.Length; r++)
            {
                if (!regles[r].Comprovar(colocada))
                {
#if UNITY_EDITOR
                    Debug.LogError($"REESTART!!! Per que la regla {r}, no s'ha aprovat.", colocada);
#endif
                    Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                    return;
                }

                for (int c = 0; c < canviades.Count; c++)
                {
                    if (!regles[r].Comprovar(canviades[c]))
                    {
#if UNITY_EDITOR
                        Debugar.LogError($"REESTART!!! Per que la regla {r}, no s'ha aprovat.", canviades[c]);
#endif
                        Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                        return;
                    }
                }
                for (int v = 0; v < colocada.VeinsPeça.Count; v++)
                {
                    if (!regles[r].Comprovar(colocada.VeinsPeça[v]))
                    {
#if UNITY_EDITOR
                        Debugar.LogError($"REESTART!!! Per que la regla {r}, no s'ha aprovat.", colocada.VeinsPeça[v]);
#endif
                        Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                        return;
                    }
                }
            }
            
            

            Finalitzar();
        }
        else
        {
            Iniciar_Propagacio(actual);
        }
    }


    TilePotencial TileWithTheLowestEntropy()
    {
        lowestEntropy = new List<TilePotencial>();
        for (int i = 0; i < pendents.Count; i++)
        {
            if (lowestEntropy.Count > 0)
            {
                if(pendents[i].PossibilitatsVirtuals.Count < lowestEntropy[0].PossibilitatsVirtuals.Count)
                {
                    lowestEntropy = new List<TilePotencial>() { pendents[i] };
                }
                else if (pendents[i].PossibilitatsVirtuals.Count == lowestEntropy[0].PossibilitatsVirtuals.Count)
                {
                    lowestEntropy.Add(pendents[i]);
                }
            }
            else lowestEntropy.Add(pendents[i]);
        }
        return lowestEntropy[UnityEngine.Random.Range(0, lowestEntropy.Count)];
    }






    //PROPAGACIO
    void Iniciar_Propagacio(List<TilePotencial> tiles)
    {
        propagables = new List<TilePotencial>();
        for (int i = 0; i < tiles.Count; i++)
        {
            propagables.Add(tiles[i]);
        }
        Propagar();
    }
    void Iniciar_Propagacio(TilePotencial tile)
    {
        //propagables.Add(tile);
        for (int i = 0; i < tile.Veins.Length; i++)
        {
            if (tile.Veins[i] != null && !tile.Veins[i].Resolt) propagables.Add(tile.Veins[i]);
        }
        Propagar();
    }
    void Reiniciar() 
    {
        if (!canviades.Contains(propagables[0].Peça)) canviades.Add(propagables[0].Peça);
        Iniciar_WFC(colocada, canviades, enFinalitzar, true);
    }


    void Propagar()
    {
        //Debug.Log($"Propagables {propagables.Count}");
        if(propagables.Count > 0)
        {
            if (TreuPossibilitatsImpossibles(propagables[0]))
            {
                if (propagables[0].PossibilitatsVirtuals.Count == 0)
                {
                    _debug = $"SENSE POSSIBILITATS ({propagables[0].EstatName})!!!\n";
                    viables = new Possibilitats();
                    //if (posibleMissingTiles == null) posibleMissingTiles = new List<string>();

                    for (int ext = 0; ext < (propagables[0].Veins[0] == null ? propagables[0].Peça.ConnexionsNules.Length : propagables[0].Veins[0].PossibilitatsVirtuals.Count); ext++)
                    {
                        for (int dre = 0; dre < propagables[0].Veins[1].PossibilitatsVirtuals.Count; dre++)
                        {
                            for (int esq = 0; esq < propagables[0].Veins[2].PossibilitatsVirtuals.Count; esq++)
                            {
                                string s = $"{(propagables[0].Veins[0] == null ? propagables[0].Peça.ConnexionsNules[ext].name : propagables[0].Veins[0].PossibilitatsVirtuals.Get(ext).Tile.Exterior(propagables[0].Veins[0].PossibilitatsVirtuals.Get(ext).Orientacio).name)}|{propagables[0].Veins[1].PossibilitatsVirtuals.Get(dre).Tile.Esquerra(propagables[0].Veins[1].PossibilitatsVirtuals.Get(dre).Orientacio).name}|{propagables[0].Veins[2].PossibilitatsVirtuals.Get(esq).Tile.Dreta(propagables[0].Veins[2].PossibilitatsVirtuals.Get(esq).Orientacio).name}";
                                //if (!posibleMissingTiles.Contains(s))
                                //    posibleMissingTiles.Add(s);
                                
                                for (int i = 0; i < all.Count; i++)
                                {
                                    if (all.Get(i).Tile.CompararConnexions(
                                        all.Get(i),
                                        propagables[0].Veins[0] == null ? 
                                            propagables[0].Peça.ConnexionsNules[ext] : propagables[0].Veins[0].PossibilitatsVirtuals.Get(ext).Tile.Exterior(propagables[0].Veins[0].PossibilitatsVirtuals.Get(ext).Orientacio),
                                        propagables[0].Veins[1].PossibilitatsVirtuals.Get(dre).Tile.Esquerra(propagables[0].Veins[1].PossibilitatsVirtuals.Get(dre).Orientacio),
                                        propagables[0].Veins[2].PossibilitatsVirtuals.Get(esq).Tile.Dreta(propagables[0].Veins[2].PossibilitatsVirtuals.Get(esq).Orientacio)
                                        ))
                                    {

                                        _debug += $"{all.Get(i).Tile.name} - {all.Get(i).Orientacio}\n";

                                        if (!viables.Contains(all.Get(i)))
                                        {
                                            
                                            viables.Add(all.Get(i));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if(viables.Count > 0)
                    {
                        int r = Random.Range(0, viables.Count);
                        propagables[0].PossibilitatsVirtuals.Add(viables.Get(r));
                        for (int i = 0; i < viables.Count; i++)
                        {
                            _debug += $"{viables.Count} - {all.Get(i).Tile.name}{(r == i ? "   <-------- TRIADA!" : "")}\n";
                        }
                    }

                    Debugar.LogError(_debug, propagables[0].Peça);
                }

                if (propagables[0].PossibilitatsVirtuals.Count == 0)
                {

                    _debug = $"COLISIO ({propagables[0].EstatName})!!!\n";

                    //Possibilitats possibilitats = propagables[0].Peça.Possibilitats;
                    /*for (int i = 0; i < possibilitats.Count; i++)
                    {
                        _debug += $"{possibilitats.Get(i).Tile.name} | {possibilitats.Get(i).Tile.Exterior(0).name}, {possibilitats.Get(i).Tile.Esquerra(0).name}, {possibilitats.Get(i).Tile.Dreta(0).name} \n";
                    }*/
                    /*connexionsEnColapse = new List<Connexio>();
                    if (propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Count > 0)
                    {
                        if(!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                    }
                    if (propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Count > 0)
                    {
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).Exterior(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).Dreta(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                    }
                    if (propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Count > 0)
                    {
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).Exterior(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).Dreta(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                    }
                    if (propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Count > 0)
                    {
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).Exterior(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).Dreta(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                    }
                    if (propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Count > 0)
                    {
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).Exterior(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).Dreta(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                    }
                    if (propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Count > 0)
                    {
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).Exterior(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).Dreta(0));
                        if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                    }


                    if(propagables[0].Peça.Tiles[0].Veins[0] != null)
                    {
                        if (propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Count > 0)
                        {
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[0].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));
                        }
                    }
                    if (propagables[0].Peça.Tiles[1].Veins[0] != null)
                    {
                        if (propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Count > 0)
                        {
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[1].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));

                        }
                    }
                    if (propagables[0].Peça.Tiles[2].Veins[0] != null)
                    {
                        if (propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Count > 0)
                        {
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[2].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));

                        }
                    }
                    if (propagables[0].Peça.Tiles[3].Veins[0] != null)
                    {
                        if (propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Count > 0)
                        {
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[3].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));

                        }
                    }
                    if (propagables[0].Peça.Tiles[4].Veins[0] != null)
                    {
                        if (propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Count > 0)
                        {
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[4].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));

                        }
                    }
                    if (propagables[0].Peça.Tiles[5].Veins[0] != null)
                    {
                        if (propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Count > 0)
                        {
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Tile(0).Exterior(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Tile(0).Dreta(0));
                            if (!connexionsEnColapse.Contains(propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0))) connexionsEnColapse.Add(propagables[0].Peça.Tiles[5].Veins[0].PossibilitatsVirtuals.Tile(0).Esquerra(0));

                        }
                    }*/




                    _debug += "\nTILES ACTUALS:\n";
                    _debug += $"0- {(propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Count > 0 ? propagables[0].Peça.Tiles[0].PossibilitatsVirtuals.Tile(0).name : " - ")}\n";
                    _debug += $"1- {(propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Count > 0 ? propagables[0].Peça.Tiles[1].PossibilitatsVirtuals.Tile(0).name : " - ")}\n";
                    _debug += $"2- {(propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Count > 0 ? propagables[0].Peça.Tiles[2].PossibilitatsVirtuals.Tile(0).name : " - ")}\n";
                    _debug += $"3- {(propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Count > 0 ? propagables[0].Peça.Tiles[3].PossibilitatsVirtuals.Tile(0).name : " - ")}\n";
                    _debug += $"4- {(propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Count > 0 ? propagables[0].Peça.Tiles[4].PossibilitatsVirtuals.Tile(0).name : " - ")}\n";
                    _debug += $"5- {(propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Count > 0 ? propagables[0].Peça.Tiles[5].PossibilitatsVirtuals.Tile(0).name : " - ")}\n";

                    _debug += $"\nCONTEXTE COLISIO:\n";
                    _debug += "Vei 0- ";
                    if (propagables[0].Veins[0] == null)
                    {
                        for (int i = 0; i < propagables[0].Peça.ConnexionsNules.Length; i++)
                        {
                            _debug += $"{propagables[0].Peça.ConnexionsNules[i].name }, ";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < propagables[0].Veins[0].PossibilitatsVirtuals.Count; i++)
                        {
                            _debug += $"{propagables[0].Veins[0].PossibilitatsVirtuals.Get(i).Tile.Exterior(propagables[0].Veins[0].PossibilitatsVirtuals.Get(i).Orientacio).name}, ";
                        }
                    }
                    _debug += "\n";

                    _debug += "Vei 1- ";
                    for (int i = 0; i < propagables[0].Veins[1].PossibilitatsVirtuals.Count; i++)
                    {
                        _debug += $"{propagables[0].Veins[1].PossibilitatsVirtuals.Get(i).Tile.Esquerra(propagables[0].Veins[1].PossibilitatsVirtuals.Get(i).Orientacio).name}, ";
                    }
                    _debug += "\n";

                    _debug += "Vei 2- ";
                    for (int i = 0; i < propagables[0].Veins[2].PossibilitatsVirtuals.Count; i++)
                    {
                        _debug += $"{propagables[0].Veins[2].PossibilitatsVirtuals.Get(i).Tile.Dreta(propagables[0].Veins[2].PossibilitatsVirtuals.Get(i).Orientacio).name}, ";
                    }
                    _debug += "\n";

                    colisions++;
                    Debugar.LogError(_debug, propagables[0].Peça);

                    XS_Coroutine.StartCoroutine_Ending(0.001f, Reiniciar);  
                    return;
                }

                for (int i = 0; i < propagables[0].Veins.Length; i++)
                {
                    if (!propagables.Contains(propagables[0].Veins[i]))
                    {
                        if (propagables[0].Veins[i] != null)
                        {
                            if (propagables[0].Veins[i].Resolt)
                                continue;

                            propagables.Add(propagables[0].Veins[i]);
                        }
                    }
                }
            }
            propagables.RemoveAt(0);

            //XS_Coroutine.StartCoroutine_Ending(0, Propagar);
            Propagar();
            return;
        }

        
        XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }



    bool TreuPossibilitatsImpossibles(TilePotencial tile)
    {
#if UNITY_EDITOR
        string _debug = $"{tile.Peça.name}({tile.Orientacio})\n";
#endif
        haCanviat = false;

        cExterior = GetConnexiosVirtuals(tile, tile.Veins[0], 0);
        cEsquerra = GetConnexiosVirtuals(tile, tile.Veins[1], 2);
        cDreta = GetConnexiosVirtuals(tile, tile.Veins[2], 1);


        toRemove.Clear();
        
        _debug += "\nCONNEXIONS\n";
        _debug += "Exterior: ";
        for (int i = 0; i < cExterior.Length; i++)
        {
            _debug += $"{cExterior[i].name}, ";
        }
        _debug += "\n";
        _debug += "Esquerra: ";
        for (int i = 0; i < cEsquerra.Length; i++)
        {
            _debug += $"{cEsquerra[i].name}, ";
        }
        _debug += "\n";
        _debug += "Dreta: ";
        for (int i = 0; i < cDreta.Length; i++)
        {
            _debug += $"{cDreta[i].name}, ";
        }
        _debug += "\n";
        
        //toRemove = new List<Possibilitat>();


        _debug += "\nINTENTS\n";
        for (int p = 0; p < tile.PossibilitatsVirtuals.Count; p++)
        {
            //_debug += $"{tile.PossibilitatsVirtuals.Get(p).Tile.name}({tile.PossibilitatsVirtuals.Get(p).Orientacio})";
            posibilitat = tile.PossibilitatsVirtuals.Get(p);
            found = false;
            for (int c1 = 0; c1 < cExterior.Length; c1++)
            {
                for (int c3 = 0; c3 < cEsquerra.Length; c3++)
                {
                    for (int c2 = 0; c2 < cDreta.Length; c2++)
                    {
                        if (tile.CompararConnexions(posibilitat, cExterior[c1], cEsquerra[c3], cDreta[c2])) 
                        {
                            found = true;
#if UNITY_EDITOR
                            _debug += $"{tile.PossibilitatsVirtuals.Get(p).Tile.name}({tile.PossibilitatsVirtuals.Get(p).Orientacio})------------------------------------------¡¡¡MATCH!!!\n";
#endif
                        }
                    }
                }
            }
            if (!found) 
            {

                toRemove.Add(posibilitat);
                haCanviat = true;
            }
            _debug += "\n";
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            tile.PossibilitatsVirtuals.Remove(toRemove[i]);
        }

#if UNITY_EDITOR
        Debug.Log(_debug);
#endif

        return haCanviat;
    }



    Connexio[] GetConnexiosVirtuals(TilePotencial tile, TilePotencial vei, int veiContrari)
    {
        _connexio = new List<Connexio>();

        //Debug.Log($"vei = {vei}");
        if (vei != null)
        {
            if (vei.Resolt)
            {
                return new Connexio[] { vei.GetConnexions(vei.PossibilitatsVirtuals.Get(0))[veiContrari] };
            }
            else
            {
                _connexio.Clear();
                if(vei.Peça != tile.Peça)//Si el vei no es intern
                {
                    bool teConnexionsEspedifiquesPelVei = false;
                    for (int i = 0; i < tile.Peça.ConnexionsEspesifiques.Length; i++)
                    {
                        if (tile.Peça.ConnexionsEspesifiques[i].subestats.Contains(vei.Peça.Subestat))
                        {
                            _connexio.AddRange(tile.Peça.ConnexionsEspesifiques[i].connexions);
                            teConnexionsEspedifiquesPelVei = true;
                            //return _connexio.ToArray();
                        }
                    }
                    if(teConnexionsEspedifiquesPelVei)
                        return _connexio.ToArray();
                }
                
                for (int p = 0; p < vei.PossibilitatsVirtuals.Count; p++)
                {
                    if (!_connexio.Contains(vei.PossibilitatsVirtuals.Tile(p).Exterior(vei.PossibilitatsVirtuals.Orietacio(p))))
                        _connexio.Add(vei.PossibilitatsVirtuals.Tile(p).Exterior(vei.PossibilitatsVirtuals.Orietacio(p)));

                    if (!_connexio.Contains(vei.PossibilitatsVirtuals.Tile(p).Esquerra(vei.PossibilitatsVirtuals.Orietacio(p))))
                        _connexio.Add(vei.PossibilitatsVirtuals.Tile(p).Esquerra(vei.PossibilitatsVirtuals.Orietacio(p)));

                    if (!_connexio.Contains(vei.PossibilitatsVirtuals.Tile(p).Dreta(vei.PossibilitatsVirtuals.Orietacio(p))))
                        _connexio.Add(vei.PossibilitatsVirtuals.Tile(p).Dreta(vei.PossibilitatsVirtuals.Orietacio(p)));
                }

                return _connexio.ToArray();
            }
        }
        else
        {
            if (tile.Peça.TeConnexionsNules && colisions < 3)
                return tile.Peça.ConnexionsNules;

            return tile.Peça.ConnexionsPossibles;
        }
    }





    void Finalitzar()
    {
        Debugar.LogError("WFC ACABAT!");
        enFinalitzar?.Invoke();
        colisions = 0;
        iniciat = false;
    }



}

