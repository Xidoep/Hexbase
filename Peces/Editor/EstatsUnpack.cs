using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    [SerializeField] Object csv;
    [SerializeField] string outputPath;
    [SerializeField] string outputProductes;
    [SerializeField] string outputReceptes;
    [SerializeField] Referencies referencies;
    [SerializeField] TilesetUnpack tilesetUnpack;



    string[] linies;
    string[] columnes;
    int liniaProductes;
    List<string> llistaProductes;
    bool viable;






    [ContextMenu("Unpack")]
    void Unpack()
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


        //Agafar productes
        CreaProductes();

        CrearEstats();

        CrearReceptes();

        Debug.Log("FALTA: Crear un proces m�s gran que ho fagi tot en ordre. Estats(Productes, Estats), Tilesets i Tiles");

        Debug.Log("FALTA! Assignar el tile set a l'estat. I potser per reordenar i que estigui tot en sucarpetes de Estats...");

    }



    private void CrearReceptes()
    {
        string debug = "RECEPTES:\n";
       
        string lastNameFounded = "";


        for (int i = 3; i < linies.Length; i++)
        {
            if (i.Equals(liniaProductes))
                break;

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
            Pe�a.ConnexioEnum cPropia = GetConnexio(HiHaConnexioPropia, ConnexioPropia);

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
            Pe�a.ConnexioEnum cInputs = GetConnexio(HiHaConnexioInputs, ConnexioInputs);

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
            Pe�a.ConnexioEnum accioConnectar = GetConnexio(HiHaAccioConnectar, AccioConnectar);



           

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
            AssetDatabase.CreateAsset(recepta, $"{outputReceptes}/{nom}.asset");



            //ASSIGNAR
            if (!assignar)
                continue;

            assignar.AddRecepta(recepta);

        }
        Debug.Log(debug);

    }



    private Pe�a.ConnexioEnum GetConnexio(bool confirmar, string connexio)
    {
        Pe�a.ConnexioEnum tmp = Pe�a.ConnexioEnum.NoImporta;

        if (!confirmar)
            return tmp;

        switch (connexio)
        {
            case "No Importa":
                break;
            case "CONNECT":
                tmp = Pe�a.ConnexioEnum.Connectat;
                break;
            case "DESCONN":
                tmp = Pe�a.ConnexioEnum.Desconnectat;
                break;
            case "AMB MI":
                tmp = Pe�a.ConnexioEnum.ConnectatAmbMi;
                break;
            default:
                Debug.LogError($"El tipus de connexio amb nom {connexio} no se quin �s!");
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
            Texture2D icone = AssetDatabase.LoadAssetAtPath<Texture2D>($"{outputProductes}/Textures/{llistaProductes[i]}.png");
            if(icone == null)
            {
                Debug.LogError($"No he trobat la icone amb el nom {llistaProductes[i]}");
                continue;
            }
            producte.Setup(icone);

            AssetDatabase.CreateAsset(producte, $"{outputProductes}/{llistaProductes[i]}.asset");
        }
        Debug.Log(debug);

        referencies.Refrex();
    }
    private void CrearEstats()
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

            AssetDatabase.CreateAsset(estat, $"{outputPath}/{Nom}.asset");
            Debug.Log("FALTA: Crear el prefab i l'EstatColocable, amb l'estat i el prefab associats");
            Debug.Log("FALTA: crear una carpeta per cada estat amb tota la seva info anidada");
        }
        Debug.Log(debug);

        referencies.Refrex();
    }





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
    //string XP => columnes[18];
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
}
