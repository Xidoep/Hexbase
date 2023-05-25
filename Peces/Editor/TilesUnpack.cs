using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;

[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackTiles")]
public class TilesUnpack : ScriptableObject
{
    const string TERRA = "Terra";
    const string CASA = "Casa";
    const string RIU = "Riu";

    const string AUTOGENERATS_PATH = "Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats";
    const string TIPUS_SIMPLE = "TS";
    const string TIPUS_OCUPABLE = "TO";
    const string TIPUS_CONDICIONAL = "TC";
    const string NULES = "Nules";
    const string ESPECIFIQUES = "Espes";
    const char SEPARADO = '_';
    const char SEPARADO_INFORMACIO = ':';
    const char IGUAL = '=';
 


    [SerializeField] Referencies referencies;
    [SerializeField] Object tiles;
    [SerializeField] string outputPath;

    [SerializeField] bool terra;
    [SerializeField] bool casa;
    [SerializeField] bool riu;

    [Apartat("Auto-configurable")]
    [SerializeField] List<Object> connexions;



    //INTERN
    Object[] subobjects;
    string nom;
    string[] nomsConnexions;
    Connexio[] connexionsTile;
    GameObject intance;
    GameObject prefab;
    bool trobat;
    Connexio connexio;
    Tile tile;


   [ContextMenu("Unpack")]
    void Unpack()
    {
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        //
        if (terra) Unpack(TERRA);
        if (casa) Unpack(CASA);
        if (riu) Unpack(RIU);
    }

    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsPeça(int i, string root) => subobjects[i].name.StartsWith($"_{root}");

    private void Unpack(string root)
    {
        return;

        if (!AssetDatabase.IsValidFolder($"{outputPath}/{root}"))
            AssetDatabase.CreateFolder(outputPath, root);
        if (!AssetDatabase.IsValidFolder($"{outputPath}/{root}/Prefab"))
            AssetDatabase.CreateFolder($"{outputPath}/{root}", "Prefab");


        string debug = "";

        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;


            if (EsPeça(i,root))
            {
                if (subobjects[i].name.Contains('.'))
                    continue;

                //Agafar nomes nets
                string[] pes = subobjects[i].name.Split('$');

                Debug.Log(pes[1]);





                string[] invertit = subobjects[i].name.Split("%");
                nom = invertit[0];
                if (nom.Contains('('))
                {
                    string[] first = nom.Split('(');
                    string[] second = first[1].Split(')');
                    nom = $"{first[0]}{second[1]}";
                    Debug.Log(nom);
                }

                //Borrar antic
                AssetDatabase.DeleteAsset($"{outputPath}/{root}/{nom}.asset");
                AssetDatabase.DeleteAsset($"{outputPath}/{root}/Prefab/{nom}.prefab");

                //Crear prefab
                intance = (GameObject)Instantiate(subobjects[i]);
                prefab = PrefabUtility.SaveAsPrefabAsset(intance, $"{outputPath}/{root}/Prefab/{nom}.prefab");
                //if (subobjects[i].name.Contains('('))
                //    prefab.transform.localScale = new Vector3(-1, 1, 1);

                DestroyImmediate(intance);



                //Agafar connexions
                nomsConnexions = nom.Substring(root.Length + 1).Split(SEPARADO);
                connexionsTile = new Connexio[3];
                for (int nc = 0; nc < 3; nc++)
                {
                    trobat = false;
                    Debug.Log($"{nomsConnexions[nc]}");
                    for (int c = 0; c < connexions.Count; c++)
                    {
                        if (nomsConnexions[nc] == connexions[c].name)
                        {
                            connexionsTile[nc] = connexions[c] as Connexio;
                            trobat = true;
                            break;
                        }
                    }
                    if (!trobat)
                    {
                        connexio = CreateInstance<Connexio>();
                        connexio.name = nomsConnexions[nc];
                        AssetDatabase.CreateAsset(connexio, $"{AUTOGENERATS_PATH}/{connexio.name}.asset");
                        connexionsTile[nc] = AssetDatabase.LoadAssetAtPath<Connexio>($"{AUTOGENERATS_PATH}/{connexio.name}.asset");
                    }
                }

                if(invertit.Length > 1)
                {
                    prefab.transform.localScale = new Vector3(-1, 1, 1);
                }

                //Crear tile
                tile = CreateInstance<Tile>();
                tile.Setup(prefab, connexionsTile);

                //Guardar tile
                AssetDatabase.CreateAsset(tile, $"{outputPath}/{root}/{nom}.asset");

                Debug.Log("FALTA: Assignar el tile al tileset que l'hi toca... es molt facil de fer, ja que el tileset està a la ruta .../root/root.asset");
            }
            Debug.Log("FALTA: Agafar les connexions de la referencia. i refrescar de tant en tant la referencia perque els altres processos tinguin acces a la informacio actualitzada");
        }
    }

    private void OnValidate()
    {
        connexions = XS_Editor.LoadAllAssetsAtPath<Object>(AUTOGENERATS_PATH);
    }
}
