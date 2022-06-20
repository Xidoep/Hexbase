using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

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
    static float startTime;

    static bool iniciat = false;
    public static System.Action EnFinalitzar { set => enFinalitzar += value; }

    public static void Process(Peça _peça, System.Action _enFinalitzar, bool _debug = true)
    {
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
        if (pendents == null) pendents = new List<TilePotencial>();
        startTime = Time.realtimeSinceStartup;

        //Prioritaria = _peça.Inicial;
        for (int i = 0; i < _peça.Tiles.Length; i++)
        {
             Add(_peça.Tiles[i]);
        }

        pecesModificades = new List<Peça>() { _peça };

        enFinalitzar = _enFinalitzar;
        debug = _debug;

        IndexMesProvable();
        //Step();
        XS_Coroutine.StartCoroutine_Ending(0.5f, Step);
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
            Debug.LogError("CREAR NOVA PEÇA PERQUE S'HA BLOQUEJAT");
            interaccionsTotals = 0;
            /*for (int p = 0; p < pendents.Count; p++)
            {
                for (int t = 0; t < peçaCreada.Tiles.Length; t++)
                {
                    if(pendents[p] == peçaCreada.Tiles[t])
                    {
                        pendents.RemoveAt(p);
                        p--;
                    }
                }
            }*/
            for (int t = 0; t < peçaCreada.Tiles.Length; t++)
            {
                if(pendents.Contains(peçaCreada.Tiles[t]))
                    pendents.Remove(peçaCreada.Tiles[t]);
            }

            Vector2Int coordenades = peçaCreada.Coordenades;
            Grid grid = peçaCreada.Grid;
            pecesModificades.Remove(peçaCreada);
            MonoBehaviour.Destroy(peçaCreada.gameObject);
            grid.CrearPeçaDesbloquejadora(coordenades);

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
        pecesModificades[0].CrearTilesFisics();
        if(pecesModificades.Count > 1)
        {
            float temps = Mathf.Max(.5f - (Time.realtimeSinceStartup - startTime), 0);
            Debug.Log($"time = {temps}");

            for (int i = 1; i < pecesModificades.Count; i++)
            {
                //XS_Coroutine.StartCoroutine_Ending(temps, pecesModificades[i].CrearTilesFisics);
                pecesModificades[i].CrearTilesFisics();
                temps += .075f;
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
        /*if (TriarPossibilitat(tile, TilesPossibles(tile)))
        {
            IndexMesProvable(tile);
        }
        else
        {
            
            IndexMesProvable();
        }*/

        TriarPossibilitat(tile, TilesPossibles(tile));
        IndexMesProvable(tile);
    }

    static bool Assegurar(TilePotencial tile)
    {
        if (tile.TileFisic == null)
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



        /*if (tile == null)
        {
            _pendents = pendents;
        }
        else
        {
            if(tile.VeinsResolts == 3)
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
                if(_pendents.Count == 0)
                {
                    _pendents = pendents;
                }
            }
          
        }*/


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

            _debug += $"\nAball - from({tile.Veins[0]?.ID})[{cExterior.Length}] = ";
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
                            /*if (!tile.Possibilitats[p].Comprovar(tile, orientacioFisica, cExterior[c1], cEsquerra[c3], cDreta[c2]))
                            {
                                #region DEBUG
                                _debug += $"| -A.{cExterior[c1].name} -E.{cDreta[c2].name} -D.{cEsquerra[c3].name} [{orientacioFisica}] ***MATCH PERO L'ESPECIFIC DEL TILE EM DIU QUE NO...***\n";
                                #endregion
                                continue;
                            }*/
                            
                            if (!possibilitats.Contains(tile.Possibilitats[p]))
                            {
                                possibilitats.Add(tile.Possibilitats[p], orientacioFisica);
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
                /*if (tile.Interaccions <= 5) 
                {
                    if (tile.Veins[i].Peça.acabadaDeCrear)
                        continue;
                }*/

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
            _debug = $"COLISIO! - tile {tile.ID} - ({tile.estatName}{tile.Coordenades}) - interaccio({tile.Interaccions})\n";
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
        //if(!tile.Peça.Estat.TotesLesPossiblitats(p.Count))
        //    return false;

        /*if (p.Contains(tile.Preferit))
        {
            tile.Escollir(p, p.IndexOf(tile.Preferit));
            #region DEBUG
            if (debug) Debug.Log($"RESULTAT: {p.Tile(p.IndexOf(tile.Preferit)).name}|{p.Orietacio(p.IndexOf(tile.Preferit))}");
            #endregion
            if (!pecesModificades.Contains(tile.Peça)) 
                pecesModificades.Add(tile.Peça);

            return true;
        }
        else
        {*/
        Possibilitats possibilitats;
        /*if (p.Count == tile.Possibilitats.Length && !tile.HaFallat)
        {
            
            possibilitats = new Possibilitats(p.Tile(0), p.Tile(0));
            tile.SetPossiblitats(possibilitats);
            #region DEBUG
            if (debug) Debug.Log($"COMPLETAMENT LLIURE: {possibilitats.Tile(0).name}|{possibilitats.Orietacio(0)}");
            #endregion
            return true;
        }*/
        //else if (tile.Interaccions > 6 && colisions >= 2)
        if (tile.Interaccions > 2 && tile.Interaccions <= 4)
        {
            possibilitats = new Possibilitats(p.Tile(0), p.Tile(0));
            tile.SetPossiblitats(possibilitats);
            #region DEBUG
            if (debug) Debug.Log($"PROVAR PRIMER: {possibilitats.Tile(0).name}|{possibilitats.Orietacio(0)}");
            #endregion
            return true;
        }
        else if (tile.Interaccions > 4)
        {
            int random = Random.Range(0, p.Count);
            possibilitats = new Possibilitats(p.Tile(random), p.Tile(random));
            tile.SetPossiblitats(possibilitats);
            #region DEBUG
            if (debug) Debug.Log($"ALEATORI: {possibilitats.Tile(0).name}|{possibilitats.Orietacio(0)}");
            #endregion
            return true;
        }
        else return false;  


        /*tile.Ambiguo();
        for (int i = 0; i < tile.Veins.Length; i++)
        {
            tile.Veins[i]?.Ambiguo();
            tile.Veins[i]?.Veins[1]?.Ambiguo();
            tile.Veins[i]?.Veins[2]?.Ambiguo();
        }*/
        //}
        //return false;
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


    public struct Possibilitats
    {
        public Possibilitats(List<Possibilitat> possibilitats)
        {
            this.possibilitats = possibilitats;
            contains = false;
            index = 0;
        }
        public Possibilitats(Tile tile, int orientacio)
        {
            possibilitats = new List<Possibilitat>() { new Possibilitat(tile, orientacio) };
            contains = false;
            index = 0;
        }
        public Possibilitats(Tile tile1, Tile tile2)
        {
            possibilitats = new List<Possibilitat>();
            possibilitats.Add(new Possibilitat(tile1, 0));
            possibilitats.Add(new Possibilitat(tile2, 0));
            contains = false;
            index = 0;
        }


        List<Possibilitat> possibilitats;

        //INTERN
        bool contains;
        int index;



        //FUNCIONS
        public int Count => possibilitats.Count;
        public Tile Tile(int index) => possibilitats[index].tile;
        public int Orietacio(int index) => possibilitats[index].orientacio;
        public void Add(Tile tile, int orientacio) => possibilitats.Add(new Possibilitat(tile, orientacio));
        public bool Contains(Tile tile)
        {
            contains = false;
            for (int i = 0; i < possibilitats.Count; i++)
            {
                if (possibilitats[i].EsIgual(tile)) contains = true;
            }
            return contains;
        }
        public int IndexOf(Tile tile)
        {
            index = 0;
            for (int i = 0; i < possibilitats.Count; i++)
            {
                if (possibilitats[i].EsIgual(tile)) 
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public Tile[] ToArray()
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < possibilitats.Count; i++)
            {
                tiles.Add(possibilitats[i].tile);
            }
            return tiles.ToArray();
        }
    }
    public struct Possibilitat
    {
        public Possibilitat(Tile tile, int orientacio)
        {
            this.tile = tile;
            this.orientacio = orientacio;
        }
        public Tile tile;
        public int orientacio;



        public bool EsIgual(Tile tile) => this.tile == tile;
    }


}
