using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Save")]
public class SaveHex : ScriptableObject
{
    [SerializeField] int current = 0;
    [SerializeField] List<SavedFile> files;
    [SerializeField] CapturarPantalla capturarPantalla;

    [Apartat("ESTATS")]
    [SerializeField] Estat[] estats;
    [Header("SUBESTATS")]
    [SerializeField] Subestat[] subestats;
    [Header("PRODUCTES")]
    [SerializeField] Producte[] productes;

    Estat eTrobat;
    Subestat sTrobat;
    Producte pTrobat;
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

    public void MostrarNomesPartidesGuardades(bool nomesGuardats) => this.nomesGuardats = nomesGuardats;
    public bool NomesGuardats => nomesGuardats;

    //public List<SavedFile> Files => files;
    public void NouArxiu(Mode mode)
    {
        files.Add(new SavedFile());
        current = files.Count - 1;
        files[current].SetMode(mode);
    }
    public void BorrarPartida()
    {
        files.RemoveAt(current);
        if (files.Count == 0) files.Add(new SavedFile());
        current = Mathf.Clamp(current - 1, 0, files.Count - 1);
    }

    public int Mode => files[current].Mode;
    public bool TePeces 
    {
        get 
        {
            if (files == null || files.Count == 0) files = new List<SavedFile>() {new SavedFile() };
            if (current > files.Count - 1) current = 0;
            return files[current].TePeces;
        
        }
    }
    public bool TeCaptures => files[current].Captures != null && files[current].Captures.Count > 0;

    public void Add(Pe�a pe�a, Grups grups) => files[current].Add(pe�a, grups);

    public void Actualitzar(List<Pe�a> pe�es, Grups grups) => files[current].Actualitzar(pe�es, grups);
    public void ActualitzarExperiencia(int experiencia, int nivell) => files[current].SetExperienciaNivell(experiencia, nivell);
    public void Load(Grups grups, Fase seguent) => files[current].Load(grups, seguent, EstatNomToPrefab, SubestatNomToPrefab, ProducteNomToPrefab);

    public void Load(int index, Grups grups, Fase colocar)
    {
        current = index;
        Load(grups, colocar);
    }
    public void AddCaptura(string path) => files[current].AddCaptura(path);

    public void RemoveCaptura(int index, string path)
    {
        if (index == -1)
            return;

        if(files[index].Captures.Contains(path))
            files[index].Captures.Remove(path);
    }
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

    public void AddToPila(Estat estat) => files[current].AddPila(estat);
    public void RemoveLastFromPila() => files[current].RemoveLastPila();
    public bool PilaPlena => files[current].PilaPlena;
    public List<Estat> Pila => files[current].Pila(EstatNomToPrefab);

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

    private void OnValidate()
    {
        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        subestats = XS_Editor.LoadAllAssetsAtPath<Subestat>("Assets/XidoStudio/Hexbase/Peces/Subestats").ToArray();
        productes = XS_Editor.LoadAllAssetsAtPath<Producte>("Assets/XidoStudio/Hexbase/Peces/Productes").ToArray();
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");
    }
}