using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
/*
public static class WaveFunctionColapse
{

    static Peça peçaCreada;
    static List<TilePotencial> pendents;
    static int index = 0;
    static Hexagon ultimaPeçaCreada;
    static int interaccionsTotals;
    static TilePotencial ultimaPreçaComprovada;
    public static Hexagon UltimaPeçaCreada { get => ultimaPeçaCreada; set => ultimaPeçaCreada = value; }


    static List<Peça> pecesModificades;
    #region DEBUG
    static bool debug;

    static string _debug;
    #endregion

    static bool HaFinalitzat => pendents.Count == 0;
    static System.Action enFinalitzar;
    static System.Action<Vector2Int> enDesbloquejar;


    static bool iniciat = false;
    public static void Process(Peça _peça, System.Action _enFinalitzar, System.Action<Vector2Int> _enDesbloquejar, bool _debug = true)
    {
        Debug.LogError("--------------WFC---------------");
        peçaCreada = _peça;
        if (iniciat)
        {
            //Prioritaria = _peça.Inicial;
            for (int i = 0; i < _peça.Tiles.Length; i++)
            {
                Add(_peça.Tiles[i]);
            }
            return;
        }

        iniciat = true;
        index = 0;
        interaccionsTotals = 0;
        StartPendents();

        //Prioritaria = _peça.Inicial;
        for (int i = 0; i < _peça.Tiles.Length; i++)
        {
             Add(_peça.Tiles[i]);
        }

        pecesModificades = new List<Peça>() { _peça };

        enFinalitzar = _enFinalitzar;
        enDesbloquejar = _enDesbloquejar;
        debug = _debug;

        IndexMesProvable();
        //Step();
        XS_Coroutine.StartCoroutine_Ending(0.5f, Step);
    }
    public static void StartPendents()
    {
        if (pendents == null) pendents = new List<TilePotencial>();
    }

    static void Step()
    {

        Debug.Log(pendents.Count);
        if (HaFinalitzat)
        {
            Finalitzar();
            return;
        }

        if(interaccionsTotals > 1000)
        {
            Debug.LogError($"CREAR NOVA PEÇA PERQUE S'HA BLOQUEJAT ({peçaCreada.name}-{peçaCreada.Coordenades})", peçaCreada);
            interaccionsTotals = 0;

            for (int t = 0; t < peçaCreada.Tiles.Length; t++)
            {
                if(pendents.Contains(peçaCreada.Tiles[t]))
                    pendents.Remove(peçaCreada.Tiles[t]);
            }

            Vector2Int coordenades = peçaCreada.Coordenades;
            //Grid grid = peçaCreada.Grid;
            pecesModificades.Remove(peçaCreada);
            MonoBehaviour.Destroy(peçaCreada.gameObject);
            //grid.CrearPeçaDesbloquejadora(coordenades);
            enDesbloquejar.Invoke(coordenades);

            XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
            return;
        }
        //IndexMesProvable();


        WFC(pendents[Mathf.Clamp(index, 0, pendents.Count - 1)]);

        XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }
    
    public static void Add(TilePotencial tilePotencial)
    {
        if (!pendents.Contains(tilePotencial)) pendents.Add(tilePotencial);
    }

    static void Finalitzar()
    {
        Debugar.Log("FINISH WFC");
        //************************************************************************************
        //Abans de crear els tiles fisics s'han d'analitzar els patrons de les peces creades buscant patrons pels tiles multiples.
        //************************************************************************************
        pecesModificades[0].CrearTilesFisics();
        if(pecesModificades.Count > 1)
        {
            for (int i = 1; i < pecesModificades.Count; i++)
            {
                //XS_Coroutine.StartCoroutine_Ending(temps, pecesModificades[i].CrearTilesFisics);
                pecesModificades[i].CrearTilesFisics();
            }
        }

        iniciat = false;
        enFinalitzar?.Invoke();
    }

    static Connexio[] cExterior;
    static Connexio[] cEsquerra;
    static Connexio[] cDreta;

    static void WFC(TilePotencial tile)
    {
        interaccionsTotals ++;
        if (tile.Assegurat)
        {
            Debug.Log("ASSEGURAT!");
            return;
        }



        #region DEBUG
        if (debug) Debug.Log("------------------");
        _debug = $"[{tile.ID}({tile.Coordenades})]:\n";

        #endregion

        

        if(tile.Resolt && Assegurar(tile))
        {
            pendents.RemoveAt(pendents.IndexOf(tile));
            return;
        }

        ultimaPreçaComprovada = tile;
        tile.Interactuar();


        TriarPossibilitat(tile, TilesPossibles(tile));
        IndexMesProvable(tile);
    }

    static bool Assegurar(TilePotencial tile)
    {
        //if (tile.TileFisic == null)
        //    return false;

        if (tile.Assegurat)
            return false;

        Possibilitats p = TilesPossibles(tile);
        bool encaixa = true;
        for (int i = 0; i < p.Count; i++)
        {
            if (p.Tile(i) != tile.Possibilitats[0]) encaixa = false;
        }
        Debug.Log($"ASSEGURADA = {encaixa}");
        tile.Assegurat = encaixa;
        return encaixa;
    }

    /// <summary>
    /// Busca l'index amb mes possiblitats de ser resolt. Si se l'hi passa un Tile, agafa els seus veins; si no, els agafa tots.
    /// </summary>
    static void IndexMesProvable(TilePotencial tile = null)
    {
        int _veinsResolts = 0;
        int _entrophyLevel = 100;
        int _interaccions = 100;
        int _index = 0;
        List<TilePotencial> _pendents;

        if(interaccionsTotals % 5 == 0)
        {
            _pendents = pendents;
        }
        else
        {
            if (tile.VeinsResolts == 3)
            {
                _pendents = pendents;
            }
            else
            {
                _pendents = new List<TilePotencial>();
                for (int i = 0; i < tile.Veins.Length; i++)
                {
                    if (tile.Veins[i] == null)
                        continue;

                    if (!tile.Veins[i].Resolt)
                        _pendents.Add(tile.Veins[i]);
                }
                if (_pendents.Count == 0)
                {
                    _pendents = pendents;
                }
            }

            if (_pendents.Count == 0 && pendents.Count != 0)
                _pendents = pendents;

        }






        for (int i = 0; i < _pendents.Count; i++)
        {
            
            if (_pendents[i] == ultimaPreçaComprovada)
                continue;

            if (_pendents[i].Resolt)
                continue;

            _index = i;
            if (_pendents[i].VeinsResolts > _veinsResolts)
            {
                _veinsResolts = _pendents[i].VeinsResolts;
                _index = i;
            }
            else if (_pendents[i].VeinsResolts == _veinsResolts)
            {
                Possibilitats p = TilesPossibles(_pendents[i],false);
                if (p.Count < _entrophyLevel)
                {
                    _entrophyLevel = p.Count;
                    _index = i;
                }
            }
        }

        _debug = "Pendents: \n";
        for (int i = 0; i < pendents.Count; i++)
        {
            _debug += $"{pendents[i].ID}{(pendents[i].Resolt ? "[Resolt]" : "")}\n";
        }
        Debug.Log(_debug);

        _debug = $"El mes provable es: {_pendents[_index].ID}";
        for (int i = 0; i < _pendents.Count; i++)
        {
            _debug += $"{_pendents[i].ID}{(_pendents[i].Resolt ? "[Resolt]" : "")}\n";
        }
        Debug.Log(_debug);

        if (_index == _pendents.IndexOf(ultimaPreçaComprovada))
        {
            index = Random.Range(0, _pendents.Count);
            Debug.Log($"Pero es igual que la ultima, així que triaré: {_pendents[index].ID}");
            return;
        }

        index = _index;
    }

    static void TransferirAlsVeins(TilePotencial tile)
    {
        for (int i = 0; i < tile.Veins.Length; i++)
        {
            if(tile.Veins[i] == null)
            {
                #region DEBUG
                _debug += $"null, ";
                #endregion
                continue;
            }

            if (!tile.Veins[i].Resolt) Add(tile.Veins[i]);
        }
    }
    

    static Connexio[] GetConnexions(TilePotencial tile, TilePotencial vei, int veiContrari)
    {
        List<Connexio> _tmp = new List<Connexio>();

        if (vei != null)
        {
            #region DEBUG
            _debug += $"Vei({vei.ID}):";
            #endregion
            Connexio[] connexions = vei.GetConnexions(0);
            if (vei.Resolt)
            {
                #region DEBUG
                //_debug += $"[RESOLT] A.{connexions[0]}- E.{connexions[1]}- D.{connexions[2]}\n";
                #endregion
                return new Connexio[] { connexions[veiContrari] };
            }
            else
            {
                #region DEBUG
                //_debug += $"({vei.ID}) {vei.Possibilitats.Length} Possiblitats\n";
                #endregion
                _tmp.Clear();
                for (int p = 0; p < vei.Possibilitats.Length; p++)
                {
                    #region DEBUG
                    //_debug += $"|-Possibilitat [{p}] = {vei.Possibilitats[p].name}\n";
                    #endregion
                    foreach (var connexio in vei.GetConnexions(p))
                    {
                        if (connexio == null)
                            continue;

                        if (!_tmp.Contains(connexio))
                            _tmp.Add(connexio);
                        #region DEBUG
                        //_debug += $"|---Connexion{connexio.name}\n";
                        #endregion
                    }
                }
                _tmp.ToArray();
            }
        }
        else
        {
            #region DEBUG
            _debug += "Es null\n";
            #endregion
            Connexio[] especifics = tile.ConnexionsNules.Invoke(tile);
            return especifics == null ? tile.Connexions : especifics;
            //return ColeccioTiles.Connexios;

        }
        return _tmp.ToArray();
    }


    static Possibilitats TilesPossibles(TilePotencial tile, bool debug = true)
    //static Possibilitats TrobarPeçaViable(TilePotencial tilePotencial, Connexio[] cExterior, Connexio[] cDreta, Connexio[] cEsquerra)
    //void Interaccio(Connexio[] cUp, Connexio[] cRight, Connexio[] cDown, Connexio[] cLeft)
    {
        cExterior = GetConnexions(tile, tile.Veins[0], 0);
        cEsquerra = GetConnexions(tile, tile.Veins[1], 2);
        cDreta = GetConnexions(tile, tile.Veins[2], 1);
        #region DEBUG
        if (debug) 
        {
            //Debug.Log(_debug);
            _debug = "";
            _debug += $"{tile.ID} Connexions:";

            _debug += $"\nExterior - from({tile.Veins[0]?.ID})[{cExterior.Length}] = ";
            for (int i = 0; i < cExterior.Length; i++) { _debug += $"{cExterior[i].name}, "; }
            _debug += $"\nEsquerra - from({tile.Veins[1]?.ID})[{cEsquerra.Length}] = ";
            for (int i = 0; i < cEsquerra.Length; i++) { _debug += $"{cEsquerra[i].name}, "; }
            _debug += $"\nDreta - from({tile.Veins[2]?.ID})[{cDreta.Length}] = ";
            for (int i = 0; i < cDreta.Length; i++) { _debug += $"{cDreta[i].name}, "; }

            if (debug) Debug.Log(_debug);
        }
        #endregion

        int orientacioFisica = -1;
        Possibilitats possibilitats = new Possibilitats(new List<Possibilitat>());
        for (int p = 0; p < tile.Possibilitats.Length; p++)
        {
            #region DEBUG
            _debug = $"{tile.Possibilitats[p].name} - Connexions = (A.{tile.GetConnexions(p)[0].name} -E.{tile.GetConnexions(p)[1].name} -D.{tile.GetConnexions(p)[2].name})...\n";
            #endregion
            for (int c1 = 0; c1 < cExterior.Length; c1++)
            {
                for (int c3 = 0; c3 < cEsquerra.Length; c3++)
                {
                    for (int c2 = 0; c2 < cDreta.Length; c2++)
                    {
                        orientacioFisica = tile.CompararConnexions(p, cExterior[c1], cEsquerra[c3], cDreta[c2]);
                        //if (tilePotencial.CompararConnexions(out int orientacio, p, cExterior[c1], cLeft[c3], cRight[c2]))
                        if (orientacioFisica != -1)
                        {
                            if (!possibilitats.Contains(tile.Possibilitats[p]))
                            {
                                possibilitats.Add(tile.Possibilitats[p], orientacioFisica, tile.Possibilitats[p].Pes);
                                #region DEBUG
                                _debug += $"| -A.{cExterior[c1].name} -E.{cDreta[c2].name} -D.{cEsquerra[c3].name} [{orientacioFisica}] ***MATCH***\n";
                                #endregion
                                break;
                            }
                        }
                        #region DEBUG
                        else
                            _debug += $"| -A.{cExterior[c1].name} -E.{cDreta[c2].name} -D.{cEsquerra[c3].name}\n";
                        #endregion
                    }
                }
            }
            #region DEBUG
            if (debug) Debug.Log(_debug);
            #endregion
        }
        return possibilitats;
    }





    static bool TriarPossibilitat(TilePotencial tile, Possibilitats p)
    {
        if (SensePossibiltats(tile, p))
            return true;

        if (UnaPossibilitat(tile, p))
            return true;

        //*^*************************************
        //Nomes tria la preferida si les possibilitats son "totes".
        //*^*************************************
        if (VariesPossibilitats(tile, p))
            return true;

        //if (NoConclusiuBloquejat(tile, p))
        //    return;

        NoConclusiu(tile, p);
        return false;
    }


    static bool SensePossibiltats(TilePotencial tile, Possibilitats p)
    {
        if (p.Count == 0)
        {
            for (int i = 0; i < tile.Veins.Length; i++)
            {


                if (tile.Veins[i] == null)
                    continue;

                tile.Veins[i].Ambiguo();

                //if(tile.Interaccions > 5)
                //{
                if(tile.Veins[i].Interaccions > 5)
                    tile.Veins[i].Veins[1].Ambiguo(true);

                if (tile.Veins[i].Interaccions > 5)
                    tile.Veins[i].Veins[2].Ambiguo(true);
                //}

            }
            if (tile.Interaccions > 5)
                tile.Ambiguo(true);
            //if (tile.Interaccions > 5)
            //Prioritaria = tile;
            //tile.Ambiguo();
            #region DEBUG
            _debug = $"COLISIO! - tile {tile.ID} - ({tile.EstatName}{tile.Coordenades}) - interaccio({tile.Interaccions})\n";
            _debug += $"|-Vei[A]{(tile.Veins[0] != null ? tile.Veins[0].ID : "Null\n")}";
            _debug += $"|-Vei[E]{(tile.Veins[1] != null ? tile.Veins[1].ID : "Null\n")}";
            _debug += $"|-Vei[D]{(tile.Veins[2] != null ? tile.Veins[2].ID : "Null\n")}";
            Debug.Log(_debug);
            #endregion
            tile.HaFallat = true;
            return true;
        }
        else return false;
    }
    static bool UnaPossibilitat(TilePotencial tile, Possibilitats p)
    {
        if (p.Count == 1)
        {
            Debug.LogError("SELECCIONAR TILE ONE");
            tile.Escollir(p, 0);
            #region DEBUG
            if (debug) Debug.Log($"RESULTAT: {p.Tile(0).name}|{p.Orietacio(0)}");
            #endregion
            if (!pecesModificades.Contains(tile.Peça)) 
                pecesModificades.Add(tile.Peça);

            return true;
        }
        else return false;
    }

    static bool VariesPossibilitats(TilePotencial tile, Possibilitats p)
    {
        Possibilitats possibilitats;
        if (tile.Interaccions > 1 && tile.Interaccions <= 2)
        {
            Debug.LogError("SELECCIONAR TILE PREFERIT");

            possibilitats = new Possibilitats(p.Tile(0), 0, 0, p.Tile(0).Pes);
            for (int i = 1; i < p.Count; i++)
            {
                if (p.Tile(i).ConnexionsIgualsA(p.Tile(0)))
                {
                    possibilitats.Add(p.Tile(i), 0,100);
                }
            }

            possibilitats = WFC_Possibilitats.RandomTiles(possibilitats);

            tile.SetPossiblitats(possibilitats);
            #region DEBUG
            if (debug) Debug.Log($"PROVAR PRIMER: {possibilitats.Tile(0).name}|{possibilitats.Orietacio(0)}");
            #endregion
            return true;
        }
        else if (tile.Interaccions > 2)
        {
            Debug.LogError("SELECCIONAR TILE RANDOM");

            possibilitats = WFC_Possibilitats.RandomTiles(p);

            tile.SetPossiblitats(possibilitats);
            #region DEBUG
            if (debug) Debug.Log($"ALEATORI: {possibilitats.Tile(0).name}|{possibilitats.Orietacio(0)}");
            #endregion
            return true;
        }
        else return false;
    }

 

    static void NoConclusiu(TilePotencial tile, Possibilitats p)
    {
        //tile.ResetPossibilitats();
        tile.SetPossiblitats(p);
        Add(tile);
        #region DEBUG
        _debug = $"INTERACCIO({tile.Interaccions})";
        for (int i = 0; i < tile.Possibilitats.Length; i++)
        {
            _debug += $"{tile.Possibilitats[i].name}, ";
        }
        if (debug) Debug.Log(_debug);
        #endregion
    }


    

   

}
*/