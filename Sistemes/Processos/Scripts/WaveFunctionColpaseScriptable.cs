using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC")]
public class WaveFunctionColpaseScriptable : ScriptableObject
{
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



    //Apunt!
    //només el falta agregar la propagació, i els tiles inicials. que de fet, ja torna
    //ambiguus els tiles del voltant de la peça, només faltaria que els de la peça fosin "fixos".

    //Proces de la propagacio
    //Affegir el tile que acabem de colapsar a un Stack
    //Per ara només hi ha un tile al stack. Per tant treiem l'ultim valor.
    //Actualitza les possiblitats del tile, si aquestes canvien, s'agafeix el tile a l'Stack
    Peça colocada;
    List<Peça> canviades;
    [SerializeField] WfcRegla[] regles;
    //[SerializeField] Fase_Colocar colocar;
    [SerializeField] List<TilePotencial> pendents;
    //private void OnEnable() => input.OnPerformedAdd(StepManual);
    //private void OnDisable() => input.OnPerformedRemove(StepManual);

    [SerializeField] List<TilePotencial> propagables;

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

    void OnEnable()
    {
        Random.InitState(1);
    }

    //INICIACIO
    public void Iniciar_WFC(Peça peça, List<Peça> canviades, System.Action _enFinalitzar, bool reiniciat = false)
    {
        Debugar.LogError("--------------WFC---------------");
        colocada = peça;
        this.canviades = canviades;
        enFinalitzar = _enFinalitzar;

        pendents = new List<TilePotencial>();

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
        Iniciar_Propagacio(pendents);
    }


    void AgafarTilesPeçaMesVeins(Peça peça)
    {
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
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
        actual.Escollir();
#if UNITY_EDITOR
        Debug.Log($"Resoldre {actual.Peça.gameObject.name} amb el tile {actual.PossibilitatsVirtuals.Get(0).Tile.name}");
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
        if(propagables.Count > 0)
        {
            if (TreuPossibilitatsImpossibles(propagables[0]))
            {
                if(propagables[0].PossibilitatsVirtuals.Count == 0)
                {
#if UNITY_EDITOR
                    string _debug = $"COLISIO ({propagables[0].EstatName}.{propagables[0].Orientacio})!!!\n";
#endif
                    //connexios = new Connexio[0];

#if UNITY_EDITOR
                    _debug += "Connexions exteriors = ";
#endif
                    connexios = GetConnexiosVirtuals(propagables[0], propagables[0].Veins[0], 0);
                    for (int i = 0; i < connexios.Length; i++)
                    {
                        _debug += connexios[i].name;
                        _debug += i != (connexios.Length - 1) ? ", " : "\n";
                    }

#if UNITY_EDITOR
                    _debug += "Connexions esquerra = ";
#endif
                    connexios = GetConnexiosVirtuals(propagables[0], propagables[0].Veins[1], 2);
                    for (int i = 0; i < connexios.Length; i++)
                    {
                        _debug += connexios[i].name;
                        _debug += i != (connexios.Length - 1) ? ", " : "\n";
                    }

#if UNITY_EDITOR
                    _debug += "Connexions dreta = ";
#endif
                    connexios = GetConnexiosVirtuals(propagables[0], propagables[0].Veins[2], 1);
                    for (int i = 0; i < connexios.Length; i++)
                    {
                        _debug += connexios[i].name;
                        _debug += i != (connexios.Length - 1) ? ", " : "\n";
                    }

#if UNITY_EDITOR
                    Possibilitats possibilitats = propagables[0].Peça.Possibilitats;
                    for (int i = 0; i < possibilitats.Count; i++)
                    {
                        _debug += $"{possibilitats.Get(i).Tile.name} | {possibilitats.Get(i).Tile.Exterior(0).name}, {possibilitats.Get(i).Tile.Esquerra(0).name}, {possibilitats.Get(i).Tile.Dreta(0).name} \n";
                    }

                    //MILLORAR AQUEST DEBUG I QUE EM MOSTRI TOTES LES LES OPCIONS POSSIBLES I COM LES HA PROVAT.
                    Debugar.LogError(_debug, propagables[0].Peça);
#endif
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

            Propagar();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }



    bool TreuPossibilitatsImpossibles(TilePotencial tile)
    {
        //string _debug = "";
        haCanviat = false;

        cExterior = GetConnexiosVirtuals(tile, tile.Veins[0], 0);
        cEsquerra = GetConnexiosVirtuals(tile, tile.Veins[1], 2);
        cDreta = GetConnexiosVirtuals(tile, tile.Veins[2], 1);

        toRemove = new List<Possibilitat>();
        for (int p = 0; p < tile.PossibilitatsVirtuals.Count; p++)
        {
            posibilitat = tile.PossibilitatsVirtuals.Get(p);
            found = false;
            for (int c1 = 0; c1 < cExterior.Length; c1++)
            {
                for (int c3 = 0; c3 < cEsquerra.Length; c3++)
                {
                    for (int c2 = 0; c2 < cDreta.Length; c2++)
                    {

                        //_debug += $"|-{cExterior[c1].name}, {cEsquerra[c2].name}, {cDreta[c3].name} = " +
                        //    $"{cExterior[c1].name}, {cEsquerra[c3].name}, {cDreta[c2].name} ?";
                        if (tile.CompararConnexions(posibilitat, cExterior[c1], cEsquerra[c3], cDreta[c2])) 
                        {
                            found = true;
                            //_debug += "------------------------------------------¡¡¡MATCH!!!";
                        }
                        //_debug += "\n";
                    }
                }
            }
            if (!found) 
            {
                toRemove.Add(posibilitat);
                haCanviat = true;
            } 
            //Debug.Log(_debug);
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            tile.PossibilitatsVirtuals.Remove(toRemove[i]);
        }

        
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
                    if (tile.Peça.ConnexionsEspesifica != null && tile.Peça.ConnexionsEspesifica.subestats.Contains(vei.Peça.Subestat))
                    {
                        _connexio.AddRange(tile.Peça.ConnexionsEspesifica.connexions);
                        return _connexio.ToArray();
                    }
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
            if (tile.Peça.TeConnexionsNules)
                return tile.Peça.ConnexionsNules;

            return tile.Peça.ConnexionsPossibles;
        }
    }





    void Finalitzar()
    {
        Debugar.LogError("WFC ACABAT!");

        colocada.CrearTilesFisics();
        XS_Coroutine.StartCoroutine_Ending(1.5f, colocada.CrearDetalls);
        for (int i = 0; i < colocada.VeinsPeça.Count; i++)
        {
            XS_Coroutine.StartCoroutine_Ending(.5f, colocada.VeinsPeça[i].CrearTilesFisics);
            XS_Coroutine.StartCoroutine_Ending(1.5f, colocada.VeinsPeça[i].CrearDetalls);
        }

        for (int i = 0; i < canviades.Count; i++)
        {
            if (canviades[i] == colocada)
                continue;

            XS_Coroutine.StartCoroutine_Ending(.5f, canviades[i].CrearTilesFisics);
            XS_Coroutine.StartCoroutine_Ending(1.5f, canviades[i].CrearDetalls);
        }

        enFinalitzar.Invoke();
    }



}

