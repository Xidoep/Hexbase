using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Xido Studio/Hex/Unpack/Estats")]
public class EstatsUnpack : ScriptableObject
{
    const char SEPARADOR = ',';
    const string TRUE = "TRUE";
    const string CONNECTAT = "connectat";
    const string PRODUCTES = "Productes";
    const string CASA = "Casa";
    const string PRODUCTOR = "Productor";
    const string EXTRACCIO = "Extraccio";
    const string CONNECTOR = "Connector";
    const string MAR = "Mar";
    const string NO_IMPORTA = "No Importa";

    [BoxGroup("EXTERNAL FILES", centerLabel: true)]
    [SerializeField] Object csv;
    
    [BoxGroup("EXTERNAL FILES", centerLabel: true)]
    [SerializeField] Object tiles;

    [BoxGroup("PATHS", centerLabel: true), FolderPath, SerializeField] 
    string outputPath, outputProductes, outputReceptes, outputDetalls;


    [BoxGroup("NEXT STEPS", centerLabel: true), SerializeField] 
    TilesetUnpack tilesetUnpack;

    [Space(20), ReadOnly, SerializeField]
    Referencies referencies;

    //INTERN
    //[OnInspectorInit("GetEstats"), PropertyOrder(-1), SerializeField] 
    //Dictionary<string, bool> estats;

    string[] linies;
    string[] columnes;
    Object[] subobjects;
    int liniaProductes;
    List<string> llistaProductes;
    bool viable;
    bool confirmat;
    int trobat;
    string lastNameFounded;
    Recepta recepta;
    Peça.ConnexioEnum cPropia;
    List<ScriptableObject> inputs;
    Peça.ConnexioEnum cInputs;
    List<ScriptableObject> outputs;
    Peça.ConnexioEnum accioConnectar;
    string nomRecepta;
    Estat assignar;



    [TableList(), SerializeField, PropertyOrder(-1)]
    Confirmacio[] confirmacions;

    [SerializeField, PropertyOrder(-2), HorizontalGroup("opcions")]
    bool detalls, reimportForçat;


    Confirmacio GetConfirmacio(string nom)
    {
        trobat = -1;
        for (int i = 0; i < confirmacions.Length; i++)
        {
            if (confirmacions[i].Nom.Equals(nom))
            {
                trobat = i;
                break;
            }
        }
        return confirmacions[trobat];
    }
    bool ConfirmarTiles(Confirmacio[] confirmacions, string nom)
    {
        confirmat = false;
        for (int i = 0; i < confirmacions.Length; i++)
        {
            if (confirmacions[i].Nom.Equals(nom))
            {
                confirmat = confirmacions[i].Tiles;
                break;
            }
        }
        return confirmat;
    }
    bool ConfirmarRecepta(Confirmacio[] confirmacions, string from)
    {
        confirmat = false;
        if(from == "")
        {
            Debug.Log($"La recepta potser és una producciò, així que intenta crear-la...");
            return true;
        }
        for (int i = 0; i < confirmacions.Length; i++)
        {
            if (confirmacions[i].Nom.Equals(from))
            {
                confirmat = confirmacions[i].Recepta;
                break;
            }
        }
        return confirmat;
    }
    bool ConfirmarEstat(Confirmacio[] confirmacions, string nom)
    {
        confirmat = false;
        for (int i = 0; i < confirmacions.Length; i++)
        {
            if (confirmacions[i].Nom.Equals(nom))
            {
                confirmat = confirmacions[i].Estat;
                break;
            }
        }
        return confirmat;
    }
    



    [PropertyOrder(-3), Button(ButtonSizes.Large, Icon = SdfIconType.Archive, IconAlignment = IconAlignment.LeftOfText)]
    void Unpack() => Unpack(confirmacions, false);
    void Unpack(Confirmacio[] confirmacions) => Unpack(confirmacions, true);

    void Unpack(Confirmacio[] confirmacions, bool allReceptes)
    {
        //Crear linies i columnes
        linies = System.IO.File.ReadAllLines(AssetDatabase.GetAssetPath(csv));

        for (int i = 3; i < linies.Length; i++)
        {
            //if (!LiniaActiva(i))
            //    continue;

            if (IniciProductes(i))
                break;

            GetColumnes(i);
        }

        CreaProductes();

        //GET MESHES
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        CrearEstats(confirmacions);

        referencies.Refresh();

        CrearReceptes(allReceptes ? this.confirmacions : confirmacions);
    }

    private void CrearReceptes(Confirmacio[] confirmacions)
    {
        lastNameFounded = "";
        for (int i = 3; i < linies.Length; i++)
        {
            if (i.Equals(liniaProductes))
                break;

            GetColumnes(i);

            if (!HiHaInput(1) && !HiHaOutput(1))
            {
                Debug.Log($"Recepta[{i + 1}] NO te ni INPUTS ni OUTPUTS...");
                continue;
            }

            viable = true;

            if (TeNom) lastNameFounded = Nom;

            if (!ConfirmarRecepta(confirmacions, From))
            {
                Debug.Log($"Recepta[{i + 1}] {From} no està preparat...");
                continue;
            }

            recepta = CreateInstance<Recepta>();

            //PROPIA
            cPropia = GetConnexio(HiHaConnexioPropia, ConnexioPropia);

            //INPUTS
            inputs = new List<ScriptableObject>();
            for (int input = 1; input < 7; input++)
            {
                if (!HiHaInput(input))
                    continue;

                if (!referencies.EstatsContains(Input(input)))
                {
                    viable = false;
                    Debug.LogError($"Recepta[{i+1}] NO es pot COMPLETAR! - Inputs => {Input(input)} que no s'ha importat!!!");
                    break;
                }
                inputs.Add(referencies.GetEstat(Input(input)));
            }
            cInputs = GetConnexio(HiHaConnexioInputs, ConnexioInputs);

            //OUTPUTS
            outputs = new List<ScriptableObject>();
            if (HiHaFrom)
            {
                if (!referencies.EstatsContains(From))
                {
                    Debug.LogError($"Recepta[{i+1}] NO es pot COMPLETAR! - Outputs => {From} no s'ha importat!!!");
                    viable = false;
                    continue;
                }
                if (!referencies.EstatsContains(lastNameFounded))
                {
                    Debug.LogError($"Recepta[{i + 1}] NO es pot COMPLETAR! - To => {lastNameFounded} no s'ha importat!!!");
                    viable = false;
                    continue;
                }
                /*Estat to = referencies.GetEstat(lastNameFounded);
                if(to == null)
                {
                    Debug.LogError($"Recepta[{i + 1}] NO es pot COMPLETAR! - To => {lastNameFounded} no s'ha importat!!!");
                    viable = false;
                    continue;
                }*/
                outputs.Add(referencies.GetEstat(lastNameFounded));
            }

            for (int output = 1; output < 7; output++)
            {
                if (!HiHaOutput(output))
                    continue;

                if (!referencies.ProductesContains(Output(output)) && !referencies.GuanyarExperienciaContains(Output(output)))
                {
                    viable = false;
                    Debug.LogError($"Recepta[{i+1}] NO es pot COMPLETAR! Outputs => {Output(output)} no s'ha importat!!!");
                    continue;
                }
                if (referencies.ProductesContains(Output(output)))
                    outputs.Add(referencies.GetProducte(Output(output)));
                else if(referencies.GuanyarExperienciaContains(Output(output)))
                    outputs.Add(referencies.GetGuanyarExperiencia(Output(output)));
            }
            accioConnectar = GetConnexio(HiHaAccioConnectar, AccioConnectar);

            Debug.Log("FALTA: Que es contemplin els modificadors com: per cada peça del grup, per grup, etc...");



            Debug.Log($"Viable = {viable}");
            if (!viable)
                continue;

            //CREAR
            if (HiHaFrom)
            {
                assignar = referencies.GetEstat(From);
                nomRecepta = $"To{lastNameFounded}_From{From}";
            }
            else
            {
                assignar = referencies.GetEstat(lastNameFounded);
                nomRecepta = $"{lastNameFounded}Produeix{Output(1)}{(HiHaInput(1) ? $"_{Input(1)}" : "")}";
            }

            //ASSIGNAR
            if (!assignar)
                continue;

            recepta.Setup(cPropia, inputs, cInputs, outputs, accioConnectar);
            AssetDatabase.CreateAsset(recepta, Path_Recepta(nomRecepta, i));

            Debug.Log($"Crear Recepta: {nomRecepta}[{i + 1}] a: {assignar.name}");
            assignar.AddRecepta(recepta);
            EditorUtility.SetDirty(assignar);
        }

        Debug.Log("SAVE ALL!");
        AssetDatabase.SaveAssets();
    }


    Peça.ConnexioEnum tmpConnexio;
    private Peça.ConnexioEnum GetConnexio(bool confirmar, string connexio)
    {
       tmpConnexio = Peça.ConnexioEnum.NoImporta;

        if (!confirmar)
            return tmpConnexio;

        switch (connexio)
        {
            case "No Importa":
                break;
            case "CONNECT":
                tmpConnexio = Peça.ConnexioEnum.Connectat;
                break;
            case "DESCONN":
                tmpConnexio = Peça.ConnexioEnum.Desconnectat;
                break;
            case "AMB MI":
                tmpConnexio = Peça.ConnexioEnum.ConnectatAmbMi;
                break;
            default:
                Debug.LogError($"El tipus de connexio amb nom {connexio} no se quin és!");
                break;
        }

        return tmpConnexio;
    }



    void CreaProductes()
    {
        for (int i = linies.Length - 1; i >= 0; i--)
        {
            if (IniciProductes(i))
            {
                liniaProductes = i;
                break;
            }
        }

        llistaProductes = new List<string>();
        for (int i = liniaProductes; i < linies.Length; i++)
        {
            GetColumnes(i);

            if (!TeNom)
                continue;

            llistaProductes.Add(columnes[1]);
        }

        for (int i = 0; i < llistaProductes.Count; i++)
        {
            if (AssetDatabase.LoadAssetAtPath<Producte>(Path_Producte(llistaProductes[i])) != null)
                continue;

            Producte producte = CreateInstance<Producte>();
            Texture2D icone = AssetDatabase.LoadAssetAtPath<Texture2D>(Path_IconeTexture(llistaProductes[i]));
            if(icone == null)
            {
                Debug.LogError($"No he trobat la icone amb el nom {llistaProductes[i]}");
                continue;
            }
            producte.Setup(icone);

            AssetDatabase.CreateAsset(producte, Path_Producte(llistaProductes[i]));

            Debug.Log($"Crear el Producte: {llistaProductes[i]}");
        }

        referencies.Refresh();
    }

    private void CrearEstats(Confirmacio[] confirmacions)
    {
        for (int i = 3; i < linies.Length; i++)
        {
            if (i.Equals(liniaProductes))
                break;

            GetColumnes(i);

            if (!TeNom)
                continue;

            if (!ConfirmarEstat(confirmacions, Nom))
            {
                if (!ConfirmarTiles(confirmacions, Nom))
                    continue;

                tilesetUnpack.Unpack($"{Nom.Substring(0, 1)}{Nom.Substring(1, Nom.Length - 1).ToLower()}", subobjects, outputPath, outputDetalls, referencies, detalls, reimportForçat);

                continue;
            }

            Estat.TipusEnum tipus = Estat.TipusEnum.Normal;
            if (TeTipus)
            {
                switch (Tipus)
                {
                    case "Casa":
                        tipus = Estat.TipusEnum.Casa;
                        break;
                    case "Productor":
                        tipus = Estat.TipusEnum.Productor;
                        break;
                    case "Extraccio":
                        tipus = Estat.TipusEnum.Extraccio;
                        break;
                    default:
                        Debug.LogError($"No reconec el tipus d'Estat amb nom {Tipus}");
                        break;
                }
            }



            Producte producte = null;
            if (TeProducte)
            {
                producte = referencies.GetProducte(NomProducte);
            }

            Estat estat = CreateInstance<Estat>();
            estat.SetupCreacio(tipus, EsAquatic, producte);

            if (!AssetDatabase.IsValidFolder($"{outputPath}/{Nom}"))
            {
                AssetDatabase.CreateFolder($"{outputPath}", $"{Nom}");
                AssetDatabase.CreateFolder($"{outputPath}/{Nom}", "Prefab");
                AssetDatabase.CreateFolder($"{outputPath}/{Nom}", "Colocable");
            }
            AssetDatabase.CreateAsset(estat, Path_Estat());

            Debug.Log($"Crear Estat: {Nom}");

            if (!ConfirmarTiles(confirmacions, Nom))
                continue;

            referencies.Refresh();
            tilesetUnpack.Unpack($"{Nom.Substring(0,1)}{Nom.Substring(1,Nom.Length - 1).ToLower()}", subobjects, outputPath, outputDetalls, referencies, detalls, reimportForçat);
        }



       


    }

    [Button("hola?")]
    void AddConnectables()
    {
        for (int i = 0; i < referencies.Tilesets.Length; i++)
        {
            Debug.Log($"Agafar Connectables de {referencies.Tilesets[i].name}");
            if (referencies.Tilesets[i] is TileSet_Simple)
            {
                AddConnectables(((TileSet_Simple)referencies.Tilesets[i]).TileSet);
            }
            else if (referencies.Tilesets[i] is TileSet_Ocupable)
            {
                AddConnectables(((TileSet_Ocupable)referencies.Tilesets[i]).TileSetLliure);
                AddConnectables(((TileSet_Ocupable)referencies.Tilesets[i]).TileSetOcupat);
            }
            else if (referencies.Tilesets[i] is TileSet_Condicional)
            {
                for (int c = 0; c < ((TileSet_Condicional)referencies.Tilesets[i]).Condicions.Length; c++)
                {
                    AddConnectables(((TileSet_Condicional)referencies.Tilesets[i]).Condicions[c].TileSet);
                }
            }
        }
    }

    Connexio[] connexions;
    void AddConnectables(TileSet tileset)
    {
        List<Tile> tiles = new List<Tile>();
        
        for (int t = 0; t < tileset.Tiles.Length; t++)
        {
            connexions = new Connexio[]
            {
                tileset.Tiles[t].tile.Exterior(0),
                tileset.Tiles[t].tile.Dreta(0),
                tileset.Tiles[t].tile.Esquerra(0)
            };
            for (int r = 0; r < referencies.Tiles.Length; r++)
            {
                if (tileset.Contains(referencies.Tiles[r]))
                    continue;

                if (tiles.Contains(referencies.Tiles[r]))
                    break;

                //Debug.Log($"{referencies.Tiles[r].name} Encaixa amb {tileset.Tiles[t].tile.name}?");

                for (int i = 0; i < connexions.Length; i++)
                {

                    if (connexions[i].EncaixaAmb(referencies.Tiles[r].Exterior(0)) ||
                        connexions[i].EncaixaAmb(referencies.Tiles[r].Dreta(0)) ||
                        connexions[i].EncaixaAmb(referencies.Tiles[r].Esquerra(0))
                        )
                    {
                        

                        Debug.Log($"[{tileset.Tiles[t].tile.name}] == [{referencies.Tiles[r].name}]");

                        tiles.Add(referencies.Tiles[r]);
                        break;
                    }
                }
            }

            
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            Debug.Log($"Add {tiles[i]}");
            tileset.AddTile(tiles[i], 0);
        }
    }





    string Path_Recepta(string nom, int linia) => $"{outputReceptes}/{nom}[{linia+1}].asset";
    string Path_Estat() => $"{outputPath}/{Nom}.asset";
    string Path_Producte(string nom) => $"{outputProductes}/{nom}.asset";
    string Path_IconeTexture(string nom) => $"{outputProductes}/Textures/{nom}.png";





    void GetColumnes(int i) => columnes = linies[i].Split(SEPARADOR);
    bool IniciProductes(int i) => linies[i].StartsWith(PRODUCTES);
    //bool LiniaActiva(int i) => linies[i].StartsWith(TRUE);
    //bool Activat => columnes[0].Equals(TRUE);
    string Nom => columnes[1];
    string Tipus => columnes[3];
    bool EsAquatic => columnes[4].Equals(TRUE);
    string NomProducte => columnes[5];
    string From => columnes[7];


    string ConnexioPropia => columnes[8];
    string Input(int i) => columnes[9 + i];
    string Output(int i) => columnes[17 + i];
    string ConnexioInputs => columnes[16];
    string AccioConnectar => columnes[24];




    bool TeNom => !string.IsNullOrEmpty(Nom);
    bool TeTipus => !string.IsNullOrEmpty(Tipus);
    bool TeProducte => !string.IsNullOrEmpty(NomProducte);

    bool HiHaFrom => !string.IsNullOrEmpty(From);
    bool HiHaInput(int i) => !string.IsNullOrEmpty(Input(i));
    bool HiHaOutput(int i) => !string.IsNullOrEmpty(Output(i));
    bool HiHaConnexioPropia => !string.IsNullOrEmpty(ConnexioPropia) || !ConnexioPropia.Equals(NO_IMPORTA);
    bool HiHaConnexioInputs => !string.IsNullOrEmpty(ConnexioInputs) || !ConnexioInputs.Equals(NO_IMPORTA);
    //bool HiHaXP => !string.IsNullOrEmpty(XP);
    bool HiHaAccioConnectar => !string.IsNullOrEmpty(AccioConnectar) || !AccioConnectar.Equals(NO_IMPORTA);











    [System.Serializable]
    public struct Confirmacio
    {
        [SerializeField] string nom;

        [SerializeField, VerticalGroup("E"), TableColumnWidth(23, resizable: false), LabelText("")]
        bool estat;

        [SerializeField, VerticalGroup("R"), TableColumnWidth(23, resizable: false), LabelText("")]
        bool recepta;

        [SerializeField, VerticalGroup("T"), TableColumnWidth(23, resizable: false), LabelText("")]
        bool tiles;

        [VerticalGroup("Import"), DisableIf("@!this.estat && !this.recepta && !this.tiles"), SerializeField, TableColumnWidth(40, resizable: false), Button(Icon = SdfIconType.Archive), PropertyOrder(-1)]
        public void Importar() => importar?.Invoke(new Confirmacio[] { this });

        System.Action<Confirmacio[]> importar;
        Referencies referencies;

        public System.Action<Confirmacio[]> SetImporar { set => importar = value; }
        public Referencies SetReferencies { set => referencies = value; }


        [VerticalGroup("Del"), SerializeField, TableColumnWidth(30, resizable: false), Button(Icon = SdfIconType.FolderMinus), GUIColor(1, .75f, .75f)]
        void Destroy()
        {
            int opcio = EditorUtility.DisplayDialogComplex("BORRAr", "Vols borrar l'estat, el tilset i tots els tiles d'aquest Estat?\nAixo només s'hauria de fer amb previsió de Reimportar-ho tot de nou.\nPensa que molt provablament s'hauran de reimportar la resta d'estat per estar segur que no hi ha referencies de tiles o estats de receptes perduts.", "BORRAR", "NOOO!!!", "BORRAR I RE-IMPORTAR");

            if (opcio == 1)
                return;

            else if (opcio == 0)
            {
                Borrar();
            }
            else
            {
                Borrar();
                Importar();
            }


           
        }
        void Borrar()
        {
            Debug.Log("Destroy");

            Estat aquest = referencies.GetEstat(nom);
            for (int r = 0; r < referencies.Receptes.Length; r++)
            {
                if (referencies.Receptes[r].ConteInput(aquest) || referencies.Receptes[r].ConteOutput(aquest))
                {
                    for (int e = 0; e < referencies.Estats.Length; e++)
                    {
                        for (int er = 0; er < referencies.Estats[e].Receptes.Length; er++)
                        {
                            if (referencies.Estats[e].Receptes[er].Equals(referencies.Receptes[r]))
                            {
                                referencies.Estats[e].RemoveRecepta(referencies.Estats[e].Receptes[er]);
                            }
                        }
                    }
                }
            }

            AssetDatabase.DeleteAsset($"Assets/XidoStudio/Hexbase/Peces/Estats/{nom}.asset");
            AssetDatabase.DeleteAsset($"Assets/XidoStudio/Hexbase/Peces/Estats/{nom}");

            referencies.Refresh();
        }

        public string Nom => nom;
        public bool Estat => estat;
        public bool Recepta => recepta;
        public bool Tiles => tiles;
    }







    protected void OnValidate()
    {
        if (referencies != null) referencies = XS_Utils.XS_Editor.LoadAssetAtPath<Referencies>("Assets/XidoStudio/Hexbase/Sistemes/Referencies.asset");

        for (int i = 0; i < confirmacions.Length; i++)
        {
            confirmacions[i].SetImporar = Unpack;
            confirmacions[i].SetReferencies = referencies;
        }

    }
}


