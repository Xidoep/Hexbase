using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    [SerializeField] ScriptableObject[] inputs;
    [SerializeField] Pe�a.EstatConnexioEnum connexio;
    [SerializeField] ScriptableObject[] output;


    //INTERN
    bool confirmat = true;
    List<object> estatsVeins;


    public bool TeInputsIguals(List<object> ingredients)
    {
        if (ingredients.Count == 0)
            return false;


        if (connexio == Pe�a.EstatConnexioEnum.NoImporta || ingredients[0] is not Pe�a)
            return ConfirmarRecepta(ingredients);


        estatsVeins = new List<object>();
        for (int i = 0; i < ingredients.Count; i++)
        {
            if(connexio.HasFlag(((Pe�a)ingredients[i]).EstatConnexio)) estatsVeins.Add(((Pe�a)ingredients[i]).Subestat);
        }
        return ConfirmarRecepta(estatsVeins);
    }

    bool ConfirmarRecepta(List<object> ingredients)
    {
        confirmat = true;
        for (int i = 0; i < this.inputs.Length; i++)
        {
            for (int x = 0; x < ingredients.Count; x++)
            {
                Debug.Log($"{this.inputs[i]} == {ingredients[x]}?");
            }
            if (!ingredients.Contains((object)this.inputs[i]))
            {
                confirmat = false;
                break;
            }
        }
        return confirmat;
    }











    public void Processar(System.Action<object> enProcessar) 
    {
        for (int i = 0; i < output.Length; i++)
        {
            enProcessar?.Invoke(output[i]);
        }
    } 
    public void Processar(Pe�a pe�a)
    {
        for (int i = 0; i < output.Length; i++)
        {
            ((IProcessable)output[i]).Processar(pe�a);
        }
    }







    private void OnValidate()
    {
        for (int i = output.Length -1; i >= 0; i--)
        {
            if (output[i] is not IProcessable) 
            {
                Debug.LogError($"l'output {output[i].name} no es un IProcessable!");
                List<ScriptableObject> _o = new List<ScriptableObject>(output);
                _o.RemoveAt(i);
                output = _o.ToArray();
            } 
        }
    }

    /*
     * Vale, tinc un problema/dubte. No puc posar la funcio a la recepta, perque la funcio depen de l'output.
     * Osigui, cada tipus de output ha de fer la seva cosa completametn diferent. 
     * Si poses com a output un estat, ha de canviar d'estat.
     * Si poses un producte, l'ha de produir i agregarlo als productes extrets per la pe�a.
     * Si poses un +punts, ha de donar punts.
     * Si en creo un de +peces, ha de aportar peces.
     * 
     * IProcessable?
     * No es pot posar la funcio la recepta, perque el que ha de fer depent dels outputs i aquests poden ser de qualsevol tipus i de mes d'un.
     * No se l'hi pot passar exteriorment pel mateix motiu, no saps quins outputs te la recepta.
     * Per tant, la funcio ha d'estar a l'output. Pero l'output es un objecte generic qualsevol.
     * Es pot provar de crear una interficie, que tingui una funcio On t'hi passa la pe�a.
     * El problema potencial que veig amb aixo, es que un mateix objecte no podr� fer 2 coses diferents segons el contexte.
     * perque l'objecte en si tindr� una sola funcio. Aixo simplifica les coses en realitat, simplifar est� b�.
     * En quins casos podria tenir aquest problema?
     * Subestats? com a output nom�s s'utilitzarien per canviar d'estat.
     * Productes? s'utilizen com a output nom�s per produir...
     * Xp? Aix� �s facil, nom�s serveixen per donar xp, aixo no canvia.
     * El preocupant son els subestats i els productes
     * Per donar peces... utilitzariem els estats, i potser creariem algun element extre per donar X peces aleatories.
     * Aixo ho aniriem descobrint, pero nom�s tindriem 4 tipus de receptes?
     * Clar, no. No hi ha tipus de receptes...
     * 
     * Provem en IProcessable.
     * Amb una funcio que es digui Processar(Pe�a pe�a).
     * I sense funcio delegada.
     * 
     * veus? sabia que algo fallava... Els inputs tamb� es fan servir com a necessitat...
     * mmm
     * no, espera.
     * Es diferent...
     * a Necessitatsm els productes son l'input
     * En canvi a Produccio els productes son l'output.
     * Per tant aixo es mant�.
     */
}
