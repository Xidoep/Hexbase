using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Texte")]
public class Informacio_Texte : Informacio
{
    [SerializeField] LocalizedString texte;
    [SerializeField] UI_InformacioTexte prefab;

    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false) 
    {
        GameObject tmp = Instantiate(prefab.gameObject, hexagon.transform.position, Quaternion.identity, hexagon.transform);
        tmp.GetComponent<UI_InformacioTexte>().Setup(texte);
        ((Boto)hexagon).InformacioMostrada = new Unitat(tmp);
    }
    public override void Amagar(Hexagon hexagon)
    {
        Destroy(((Boto)hexagon).InformacioMostrada.gameObject);
    }
}
