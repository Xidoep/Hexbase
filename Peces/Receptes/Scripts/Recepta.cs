using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    [Header("CONNEXIO")]
    [Tooltip("La connexio de la pe�a que porta la recepta ha de cohincidir amb el que s'ha posat aqu�.")]
    [SerializeField] Pe�a.ConnexioEnum connexioPropia;
    //[SerializeField] Pe�a.ConnexioEnum connectada;

    [Apartat("INPUTS")]
    [SerializeField] ScriptableObject[] inputs;
    [Tooltip("En el cas que l'input sigui una Pe�a, la connexio d'aquesta ha de coincidir amb el que s'ha posat aqu�.")]
    [SerializeField] Pe�a.ConnexioEnum connexioInput;

    [Apartat("OUTPUTS")]
    [SerializeField] ScriptableObject[] output;
    [SerializeField] Pe�a.ConnexioEnum connexio;

    [Apartat("REFERENCIUES AUTO-CONFIGURABLES")]
    [SerializeField] Produccio produccio;

    //INTERN
    bool confirmat = true;
    List<object> estats;


    public ScriptableObject[] Inputs => inputs;





    /*public bool ConnexioConnectada(Pe�a pe�a)
    {
        if (connectada == Pe�a.ConnexioEnum.NoImporta)
            return true;

        if (!pe�a.Connectat)
            return false;

        return pe�a.Connexio.EstatConnexio.HasFlag(connectada);
    }*/

    public bool TeInputsIguals(Pe�a pe�a, List<object> inputs)
    {
        if (!HiHaInputs(inputs))
            return false;

        if (!ConnexioPropia(pe�a))
            return false;

        if(!InputPe�a(inputs[0]))
            return ConfirmarRecepta(pe�a, inputs);

        //agafar els estats que tinguin les connexions com les demano.
        estats = new List<object>();
        for (int i = 0; i < inputs.Count; i++)
        {
            if (ConnexioNoImporta(inputs[i]))
            {
                estats.Add(((Pe�a)inputs[i]).Subestat);
                continue;
            }

            if (ConnectatAmbMi(ref estats, pe�a, inputs[i]))
                continue;

            if(ConnexionsIguals(inputs[i]))
                estats.Add(((Pe�a)inputs[i]).Subestat);
        }
        return ConfirmarRecepta(pe�a, estats);
    }

    bool HiHaInputs(List<object> inputsPassats) => inputsPassats.Count != 0;
    bool ConnexioPropia(Pe�a pe�a)
    {
        if (connexioPropia == Pe�a.ConnexioEnum.NoImporta)
            return true;

        return pe�a.GetEstatConnexio.HasFlag(connexioPropia);
    }
    bool InputPe�a(object input) 
    {
        Debug.Log($"{input} is Pe�a? = {input is Pe�a}");
        return input is Pe�a;
    }
    bool ConnexioNoImporta(object input) => connexioInput.HasFlag(Pe�a.ConnexioEnum.NoImporta);
    bool ConnectatAmbMi(ref List<object> passats, Pe�a pe�a, object input)
    {
        if (connexioInput.HasFlag(Pe�a.ConnexioEnum.ConnectatAmbMi))
        {
            if (!((Pe�a)input).EstaConnectat)
                return true; //Ha d'estar connectat pero no ho est� a res, per tant no el puc deixar contiuar

            if (((Pe�a)input).Connexio != pe�a)
                return true; //Ha d'estar connectat amb mi, pero no ho est� amb mi. per tant no el puc deixar continuar

            passats.Add(((Pe�a)input).Subestat);
            return true; //Est� connectat amb mi, per tant no cal que continui.
        }
        return false; //No est� marcat com connectat per tant haig de continuar mirant.
    }
    bool ConnexionsIguals(object input) => connexioInput.HasFlag(((Pe�a)input).GetEstatConnexio);


    bool ConfirmarRecepta(Pe�a pe�a, List<object> passats)
    {
        confirmat = true;
        for (int i = 0; i < inputs.Length; i++)
        {
            for (int x = 0; x < passats.Count; x++)
            {
                Debug.Log($"{inputs[i]} == {passats[x]}?");
            }

            if (!passats.Contains(inputs[i]))
            {
                confirmat = false;
                break;
            }
        }

        if (connexio.HasFlag(Pe�a.ConnexioEnum.NoImporta))
            return confirmat;

        if (confirmat)
        {
            switch (connexio)
            {
                case Pe�a.ConnexioEnum.Connectat:
                    pe�a.Connectar(((Pe�a)passats[0]));
                    break;
                case Pe�a.ConnexioEnum.Desconnectat:
                    pe�a.Desconnectar();
                    break;
            }
        }

        return confirmat;
    }

    public void Processar(Pe�a pe�a)
    {
        for (int i = 0; i < output.Length; i++)
        {
            ((IProcessable)output[i]).Processar(pe�a);
        }

        if (produccio == null)
            return;

        produccio.AddProductor(pe�a);
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
                    Debug.LogError("PROHIBIT!!! No pots barrejar productes i estats en els inputs d'una recepta!!! Simplement no est� preparat perqu� passi", this);
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

        for (int i = 0; i < output.Length; i++)
        {
            if(output[i] is Producte)
            {
                produccio = XS_Utils.XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
                break;
            }
        }

    }

}
