using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Save")]
public class SaveHex : ScriptableObject
{
    public const string KEY_BROMA_SORTIR_VISTA = "BromaSortirVista";
    public const string KEY_SEGONA_PARTIDA = "SegonaPartida";

    [SerializeField] int partidaAnterior = -1;
    [SerializeField] int actual = 0;
    [SerializeField] List<SavedFile> files;
    [SerializeField] CapturarPantalla capturarPantalla;

    [Apartat("ESTATS")]
    [SerializeField] Estat[] estats;
    [Header("SUBESTATS")]
    [SerializeField] Subestat[] subestats;
    [Header("PRODUCTES")]
    [SerializeField] Producte[] productes;
    [Header("TILES")]
    [SerializeField] Tile[] tiles;

    Estat eTrobat;
    Subestat sTrobat;
    Producte pTrobat;
    Tile tTrobat;
    [SerializeField] bool nomesGuardats;

    public int FilesLength => files.Count;

    private void OnEnable()
    {
        capturarPantalla.OnCapturatRegistrar(AddCaptura);
        nomesGuardats = false;
    }

    private void OnDisable()
    {
        capturarPantalla.OnCapturatDesregistrar(AddCaptura);
        nomesGuardats = false;
    }

    public bool HiHaPartidaAnterior => partidaAnterior != -1; 

    //GETTERS
    public bool NomesGuardats => nomesGuardats;
    public int Mode => files[actual].Mode; 
    public bool TePeces 
    {
        get 
        {
            if (files == null || files.Count == 0) files = new List<SavedFile>() {new SavedFile() };
            if (actual > files.Count - 1) actual = 0;
            return files[actual].TePeces;
        
        }
    }
    public bool TeCaptures => files[actual].Captures != null && files[actual].Captures.Count > 0;
    public int Experiencia(int index) => files[index].Experiencia;
    public int CapturaToIndex(string path)
    {
        //Debugar.Log($"Existeix {path}?");
        int trobat = -1;
        for (int f = 0; f < files.Count; f++)
        {
            if(files[f].Captures != null && files[f].Captures.Count > 0)
            {
                for (int c = 0; c < files[f].Captures.Count; c++)
                {
                    //Debugar.Log($"{files[f].Captures[c]} =? {path}");
                    if (files[f].Captures[c] == path || files[f].Captures[c] == path.Replace(@"\", "/"))
                    {
                        trobat = f;
                        break;
                    }
                }
                if (trobat != -1)
                    break;
            }
            
        }
        return trobat;
    }
    public string GetCapturaMesRecent(int index) 
    {
        if (files[index].Captures.Count > 0)
            return files[index].Captures[files[index].Captures.Count - 1];
        else return "";
    } 
    public bool PilaPlena => files[actual].PilaPlena;
    public List<Estat> Pila => files[actual].Pila(EstatNomToPrefab);
    Estat EstatNomToPrefab(string nom)
    {
        eTrobat = null;
        for (int i = 0; i < estats.Length; i++)
        {
            if (estats[i].name == nom)
            {
                eTrobat = estats[i];
                break;
            }
        }
        return eTrobat;
    }
    Subestat SubestatNomToPrefab(string nom)
    {
        sTrobat = null;
        for (int i = 0; i < subestats.Length; i++)
        {
            if (subestats[i].name == nom)
            {
                sTrobat = subestats[i];
                break;
            }
        }
        return sTrobat;
    }
    public Producte ProducteNomToPrefab(string nom)
    {
        pTrobat = null;
        for (int i = 0; i < productes.Length; i++)
        {
            if (productes[i].name == nom)
            {
                pTrobat = productes[i];
                break;
            }
        }
        return pTrobat;
    }
    Tile TileNomToPrefab(string nom)
    {
        tTrobat = null;
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].name == nom)
            {
                tTrobat = tiles[i];
                break;
            }
        }
        return tTrobat;
    }


    //SETTERS / FUNCIONS
    public void MostrarNomesPartidesGuardades(bool nomesGuardats) => this.nomesGuardats = nomesGuardats;
    public void NouArxiu(Mode mode)
    {
        files.Add(new SavedFile());
        partidaAnterior = actual;
        actual = files.Count - 1;
        files[actual].SetMode(mode);
    }
    public void Continuar(Grups grups, Fase seguent, Modes modes)
    {
        BorrarPartida();
        actual = partidaAnterior;
        modes.Set((Mode)files[actual].Mode);
        //files[actual].SetMode((Mode)files[actual].Mode);
        Load(grups, seguent);
    }
    public void BorrarPartida()
    {
        files.RemoveAt(actual);
        if (files.Count == 0) files.Add(new SavedFile());
        actual = Mathf.Clamp(actual - 1, 0, files.Count - 1);
    }
    public void SetMode(Mode mode) => files[actual].SetMode(mode);
    public void Add(Peça peça, Grups grups) => files[actual].Add(peça, grups);
    public void Actualitzar(List<Peça> peçes, Grups grups) => files[actual].Actualitzar(peçes, grups);
    public void ActualitzarExperiencia(int experiencia, int nivell) => files[actual].SetExperienciaNivell(experiencia, nivell);
    public void AddCaptura(string path) => files[actual].AddCaptura(path);
    public void RemoveCaptura(int index, string path)
    {
        if (index == -1)
            return;

        if(files[index].Captures.Contains(path))
            files[index].Captures.Remove(path);
    }
    public void AddToPila(Estat estat) => files[actual].AddPila(estat);
    public void RemoveLastFromPila() => files[actual].RemoveLastPila();



    public void Load(int index, Grups grups, Fase colocar)
    {
        actual = index;
        Load(grups, colocar);
    }
    public void Load(Grups grups, Fase seguent) => files[actual].Load(grups, seguent, EstatNomToPrefab, SubestatNomToPrefab, ProducteNomToPrefab, TileNomToPrefab);






    private void OnValidate()
    {
        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        subestats = XS_Editor.LoadAllAssetsAtPath<Subestat>("Assets/XidoStudio/Hexbase/Peces/Subestats").ToArray();
        productes = XS_Editor.LoadAllAssetsAtPath<Producte>("Assets/XidoStudio/Hexbase/Peces/Productes").ToArray();
        tiles = XS_Editor.LoadAllAssetsAtPathAndSubFolders<Tile>("Assets/XidoStudio/Hexbase/Peces/Tiles/Tiles").ToArray();
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");
    }
}