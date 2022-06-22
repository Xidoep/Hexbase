using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Basic")]
public class EstatPeça : ScriptableObject
{
    [SerializeField] Tile[] tilesPossibles;
    [SerializeField] Connexio[] connexionsPossibles;
    [SerializeField] Subestat subestatInicial;

    public Tile[] Possibilitats() => tilesPossibles;
    public Connexio[] ConnexionsPossibles => connexionsPossibles;
    public Subestat SubestatInicial => subestatInicial;

    public virtual void TilesInicials(TilePotencial[] tiles) 
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Escollir(new WaveFunctionColapse.Possibilitats(tilesPossibles[0], 0), 0);
        }
    }

    public virtual Connexio[] Null(TilePotencial tile) => null;

    protected bool VeiNull(TilePotencial tile) => tile.Veins[0] == null;

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

