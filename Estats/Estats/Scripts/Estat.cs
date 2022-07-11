using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Basic")]
public class Estat : ScriptableObject
{
    [Header("TILES")]
    [SerializeField] Tile[] tilesPossibles;
    [Tooltip("S'emplena automaticament")][SerializeField] Connexio[] connexionsPossibles;

    [Header("SUBESTAT")]
    [SerializeField] Subestat subestatInicial;
    [SerializeField] Condicio[] condicions;

    //PROPIETATS
    public Tile[] Possibilitats() => tilesPossibles;
    public Connexio[] ConnexionsPossibles => connexionsPossibles;
    public Subestat SubestatInicial => subestatInicial;



    //public virtual void OnCreate(Peça peça) { }


    //Els tiles amb que comença la peça quan es crea
    public virtual void TilesInicials(TilePotencial[] tiles) 
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Escollir(new WaveFunctionColapse.Possibilitats(tilesPossibles[0], 0), 0);
        }
    }


    //Que retorna si troba un vei null. En alguns estat puc voler que sigui una altre cosa.
    public virtual Connexio[] VeiNull(TilePotencial tile) => null;




    //Retorna les condicions de canvi de l'estat mes es subestat si es passa.
    public Condicio[] Condicions(Subestat subestat = null)
    {
        List<Condicio> _tmp = new List<Condicio>(condicions);
        if (subestat != null) _tmp.AddRange(subestat.Condicions);
        return _tmp.ToArray();
    }




    protected bool EsVeiNull(TilePotencial tile) => tile.Veins[0] == null;







    void OnValidate()
    {
        List<Connexio> connexios = new List<Connexio>();
        for (int i = 0; i < tilesPossibles.Length; i++)
        {
            if (!connexios.Contains(tilesPossibles[i].Exterior(0))) connexios.Add(tilesPossibles[i].Exterior(0));
            if (!connexios.Contains(tilesPossibles[i].Esquerra(0))) connexios.Add(tilesPossibles[i].Esquerra(0));
            if (!connexios.Contains(tilesPossibles[i].Dreta(0))) connexios.Add(tilesPossibles[i].Dreta(0));
        }
        connexionsPossibles = connexios.ToArray();
    }
}

