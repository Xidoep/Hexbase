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
    [SerializeField] InputActionReference step;
    [SerializeField] InputActionReference propagar;
    [SerializeField] WfcRegla[] regles;
    //[SerializeField] Fase_Colocar colocar;
    [SerializeField] List<TilePotencial> pendents;
    //private void OnEnable() => input.OnPerformedAdd(StepManual);
    //private void OnDisable() => input.OnPerformedRemove(StepManual);

    [SerializeField] List<TilePotencial> propagables;

    System.Action enFinalitzar;

    private void OnEnable()
    {
        step.action.Enable();
        step.OnPerformedAdd(StepManual);
        propagar.action.Enable();
        propagar.OnPerformedAdd(PropagarManual);
    }
    private void OnDisable()
    {
        step.OnPerformedRemove(StepManual);
        step.action.Disable();
        propagar.OnPerformedRemove(PropagarManual);
        propagar.action.Disable();

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
        AgafarTilesPeça(peça);
        AgafarTilesVeins(peça);
        AgafarTilesCanviadesSiNecessari(canviades);

        //Propacació inial on mirem les possiblitats de totes les peces.
        Iniciar_Propagacio(pendents);
    }


    void AgafarTilesPeça(Peça peça)
    {
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            peça.Tiles[i].Ambiguo();
        }
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            pendents.Add(peça.Tiles[i]);
        }
    }
    void AgafarTilesVeins(Peça peça)
    {
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (peça.Tiles[i].Veins[0] == null)
                continue;

            peça.Tiles[i].Veins[0].Ambiguo();
            peça.Tiles[i].Veins[0].Veins[0] = peça.Tiles[i];
            peça.Tiles[i].Veins[0].Veins[1].Ambiguo();
            peça.Tiles[i].Veins[0].Veins[2].Ambiguo();
        }
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
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
            for (int t = 0; t < peces[p].Tiles.Length; t++)
            {
                peces[p].Tiles[t].Ambiguo();

                if (peces[p].Tiles[t].Veins[0] == null)
                    continue;

                peces[p].Tiles[t].Veins[0].Ambiguo();
                peces[p].Tiles[t].Veins[0].Veins[0] = peces[p].Tiles[t];
                peces[p].Tiles[t].Veins[0].Veins[1].Ambiguo();
                peces[p].Tiles[t].Veins[0].Veins[2].Ambiguo();
            }
        }

        for (int p = 0; p < peces.Count; p++)
        {
            for (int t = 0; t < peces[p].Tiles.Length; t++)
            {
                 pendents.Add(peces[p].Tiles[t]);

                if (peces[p].Tiles[t].Veins[0] == null)
                    continue;

                pendents.Add(peces[p].Tiles[t].Veins[0]);
                pendents.Add(peces[p].Tiles[t].Veins[0].Veins[1]);
                pendents.Add(peces[p].Tiles[t].Veins[0].Veins[2]);
            }
        }
    }





    //MAIN STEPS
    void StepManual(InputAction.CallbackContext context) => Step();
    void PropagarManual(InputAction.CallbackContext context) => Propagar();
    void Step()
    {
        TilePotencial actual = TileWithTheLowestEntropy();

        pendents.Remove(actual);
        Debugar.Log($"WFC step (tries to solve {actual.EstatName})");
        actual.Escollir();
        //RandomTile(actual);

        

        if (pendents.Count == 0)
        {
            bool reglesAprovades = true;
            for (int r = 0; r < regles.Length; r++)
            {
                if (!regles[r].Comprovar(colocada))
                {
                    Debugar.LogError($"REESTART!!! Per que la regla {r}, no s'ha aprovat.", colocada);
                    //NetejarTilesFisicsAlReiniciar();
                    Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                    return;
                }

                for (int c = 0; c < canviades.Count; c++)
                {
                    if (!regles[r].Comprovar(canviades[c]))
                    {
                        Debugar.LogError($"REESTART!!! Per que la regla {r}, no s'ha aprovat.", canviades[c]);
                        //NetejarTilesFisicsAlReiniciar();
                        Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                        return;
                    }
                }
                for (int v = 0; v < colocada.VeinsPeça.Count; v++)
                {
                    if (!regles[r].Comprovar(colocada.VeinsPeça[v]))
                    {
                        Debugar.LogError($"REESTART!!! Per que la regla {r}, no s'ha aprovat.", colocada.VeinsPeça[v]);
                        //NetejarTilesFisicsAlReiniciar();
                        Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                        return;
                    }
                }
            }
            
            

            Finalitzar();

            //colocar.Iniciar();
        }
        else
        {
            Iniciar_Propagacio(actual);
        }
    }


    TilePotencial TileWithTheLowestEntropy()
    {
        List<TilePotencial> lowestEntropy = new List<TilePotencial>();
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
            //En cas que no hi hagui cap element, posen un.
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
    void Propagar()
    {
        //TilePotencial actual = propagables[0];
        if(propagables.Count > 0)
        {
            if (TreuPossibilitatsImpossibles(propagables[0]))
            {
                if(propagables[0].PossibilitatsVirtuals.Count == 0)
                {
                    string _debug = $"COLISIO ({propagables[0].EstatName}.{propagables[0].Orientacio})!!!\n";
                    Connexio[] connexios;

                    _debug += "Connexions exteriors = ";
                    connexios = GetConnexiosVirtuals(propagables[0], propagables[0].Veins[0], 0);
                    for (int i = 0; i < connexios.Length; i++)
                    {
                        _debug += connexios[i].name;
                        _debug += i != (connexios.Length - 1) ? ", " : "\n";
                    }

                    _debug += "Connexions esquerra = ";
                    connexios = GetConnexiosVirtuals(propagables[0], propagables[0].Veins[1], 2);
                    for (int i = 0; i < connexios.Length; i++)
                    {
                        _debug += connexios[i].name;
                        _debug += i != (connexios.Length - 1) ? ", " : "\n";
                    }

                    _debug += "Connexions dreta = ";
                    connexios = GetConnexiosVirtuals(propagables[0], propagables[0].Veins[2], 1);
                    for (int i = 0; i < connexios.Length; i++)
                    {
                        _debug += connexios[i].name;
                        _debug += i != (connexios.Length - 1) ? ", " : "\n";
                    }

                    Debugar.LogError(_debug, propagables[0].Peça);
                    Iniciar_WFC(colocada, canviades, enFinalitzar, true);
                    return;
                }

                for (int i = 0; i < propagables[0].Veins.Length; i++)
                {
                    if (!propagables.Contains(propagables[0].Veins[i]))
                    {
                        if (propagables[0].Veins[i] != null)
                        {
                            //Debug.Log($"Propagar a index {i}");
                            //Debug.Log($"Propagar a {propagables[0].Veins[i].EstatName}");
                            if (propagables[0].Veins[i].Resolt)
                                continue;

                            propagables.Add(propagables[0].Veins[i]);
                        }
                    }
                }
            }
            propagables.RemoveAt(0);

            XS_Coroutine.StartCoroutine_Ending(0.001f, Propagar);
            return;
        }

        //Debug.LogError("PROPAGACIO ACABADA!");
        XS_Coroutine.StartCoroutine_Ending(0.001f, Step);
    }



    bool TreuPossibilitatsImpossibles(TilePotencial tile)
    {
        string _debug = "";
        bool haCanviat = false;

        Connexio[] cExterior = GetConnexiosVirtuals(tile, tile.Veins[0], 0);
        Connexio[] cEsquerra = GetConnexiosVirtuals(tile, tile.Veins[1], 2);
        Connexio[] cDreta = GetConnexiosVirtuals(tile, tile.Veins[2], 1);

        /*_debug = "";
        _debug += $"{tile.ID} Connexions:";

        _debug += $"\nExterior - from({tile.Veins[0]?.ID})[{cExterior.Length}] = ";
        for (int i = 0; i < cExterior.Length; i++) { _debug += $"{cExterior[i].name}, "; }
        _debug += $"\nEsquerra - from({tile.Veins[1]?.ID})[{cEsquerra.Length}] = ";
        for (int i = 0; i < cEsquerra.Length; i++) { _debug += $"{cEsquerra[i].name}, "; }
        _debug += $"\nDreta - from({tile.Veins[2]?.ID})[{cDreta.Length}] = ";
        for (int i = 0; i < cDreta.Length; i++) { _debug += $"{cDreta[i].name}, "; }

        Debug.Log(_debug);
        */
        /*_debug = "";
        for (int p = 0; p < tile.PossibilitatsVirtuals.Count; p++)
        {
            Possibilitat pos = tile.PossibilitatsVirtuals.Get(p);
            _debug += $"{pos.tile.name} - Connexions = " +
                $"(A.{pos.tile.Exterior(pos.orientacio).name} " +
                $"-E.{pos.tile.Esquerra(pos.orientacio).name} " +
                $"-D.{pos.tile.Dreta(pos.orientacio).name})...\n";
        }
        Debug.Log(_debug);*/

        List<Possibilitat> toRemove = new List<Possibilitat>();
        for (int p = 0; p < tile.PossibilitatsVirtuals.Count; p++)
        {
            Possibilitat pos = tile.PossibilitatsVirtuals.Get(p);
            //_debug = $"{pos.tile.name}\n";
            bool found = false;
            for (int c1 = 0; c1 < cExterior.Length; c1++)
            {
                for (int c3 = 0; c3 < cEsquerra.Length; c3++)
                {
                    for (int c2 = 0; c2 < cDreta.Length; c2++)
                    {
                        //_debug += $"|-{pos.tile.Exterior(pos.orientacio).name}, {pos.tile.Esquerra(pos.orientacio).name}, {pos.tile.Dreta(pos.orientacio).name} = " +
                        //    $"{cExterior[c1].name}, {cEsquerra[c3].name}, {cDreta[c2].name} ?";
                        if (tile.CompararConnexions(pos, cExterior[c1], cEsquerra[c3], cDreta[c2])) 
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
                toRemove.Add(pos);
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
        List<Connexio> _tmp = new List<Connexio>();

        //Debug.Log($"vei = {vei}");
        if (vei != null)
        {
            if (vei.Resolt)
            {
                return new Connexio[] { vei.GetConnexions(vei.PossibilitatsVirtuals.Get(0))[veiContrari] };
            }
            else
            {
                _tmp.Clear();
                if (tile.Peça.Subestat.connexioEspesifica != null && tile.Peça.Subestat.connexioEspesifica.subestats.Contains(vei.Peça.Subestat))
                {
                    _tmp.AddRange(tile.Peça.Subestat.connexioEspesifica.connexions);
                    return _tmp.ToArray();
                }

                
                for (int p = 0; p < vei.PossibilitatsVirtuals.Count; p++)
                {
                    if (!_tmp.Contains(vei.PossibilitatsVirtuals.Tile(p).Exterior(vei.PossibilitatsVirtuals.Orietacio(p))))
                        _tmp.Add(vei.PossibilitatsVirtuals.Tile(p).Exterior(vei.PossibilitatsVirtuals.Orietacio(p)));

                    if (!_tmp.Contains(vei.PossibilitatsVirtuals.Tile(p).Esquerra(vei.PossibilitatsVirtuals.Orietacio(p))))
                        _tmp.Add(vei.PossibilitatsVirtuals.Tile(p).Esquerra(vei.PossibilitatsVirtuals.Orietacio(p)));

                    if (!_tmp.Contains(vei.PossibilitatsVirtuals.Tile(p).Dreta(vei.PossibilitatsVirtuals.Orietacio(p))))
                        _tmp.Add(vei.PossibilitatsVirtuals.Tile(p).Dreta(vei.PossibilitatsVirtuals.Orietacio(p)));
                }
                return _tmp.ToArray();

            }
        }
        else
        {
            //Connexio[] especifics = tile.ConnexionsNules.Invoke(tile);
            //return especifics == null ? tile.Connexions : especifics;
            //return tile.Connexions;
            //Si el sub no te connexions nules, agafa les possibles del sub si n'hi ha, i si no

            /*
            if (tile.Peça.Subestat.TeConnexionsNules)
                return tile.Peça.Subestat.ConnexionsNules;
            else return tile.Peça.Subestat.ConnexionsPossibles;
            */
            /*if (tile.Peça.Subestat.TeConnexionsNules)
                return tile.Peça.Subestat.ConnexionsNules;
            else return tile.Peça.Estat.ConnexionsPossibles;*/
            if (tile.Peça.Subestat.TeConnexionsNules)
                return tile.Peça.Subestat.ConnexionsNules;
            return tile.Peça.Subestat.ConnexionsPossibles;
        }
        //return _tmp.ToArray();
    }





    void Finalitzar()
    {
        Debugar.LogError("WFC ACABAT!");

        colocada.CrearTilesFisics();
        for (int i = 0; i < colocada.VeinsPeça.Count; i++)
        {
            colocada.VeinsPeça[i].CrearTilesFisics();
        }

        for (int i = 0; i < canviades.Count; i++)
        {
            canviades[i].CrearTilesFisics();
        }

        enFinalitzar.Invoke();
    }



}

