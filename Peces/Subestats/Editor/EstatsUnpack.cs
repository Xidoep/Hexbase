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
    [SerializeField] Referencies referencies;



    string[] linies;
    string[] columnes;
    int liniaProductes;
    List<string> llistaProductes;







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
        debug = "PRODUCTES:\n";
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
        }
        Debug.Log(debug);


        Debug.Log("FALTA CREAR PRODUCTE");



        debug = "ESTATS:\n";
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

            if (TeProducte) debug += $" | Pot gestionar = {Producte}";

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




        }
        Debug.Log(debug);

        referencies.Refrex();

        debug = "RECEPTES:\n";

        string lastNameFounded = "";

        for (int i = 3; i < linies.Length; i++)
        {
            if (i.Equals(liniaProductes))
                break;

            GetColumnes(i);
            if (!Activat)
                continue;

            if (!HiHaInput1)
                continue;

            if (TeNom) lastNameFounded = Nom;

            if (EsProducte)
            {
                debug += $"{lastNameFounded}Produeix{Input1}";
            }
            else
            {
                debug += $"To{lastNameFounded}_From{From}: ";
            }


            if (HiHaConnexioPropia) debug += $" || ConnexioPropia = {ConnexioPropia}";

            if (EsProducte)
            {
                debug += $" || OUTPUTS:";
            }
            else
            {
                debug += $" || INPUTS:";
            }

            if (HiHaInput1) debug += $" {Input1} ,";
            if (HiHaInput2) debug += $" {Input2} ,";
            if (HiHaInput3) debug += $" {Input3} ,";
            if (HiHaInput4) debug += $" {Input4} ,";
            if (HiHaInput5) debug += $" {Input5} ,";
            if (HiHaInput6) debug += $" {Input6} ,";

            if (!EsProducte)
            {
                if (HiHaConnexioInputs) debug += $" | Connexio Inputs = {ConnexioInputs}";
            }


            if (!EsProducte)
            {
                debug += $" || OUTPUTS:";
                debug += $" {lastNameFounded},";
                if (HiHaXP) debug += $" {XP}xp,";
            }

            if (HiHaAccioConnectar) debug += $" | {AccioConnectar}";

            debug += $" --- Add it to {lastNameFounded}";

            debug += "\n";
            //debug += $"Nova recepta posada a {columnes[6]} | Inputs:{columnes[8]},{columnes[9]},{columnes[10]},{columnes[11]},{columnes[12]},{columnes[13]} | Output: {columnes[1]}";
        }
        Debug.Log(debug);



        

    }

    void GetColumnes(int i) => columnes = linies[i].Split(SEPARADOR);
    bool IniciProductes(int i) => linies[i].StartsWith(PRODUCTES);
    bool LiniaActiva(int i) => linies[i].StartsWith(TRUE);
    bool Activat => columnes[0].Equals(TRUE);
    string Nom => columnes[1];
    string Tipus => columnes[3];
    bool EsAquatic => columnes[4].Equals(TRUE);
    string Producte => columnes[5];
    string From => columnes[7];

    string ConnexioPropia => columnes[8];
    string Input1 => columnes[10];
    string Input2 => columnes[11];
    string Input3 => columnes[12];
    string Input4 => columnes[13];
    string Input5 => columnes[14];
    string Input6 => columnes[15];
    string ConnexioInputs => columnes[16];
    string XP => columnes[18];
    string AccioConnectar => columnes[19];



    bool TeNom => !string.IsNullOrEmpty(Nom);
    bool TeTipus => !string.IsNullOrEmpty(Tipus);
    bool TeProducte => !string.IsNullOrEmpty(Producte);

    bool HiHaConnexioPropia => !string.IsNullOrEmpty(ConnexioPropia) || !ConnexioPropia.Equals(NO_IMPORTA);
    bool EsProducte => llistaProductes.Contains(Input1);
    bool HiHaInput1 => !string.IsNullOrEmpty(Input1);
    bool HiHaInput2 => !string.IsNullOrEmpty(Input2);
    bool HiHaInput3 => !string.IsNullOrEmpty(Input3);
    bool HiHaInput4 => !string.IsNullOrEmpty(Input4);
    bool HiHaInput5 => !string.IsNullOrEmpty(Input5);
    bool HiHaInput6 => !string.IsNullOrEmpty(Input6);
    bool HiHaConnexioInputs => !string.IsNullOrEmpty(ConnexioInputs) || !ConnexioInputs.Equals(NO_IMPORTA);
    bool HiHaXP => !string.IsNullOrEmpty(XP);
    bool HiHaAccioConnectar => !string.IsNullOrEmpty(AccioConnectar) || !AccioConnectar.Equals(NO_IMPORTA);
}
