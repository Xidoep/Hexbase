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

    EstatColocable eTrobat;
    Subestat sTrobat;
    Producte pTrobat;
    Tile tTrobat;
    [SerializeField] bool nomesGuardats;

    System.Action<Peça> enColocar;
    public System.Action<Peça> EnColocar { get => enColocar; set => enColocar = value; }

    private void OnEnable()
    {
        nomesGuardats = false;
    }

    private void OnDisable()
    {
        nomesGuardats = false;
    }



    //ARCHIUS (NOU - BORRAR)
    public int FilesLength => files.Count;
    public int Actual { get => actual; }
    public void NouArxiu(Mode mode)
    {
        files.Add(new SavedFile());
        partidaAnterior = actual;
        actual = files.Count - 1;
        files[actual].SetMode(mode);
    }
    public void BorrarPartida()
    {
        files.RemoveAt(actual);
        if (files.Count == 0) files.Add(new SavedFile());
        actual = Mathf.Clamp(actual - 1, 0, files.Count - 1);
    }



    //GUARDA - CARREGAR
    public bool NomesGuardats => nomesGuardats;
    public bool HiHaPartidaAnterior => partidaAnterior != -1;
    public void SetActual(int actual) => this.actual = actual;
    public void Continuar(Grups grups, Fase seguent, Modes modes)
    {
        BorrarPartida();
        actual = partidaAnterior;
        modes.Set((Mode)files[actual].Mode);
        //files[actual].SetMode((Mode)files[actual].Mode);
        Load(grups, seguent);
    }
    public void Load(int index, Grups grups, Fase seguent, System.Action enCarregat = null)
    {
        actual = index;
        Load(grups, seguent, enCarregat);
    }
    public void Load(Grups grups, Fase seguent, System.Action enCarregat = null) => files[actual].Load(grups, seguent, EstatNomToPrefab, SubestatNomToPrefab, ProducteNomToPrefab, TileNomToPrefab, enColocar, enCarregat);
    EstatColocable EstatNomToPrefab(string nom)
    {
        eTrobat = null;
        for (int i = 0; i < Referencies.Instance.Estats.Length; i++)
        {
            if (Referencies.Instance.Estats[i].name == nom)
            {
                eTrobat = Referencies.Instance.Estats[i];
                break;
            }
        }
        return eTrobat;
    }
    Subestat SubestatNomToPrefab(string nom)
    {
        sTrobat = null;
        for (int i = 0; i < Referencies.Instance.Subestats.Length; i++)
        {
            if (Referencies.Instance.Subestats[i].name == nom)
            {
                sTrobat = Referencies.Instance.Subestats[i];
                break;
            }
        }
        return sTrobat;
    }
    Producte ProducteNomToPrefab(string nom)
    {
        pTrobat = null;
        for (int i = 0; i < Referencies.Instance.Productes.Length; i++)
        {
            if (Referencies.Instance.Productes[i].name == nom)
            {
                pTrobat = Referencies.Instance.Productes[i];
                break;
            }
        }
        return pTrobat;
    }
    Tile TileNomToPrefab(string nom)
    {
        tTrobat = null;
        for (int i = 0; i < Referencies.Instance.Tiles.Length; i++)
        {
            if (Referencies.Instance.Tiles[i].name == nom)
            {
                tTrobat = Referencies.Instance.Tiles[i];
                break;
            }
        }
        return tTrobat;
    }


    
    //PECES
    public bool TePeces 
    {
        get 
        {
            if (files == null || files.Count == 0) files = new List<SavedFile>() {new SavedFile() };
            if (actual > files.Count - 1) actual = 0;
            return files[actual].TePeces;
        
        }
    }
    public void Add(Peça peça, Grups grups) => files[actual].Add(peça, grups);
    public void Actualitzar(List<Peça> peçes, Grups grups) => files[actual].Actualitzar(peçes, grups);
    


    //EXPERIENCIA
    public int Experiencia(int index) => files[index].Experiencia;
    public void GuardarExperiencia(int experiencia) => files[actual].SetExperiencia(experiencia);
    public void guardarNivell(int nivell) => files[actual].SetNivell(nivell);


    //MODES
    public void SetMode(Mode mode) => files[actual].SetMode(mode);
    public int Mode => files[actual].Mode; 



    //CAPTURES
    public bool TeCaptures => files[actual].Captures != null && files[actual].Captures.Count > 0;
    public string GetCapturaMesRecent(int index) 
    {
        if (files[index].Captures.Count > 0)
            return files[index].Captures[files[index].Captures.Count - 1];
        else return "";
    } 
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
    public string[] Paths
    {
        get
        {
            List<string> captures = new List<string>();
            for (int f = 0; f < files.Count; f++)
            {
                if (files[f].Captures.Count == 0)
                    continue;

                captures.AddRange(files[f].Captures);
            }

            return captures.ToArray();
        }
    }
    public void AddCaptura(string path) => files[actual].AddCaptura(path);
    public void RemoveCaptura(int index, string path)
    {
        if (index == -1)
            return;

        if(files[index].Captures.Contains(path))
            files[index].Captures.Remove(path);
    }
    


    //PILA
    public bool HiHaAlgunaPeça => files[actual].PilaPlena;
    public List<EstatColocable> Pila => files[actual].Pila(EstatNomToPrefab);
    public void AddToPila(EstatColocable estat) => files[actual].AddPila(estat);
    public void RemoveLastFromPila() => files[actual].RemoveLastPila();



    //UI
    public void MostrarNomesPartidesGuardades(bool nomesGuardats) => this.nomesGuardats = nomesGuardats;


}