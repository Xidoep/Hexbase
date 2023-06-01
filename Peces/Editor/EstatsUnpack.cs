using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackEstats")]
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
    string outputPath, outputProductes, outputReceptes, outputColocables;


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

    [TableList(), SerializeField, PropertyOrder(-1)]
    Confirmacio[] confirmacions;

    [System.Serializable]
    public struct Confirmacio
    {
        [SerializeField] string nom;
        [SerializeField, VerticalGroup("E"), TableColumnWidth(23, resizable: false), LabelText("")] 
        bool estat;

        [SerializeField, VerticalGroup("T"), TableColumnWidth(23, resizable: false), LabelText("")] 
        bool tiles;

        [DisableIf("@!this.estat && !this.tiles"), SerializeField, TableColumnWidth(40, resizable: false), Button(Icon = SdfIconType.Archive), VerticalGroup("Importar")]
        public void Importar() => importar?.Invoke(new Confirmacio[] { this });
        

        System.Action<Confirmacio[]> importar;

        public System.Action<Confirmacio[]> SetImporar { set => importar = value; }

        public string Nom => nom;
        public bool Estat => estat;
        public bool Tiles => tiles;
    }

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




    [PropertyOrder(-2), Button(ButtonSizes.Large, Icon = SdfIconType.Archive, IconAlignment = IconAlignment.LeftOfText)]
    void Unpack() => Unpack(confirmacions);
    void Unpack(Confirmacio[] confirmacions)
    {

        string debug = "";

        //Crear linies i columnes
        linies = System.IO.File.ReadAllLines(AssetDatabase.GetAssetPath(csv));

        for (int i = 3; i < linies.Length; i++)
        {
            if (!LiniaActiva(i))
                continue;

            if (IniciProductes(i))
                break;

            GetColumnes(i);
        }

        CreaProductes();

        //GET MESHES
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        Debug.LogError("FALTA: Crear els props, si no existeixen");

        CrearEstats(confirmacions);

        CrearReceptes(confirmacions);

    }

    private void CrearReceptes(Confirmacio[] confirmacions)
    {
        string debug = "RECEPTES:\n";
       
        string lastNameFounded = "";


        for (int i = 3; i < linies.Length; i++)
        {
            if (i.Equals(liniaProductes))
                break;

            if (!ConfirmarTiles(confirmacions, Nom))
                continue;

            GetColumnes(i);
            if (!Activat)
                continue;

            if (!HiHaInput(1) && !HiHaOutput(1))
                continue;

            viable = true;


            if (TeNom) lastNameFounded = Nom;

            #region DEBUGGING
            if (HiHaFrom)
            {
                debug += $"To{lastNameFounded}_From{From}: ";
            }
            else
            {
                debug += $"{lastNameFounded}Produeix{Output(1)}{(HiHaInput(1) ? $"_{Input(1)}" : "")}";
            }


            if (HiHaConnexioPropia) debug += $" | ConnexioPropia = {ConnexioPropia}";

            debug += $" |  | INPUTS:";

            if (HiHaInput(1)) debug += $" {Input(1)} ,";
            if (HiHaInput(2)) debug += $" {Input(2)} ,";
            if (HiHaInput(3)) debug += $" {Input(3)} ,";
            if (HiHaInput(4)) debug += $" {Input(4)} ,";
            if (HiHaInput(5)) debug += $" {Input(5)} ,";
            if (HiHaInput(6)) debug += $" {Input(6)} ,";

            //-------------------------------------------------
            if (HiHaConnexioInputs) debug += $" | Connexio Inputs = {ConnexioInputs}";

            debug += $" |  | OUTPUTS:";

            if (HiHaFrom) debug += $" {From} ,";
            if (HiHaOutput(1)) debug += $" {Output(1)} ,";
            if (HiHaOutput(2)) debug += $" {Output(2)} ,";
            if (HiHaOutput(3)) debug += $" {Output(3)} ,";
            if (HiHaOutput(4)) debug += $" {Output(4)} ,";
            if (HiHaOutput(5)) debug += $" {Output(5)} ,";
            if (HiHaOutput(6)) debug += $" {Output(6)} ,";

            if (HiHaAccioConnectar) debug += $" | {AccioConnectar}";

            if (HiHaFrom)
            {
                debug += $" --- Add it to {From}";
            }
            else
            {
                debug += $" --- Add it to {lastNameFounded}";
            }
            debug += "\n";
            #endregion

            //continue;

            Recepta recepta = CreateInstance<Recepta>();

            //PROPIA
            Peça.ConnexioEnum cPropia = GetConnexio(HiHaConnexioPropia, ConnexioPropia);

            //INPUTS
            List<ScriptableObject> inputs = new List<ScriptableObject>();
            for (int input = 1; input < 7; input++)
            {
                if (!HiHaInput(input))
                    continue;

                if (!referencies.EstatsContains(Input(input)))
                {
                    viable = false;
                    Debug.LogError($"Aquesta recepta no es pot completar perque aquest l'Estat {Input(input)} no s'ha importat");
                    Debug.Log("FALTA: Ara no es contemplen modificadors o altres coses que no siguin estats...");
                    break;
                }
                inputs.Add(referencies.GetEstat(Input(input)));
            }
            Peça.ConnexioEnum cInputs = GetConnexio(HiHaConnexioInputs, ConnexioInputs);

            //OUTPUTS
            List<ScriptableObject> outputs = new List<ScriptableObject>();
            if (HiHaFrom)
            {
                if (!referencies.EstatsContains(From))
                {
                    Debug.LogError($"Aquesta recepta no es pot completar perque aquest el L'Estat {From} no s'ha importat");
                    viable = false;
                }
                outputs.Add(referencies.GetEstat(lastNameFounded));
            }
            for (int output = 1; output < 7; output++)
            {
                if (!HiHaOutput(output))
                    continue;

                if (!referencies.ProductesContains(Output(output)))
                {
                    viable = false;
                    Debug.LogError($"Aquesta recepta no es pot completar perque aquest el producte {Output(output)} no s'ha importat");
                    Debug.Log("FALTA: Ara no es contemplen modificadors o altres coses que no siguin productes...");
                    break;
                }
                outputs.Add(referencies.GetProducte(Output(output)));
            }
            Peça.ConnexioEnum accioConnectar = GetConnexio(HiHaAccioConnectar, AccioConnectar);



           

            if (!viable)
                continue;

            //CREAR
            string nom = "";
            Estat assignar = null;
            if (HiHaFrom)
            {
                assignar = referencies.GetEstat(From);
                nom = $"To{lastNameFounded}_From{From}: ";
            }
            else
            {
                assignar = referencies.GetEstat(lastNameFounded);
                nom = $"{lastNameFounded}Produeix{Output(1)}{(HiHaInput(1) ? $"_{Input(1)}" : "")}";
            }

            recepta.Setup(cPropia, inputs, cInputs, outputs, accioConnectar);
            AssetDatabase.CreateAsset(recepta, Path_Recepta(nom));



            //ASSIGNAR
            if (!assignar)
                continue;

            assignar.AddRecepta(recepta);

        }
        Debug.Log(debug);

    }



    private Peça.ConnexioEnum GetConnexio(bool confirmar, string connexio)
    {
        Peça.ConnexioEnum tmp = Peça.ConnexioEnum.NoImporta;

        if (!confirmar)
            return tmp;

        switch (connexio)
        {
            case "No Importa":
                break;
            case "CONNECT":
                tmp = Peça.ConnexioEnum.Connectat;
                break;
            case "DESCONN":
                tmp = Peça.ConnexioEnum.Desconnectat;
                break;
            case "AMB MI":
                tmp = Peça.ConnexioEnum.ConnectatAmbMi;
                break;
            default:
                Debug.LogError($"El tipus de connexio amb nom {connexio} no se quin és!");
                break;
        }

        return tmp;
    }



    void CreaProductes()
    {
        string debug = "PRODUCTES:\n";
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
            if (!LiniaActiva(i))
                continue;

            GetColumnes(i);

            llistaProductes.Add(columnes[1]);
        }


        for (int i = 0; i < llistaProductes.Count; i++)
        {
            debug += $"-{llistaProductes[i]}\n";
            Producte producte = CreateInstance<Producte>();
            Texture2D icone = AssetDatabase.LoadAssetAtPath<Texture2D>(Path_IconeTexture(llistaProductes[i]));
            if(icone == null)
            {
                Debug.LogError($"No he trobat la icone amb el nom {llistaProductes[i]}");
                continue;
            }
            producte.Setup(icone);

            AssetDatabase.CreateAsset(producte, Path_Producte(llistaProductes[i]));
        }
        Debug.Log(debug);

        referencies.Refrex();
    }
    private void CrearEstats(Confirmacio[] confirmacions)
    {
        string debug = "ESTATS:\n";
        for (int i = 3; i < linies.Length; i++)
        {
            if (i.Equals(liniaProductes))
                break;

            GetColumnes(i);
            if (!Activat)
                continue;

            if (!TeNom)
                continue;

            if (!ConfirmarEstat(confirmacions, Nom))
            {
                if (!ConfirmarTiles(confirmacions, Nom))
                    continue;

                tilesetUnpack.Unpack($"{Nom.Substring(0, 1)}{Nom.Substring(1, Nom.Length - 1).ToLower()}", subobjects, outputPath, referencies);

                continue;
            }

            debug += $"{Nom}";
            if (TeTipus) debug += $" ({Tipus})";

            if (EsAquatic) debug += $" | Aquatica";

            if (TeProducte) debug += $" | Pot gestionar = {NomProducte}";

            if (TeTipus)
            {
                debug += " | INFORMACIO: ";
                if (Tipus.Equals(CASA))
                {
                    debug += $"Casa i Grup";
                }
                else if (Tipus.Equals(PRODUCTOR))
                {
                    debug += $"Grup, Connexio i Productor";
                }
                else if (Tipus.Equals(EXTRACCIO))
                {
                    debug += $"Connexio i Extraccio";
                }
                else if (Tipus.Equals(CONNECTOR))
                {
                    debug += "Grup";
                }
                else if (Tipus.Equals(MAR))
                {
                    debug += "Grup";
                }
            }
            debug += "\n";

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


            if (!ConfirmarTiles(confirmacions, Nom))
                continue;

            referencies.Refrex();
            tilesetUnpack.Unpack($"{Nom.Substring(0,1)}{Nom.Substring(1,Nom.Length - 1).ToLower()}", subobjects, outputPath, referencies);
        }
        Debug.Log(debug);

        
    }







    string Path_Recepta(string nom) => $"{outputReceptes}/{nom}.asset";
    string Path_Estat() => $"{outputPath}/{Nom}/{Nom}.asset";
    string Path_Producte(string nom) => $"{outputProductes}/{nom}.asset";
    string Path_IconeTexture(string nom) => $"{outputProductes}/Textures/{nom}.png";




    void GetColumnes(int i) => columnes = linies[i].Split(SEPARADOR);
    bool IniciProductes(int i) => linies[i].StartsWith(PRODUCTES);
    bool LiniaActiva(int i) => linies[i].StartsWith(TRUE);
    bool Activat => columnes[0].Equals(TRUE);
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







    protected void OnValidate()
    {
        if (referencies != null) referencies = XS_Utils.XS_Editor.LoadAssetAtPath<Referencies>("Assets/XidoStudio/Hexbase/Sistemes/Referencies.asset");

        for (int i = 0; i < confirmacions.Length; i++)
        {
            confirmacions[i].SetImporar = Unpack;
        }
    }
}


