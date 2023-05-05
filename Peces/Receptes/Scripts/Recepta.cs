using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    [SerializeField] ScriptableObject[] inputs;

    [Tooltip("La connexio de la peça que porta la recepta ha de cohincidir amb el que s'ha posat aquí.")]
    [SerializeField] Peça.ConnexioEnum connexioPropia;
    
    [Tooltip("En el cas que l'input sigui una Peça, la connexio d'aquesta ha de coincidir amb el que s'ha posat aquí.")]
    [SerializeField] Peça.ConnexioEnum connexioInputs;

    [Space(20)]
    [SerializeField] ScriptableObject[] output;


    //INTERN
    bool confirmat = true;
    List<object> estatsVeins;

    public ScriptableObject[] Inputs => inputs;
    public Peça.ConnexioEnum ConnexioPropia => connexioPropia;

    public bool TeInputsIguals(List<object> inputsPassats)
    {
        if (inputsPassats.Count == 0)
        {
            Debug.Log("No s'han passat ingredients...");
            return false;
        }

        Debug.Log($"{inputsPassats[0]} is Peça? = {inputsPassats[0] is Peça}");

        if (inputsPassats[0] is not Peça)
            return ConfirmarRecepta(inputsPassats);

        //if (connexioInputs == Peça.ConnexioEnum.NoImporta)
        //    return ConfirmarRecepta(inputsPassats);


        estatsVeins = new List<object>();
        for (int i = 0; i < inputsPassats.Count; i++)
        {
            if(connexioInputs.HasFlag(((Peça)inputsPassats[i]).EstatConnexio) || connexioInputs == Peça.ConnexioEnum.NoImporta) 
                estatsVeins.Add(((Peça)inputsPassats[i]).Subestat);
        }
        return ConfirmarRecepta(estatsVeins);
    }

    bool ConfirmarRecepta(List<object> inputsPassats)
    {
        confirmat = true;
        for (int i = 0; i < inputs.Length; i++)
        {
            for (int x = 0; x < inputsPassats.Count; x++)
            {
                Debug.Log($"{this.inputs[i]} == {inputsPassats[x]}?");
            }

            if (!inputsPassats.Contains((object)inputs[i]))
            {
                confirmat = false;
                break;
            }
        }
        return confirmat;
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
