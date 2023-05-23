using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackEstats")]
public class EstatsUnpack : ScriptableObject
{
    const string TRUE = "TRUE";
    const string CONNECTAT = "connectat";
    List<string> PRODUCTES = new List<string>
    {
        "Blat",
        "Arros",
        "Aigua",
        "Peix",
        "Carn",
        "Llenya",
        "Pa",
        "Minerals"
    };

    [SerializeField] Object csv;
    [SerializeField] string outputPath;

    string[] linies;
    string[] columnes;



    [ContextMenu("Unpack")]
    void Unpack()
    {
        string debug = "";

        linies = System.IO.File.ReadAllLines(AssetDatabase.GetAssetPath(csv));

        for (int l = 3; l < linies.Length; l++)
        {
            columnes = linies[l].Split(',');
            if (!columnes[0].Equals(TRUE))
                continue;

            debug = "";

            debug += $"{columnes[1]} | Tipus = {columnes[3]} | Gestiona = {columnes[4]} | Aquatic = {columnes[5]}";
            Debug.Log(debug);
        }

        for (int l = 3; l < linies.Length; l++)
        {
            columnes = linies[l].Split(',');
            if (!columnes[0].Equals(TRUE))
                continue;

            debug = "";
            debug += $"Posat a {(columnes[6] == CONNECTAT ? columnes[1] : columnes[6])} |";
            debug += $"{(PRODUCTES.Contains(columnes[8]) ? $"Produeix: {columnes[8]},{columnes[9]},{columnes[10]},{columnes[11]},{columnes[12]},{columnes[13]}" : $"Inputs: {columnes[8]},{columnes[9]},{columnes[10]},{columnes[11]},{columnes[12]},{columnes[13]}")}";
            debug += $" |";


            //debug += $"Nova recepta posada a {columnes[6]} | Inputs:{columnes[8]},{columnes[9]},{columnes[10]},{columnes[11]},{columnes[12]},{columnes[13]} | Output: {columnes[1]}";
            Debug.Log(debug);
        }


    }
}
