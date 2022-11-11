using UnityEngine;
using UnityEngine.UI;

public class UI_Foto : MonoBehaviour
{
    public void Setup(Texture2D textura, string path, int indexPartida, System.Action<int> carregar, System.Action<string, int> borrar)
    {
        this.textura = textura;
        this.path = path;
        this.indexPartida = indexPartida;
        this.carregar = carregar;
        this.borrar = borrar;
    }

    [SerializeField] GameObject fotoZoom;


    Texture2D textura;
    string path;
    int indexPartida;
    System.Action<int> carregar;
    System.Action<string, int> borrar;


    public void Zoom()
    {
        Instantiate(fotoZoom).GetComponent<UI_FotoZoom>().Setup(textura,path,indexPartida,carregar,borrar);
        

        //******************************************************************************************
        //Mostrar o amagar el boto de carrear
        //Passar funcio carregar
        //Passar funcio eliminar
        //******************************************************************************************
    }

}
