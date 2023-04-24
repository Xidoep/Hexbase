using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    [SerializeField] ScriptableObject[] inputs;
    [SerializeField] ScriptableObject[] output;

    //INTERN
    bool confirmat = true;

    public bool TeInputsIguals(ScriptableObject[] inputs) => inputs == this.inputs;
    public bool TeInputsIguals(List<object> ingredients)
    {
        //per cada input que s'envia s'ha de mirar si està contingut a la llista de la recepta.
        //Si un element hi es. Perfecte, es passa a comprar el segon element, i aixi fins a confirmar que els conté tots.
        //Si un sol falla, ja no es podrà complir la recepta, així que es para.
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
}
