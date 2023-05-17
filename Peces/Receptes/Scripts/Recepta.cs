using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    [Header("CONNEXIO")]
    [Tooltip("La connexio de la peça que porta la recepta ha de cohincidir amb el que s'ha posat aquí.")]
    [SerializeField] Peça.ConnexioEnum connexioPropia;
    //[SerializeField] Peça.ConnexioEnum connectada;

    [Apartat("INPUTS")]
    [SerializeField] ScriptableObject[] inputs;
    [Tooltip("En el cas que l'input sigui una Peça, la connexio d'aquesta ha de coincidir amb el que s'ha posat aquí.")]
    [SerializeField] Peça.ConnexioEnum connexioInput;

    [Apartat("OUTPUTS")]
    [SerializeField] ScriptableObject[] output;
    [SerializeField] Peça.ConnexioEnum connexio;

    [Apartat("REFERENCIUES AUTO-CONFIGURABLES")]
    [SerializeField] Produccio produccio;

    //INTERN
    bool confirmat = true;
    List<object> estats;


    public ScriptableObject[] Inputs => inputs;





    /*public bool ConnexioConnectada(Peça peça)
    {
        if (connectada == Peça.ConnexioEnum.NoImporta)
            return true;

        if (!peça.Connectat)
            return false;

        return peça.Connexio.EstatConnexio.HasFlag(connectada);
    }*/

    public bool TeInputsIguals(Peça peça, List<object> inputs)
    {
        if (!HiHaInputs(inputs))
            return false;

        if (!ConnexioPropia(peça))
            return false;

        if(!InputPeça(inputs[0]))
            return ConfirmarRecepta(peça, inputs);

        //agafar els estats que tinguin les connexions com les demano.
        estats = new List<object>();
        for (int i = 0; i < inputs.Count; i++)
        {
            if (ConnexioNoImporta(inputs[i]))
            {
                estats.Add(((Peça)inputs[i]).Subestat);
                continue;
            }

            if (ConnectatAmbMi(ref estats, peça, inputs[i]))
                continue;

            if(ConnexionsIguals(inputs[i]))
                estats.Add(((Peça)inputs[i]).Subestat);
        }
        return ConfirmarRecepta(peça, estats);
    }

    bool HiHaInputs(List<object> inputsPassats) => inputsPassats.Count != 0;
    bool ConnexioPropia(Peça peça)
    {
        if (connexioPropia == Peça.ConnexioEnum.NoImporta)
            return true;

        return peça.GetEstatConnexio.HasFlag(connexioPropia);
    }
    bool InputPeça(object input) 
    {
        Debug.Log($"{input} is Peça? = {input is Peça}");
        return input is Peça;
    }
    bool ConnexioNoImporta(object input) => connexioInput.HasFlag(Peça.ConnexioEnum.NoImporta);
    bool ConnectatAmbMi(ref List<object> passats, Peça peça, object input)
    {
        if (connexioInput.HasFlag(Peça.ConnexioEnum.ConnectatAmbMi))
        {
            if (!((Peça)input).EstaConnectat)
                return true; //Ha d'estar connectat pero no ho està a res, per tant no el puc deixar contiuar

            if (((Peça)input).Connexio != peça)
                return true; //Ha d'estar connectat amb mi, pero no ho està amb mi. per tant no el puc deixar continuar

            passats.Add(((Peça)input).Subestat);
            return true; //Està connectat amb mi, per tant no cal que continui.
        }
        return false; //No està marcat com connectat per tant haig de continuar mirant.
    }
    bool ConnexionsIguals(object input) => connexioInput.HasFlag(((Peça)input).GetEstatConnexio);


    bool ConfirmarRecepta(Peça peça, List<object> passats)
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

        if (connexio.HasFlag(Peça.ConnexioEnum.NoImporta))
            return confirmat;

        if (confirmat)
        {
            switch (connexio)
            {
                case Peça.ConnexioEnum.Connectat:
                    peça.Connectar(((Peça)passats[0]));
                    break;
                case Peça.ConnexioEnum.Desconnectat:
                    peça.Desconnectar();
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

        if (produccio == null)
            return;

        produccio.AddProductor(peça);
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
