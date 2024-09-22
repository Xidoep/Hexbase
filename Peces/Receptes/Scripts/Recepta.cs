using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    public void Setup(Pe�a.ConnexioEnum connexioPropia, List<ScriptableObject> inputs, Pe�a.ConnexioEnum connexioInput, List<ScriptableObject> output, Pe�a.ConnexioEnum conneixo, int experiencia)
    {
        this.connexioPropia = connexioPropia;
        this.inputs = inputs.ToArray();
        this.connexioInput = connexioInput;
        this.output = output.ToArray();
        this.connexio = conneixo;
        this.experiencia = experiencia;
        AgafarProduccioSiCal();
    }

    [Tooltip("La connexio de la pe�a que porta la recepta ha de cohincidir amb el que s'ha posat aqu�.")]
    [BoxGroup("PROPI", centerLabel: true), Title("Estat connexio"), EnumToggleButtons, HideLabel, SerializeField, PropertySpace(0, spaceAfter: 20)] 
    Pe�a.ConnexioEnum connexioPropia;


    [BoxGroup("INPUTS", centerLabel: true), SerializeField] 
    ScriptableObject[] inputs;

    [Tooltip("En el cas que l'input sigui una Pe�a, la connexio d'aquesta ha de coincidir amb el que s'ha posat aqu�.")]
    [BoxGroup("INPUTS", centerLabel: true), Title("Estat connexio"), EnumToggleButtons, HideLabel, SerializeField, PropertySpace(0, spaceAfter: 20)] 
    Pe�a.ConnexioEnum connexioInput;

    [BoxGroup("OUTPUTS", centerLabel: true), SerializeField] 
    ScriptableObject[] output;

    [BoxGroup("OUTPUTS", centerLabel: true), Title("Accio connectar"), EnumToggleButtons, HideLabel, SerializeField, PropertySpace(0, spaceAfter: 20)] 
    Pe�a.ConnexioEnum connexio;

    [BoxGroup("OUTPUTS", centerLabel: true), SerializeField]
    int experiencia;

    [SerializeField, ReadOnly] Produccio produccio;

    //INTERN
    bool confirmat = true;
    bool trobat = false;
    List<Pe�a> peces;
    bool conte = false;



    public ScriptableObject[] Inputs => inputs;
    public ScriptableObject[] Outputs => output;
    public int Experiencia => experiencia;



    public bool ConteInput(Object estat)
    {
        conte = false;
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i].Equals(estat))
            {
                conte = true;
                break;
            }
        }
        return conte;
    }
    public bool ConteOutput(Object estat)
    {
        conte = false;
        for (int i = 0; i < output.Length; i++)
        {
            if (output[i].Equals(estat))
            {
                conte = true;
                break;
            }
        }
        return conte;
    }
    //public Pe�a.ConnexioEnum Connexio => connexio;




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
            return ConfirmarRecepta(inputs);

        //agafar els estats que tinguin les connexions com les demano.
        peces = new List<Pe�a>();
        for (int i = 0; i < inputs.Count; i++)
        {
            if (ConnexioNoImporta(inputs[i]))
            {
                /*
                 * clar, aquest es el primer filtre.
                 * s'han de confirmar tots els filtres abans de continuar.
                 */
                Debug.Log("Added perque la connexio no importa");
                peces.Add((Pe�a)inputs[i]);
                continue;
            }

            if (ConnectatAmbMi(ref peces, pe�a, inputs[i]))
                continue;

            if (ConnexionsIguals(inputs[i]))
            {
                Debug.Log($"Added perque et les connexions com demano. Busco ({connexioInput}) i he trobat ({((Pe�a)inputs[i]).GetEstatConnexio})");
                peces.Add((Pe�a)inputs[i]);
            }
        }
        return ConfirmarRecepta(pe�a, peces);
    }

    bool HiHaInputs(List<object> inputsPassats) => inputsPassats.Count != 0;
    bool ConnexioPropia(Pe�a pe�a)
    {
        if (connexioPropia == Pe�a.ConnexioEnum.NoImporta)
            return true;

        return pe�a.GetEstatConnexio == connexioPropia;
    }
    bool InputPe�a(object input) 
    {
        Debug.Log($"{input} is Pe�a? = {input is Pe�a}");
        return input is Pe�a;
    }
    bool ConnexioNoImporta(object input) => connexioInput == Pe�a.ConnexioEnum.NoImporta;
    bool ConnectatAmbMi(ref List<Pe�a> passats, Pe�a pe�a, object input)
    {
        if (connexioInput == Pe�a.ConnexioEnum.ConnectatAmbMi)
        {
            if (!((Pe�a)input).EstaConnectat)
                return true; //Ha d'estar connectat pero no ho est� a res, per tant no el puc deixar contiuar

            if (((Pe�a)input).Connexio != pe�a)
                return true; //Ha d'estar connectat amb mi, pero no ho est� amb mi. per tant no el puc deixar continuar

            Debug.Log("Added perque est� conncetada amb mi.");
            passats.Add((Pe�a)input);
            return true; //Est� connectat amb mi, per tant no cal que continui.
        }
        return false; //No est� marcat com connectat per tant haig de continuar mirant.
    }
    bool ConnexionsIguals(object input) => connexioInput == ((Pe�a)input).GetEstatConnexio;


    bool ConfirmarRecepta(List<object> passats)
    {
        confirmat = true;
        for (int i = 0; i < inputs.Length; i++)
        {
            for (int x = 0; x < passats.Count; x++)
            {
                Debug.Log($"{inputs[i]} == {passats[x]}? = {inputs[i] == passats[x]}");
            }

            if (!passats.Contains(inputs[i]))
            {
                confirmat = false;
                break;
            }
        }

        return confirmat;
    }
    bool ConfirmarRecepta(Pe�a pe�a, List<Pe�a> peces)
    {
        confirmat = true;
        for (int i = 0; i < inputs.Length; i++)
        {
            for (int p = 0; p < peces.Count; p++)
            {
                Debug.Log($"{inputs[i]} == {peces[p].Subestat}? = {inputs[i] == peces[p].Subestat}");
            }

            trobat = false;
            for (int p = 0; p < peces.Count; p++)
            {
                if(peces[p].Subestat == inputs[i])
                {
                    trobat = true;

                    if (connexio == Pe�a.ConnexioEnum.Connectat)
                        pe�a.Connectar(peces[p]);
                    else if (connexio == Pe�a.ConnexioEnum.Desconnectat)
                        pe�a.Desconnectar();

                    peces.Remove(peces[p]);
                    break;
                }
            }

            if (!trobat)
            {
                confirmat = false;
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

        AgafarProduccioSiCal();
    }

    void AgafarProduccioSiCal()
    {
        for (int i = 0; i < output.Length; i++)
        {
            if (output[i] is Producte)
            {
                produccio = XS_Utils.XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
                break;
            }
        }
    }

}
