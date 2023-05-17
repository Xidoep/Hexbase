using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Informacio")]
public class Informacio : ScriptableObject
{
    [SerializeField] UI_Informacio informacio;
    [Apartat("Comprovacions")]
    [SerializeField] bool comprovarConnexio;
    [SerializeField] bool connexioEsperada;

    Peça peça;

    public virtual void Mostrar(Hexagon hexagon)
    {
        if (!Comprovacions(hexagon))
            return;

        hexagon.InformacioMostrada = new Unitat(Instantiate(informacio, hexagon.transform.position, Quaternion.identity, hexagon.transform).Setup(hexagon));
    }
    public virtual void Amagar(Hexagon hexagon)
    {
        if (!Comprovacions(hexagon))
            return;

        hexagon.InformacioMostrada.Destroy();
    }


    protected bool Comprovacions(Hexagon hexagon)
    {
        if (comprovarConnexio)
        {
            peça = (Peça)hexagon;

            if (peça.EstaConnectat == connexioEsperada)
                return true;
            else return false;
        }
        else return true;
    }








    [System.Serializable]
    public struct Unitat
    {
        public Unitat(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        GameObject gameObject;

        public void Destroy() 
        {
            if (gameObject == null)
                return;

            GameObject.Destroy(gameObject);
        } 
    }
}
