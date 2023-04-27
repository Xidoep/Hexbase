using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    [SerializeField] ScriptableObject[] inputs;
    [SerializeField] Peça.EstatConnexioEnum connexio;
    [SerializeField] ScriptableObject[] output;


    //INTERN
    bool confirmat = true;
    List<object> estatsVeins;

    public ScriptableObject[] Inputs => inputs;


    public bool TeInputsIguals(List<object> ingredients)
    {
        if (ingredients.Count == 0)
            return false;


        if (connexio == Peça.EstatConnexioEnum.NoImporta || ingredients[0] is not Peça)
            return ConfirmarRecepta(ingredients);


        estatsVeins = new List<object>();
        for (int i = 0; i < ingredients.Count; i++)
        {
            if(connexio.HasFlag(((Peça)ingredients[i]).EstatConnexio)) estatsVeins.Add(((Peça)ingredients[i]).Subestat);
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
    public void Processar(Peça peça)
    {
        for (int i = 0; i < output.Length; i++)
        {
            ((IProcessable)output[i]).Processar(peça);
        }
    }






    int tipus = -1;
    private void OnValidate()
    {
        
        if(inputs.Length > 1)
        {
            tipus = inputs[0] is Producte ? 1 : 2;
            for (int i = 1; i < inputs.Length; i++)
            {
                if (tipus == 1 && inputs[i] is not Producte)
                {
                    Debug.LogError("PROHIBIT!!! No pots barrejar productes i estats en els inputs d'una recepta!!! Simplement no està preparat perquè passi", this);
                }
            }
        }
        

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

}
