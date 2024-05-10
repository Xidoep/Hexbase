using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
public class Recepta : ScriptableObject
{
    public void Setup(Peça.ConnexioEnum connexioPropia, List<ScriptableObject> inputs, Peça.ConnexioEnum connexioInput, List<ScriptableObject> output, Peça.ConnexioEnum conneixo, int experiencia)
    {
        this.connexioPropia = connexioPropia;
        this.inputs = inputs.ToArray();
        this.connexioInput = connexioInput;
        this.output = output.ToArray();
        this.connexio = conneixo;
        this.experiencia = experiencia;
        AgafarProduccioSiCal();
    }

    [Tooltip("La connexio de la peça que porta la recepta ha de cohincidir amb el que s'ha posat aquí.")]
    [BoxGroup("PROPI", centerLabel: true), Title("Estat connexio"), EnumToggleButtons, HideLabel, SerializeField, PropertySpace(0, spaceAfter: 20)] 
    Peça.ConnexioEnum connexioPropia;


    [BoxGroup("INPUTS", centerLabel: true), SerializeField] 
    ScriptableObject[] inputs;

    [Tooltip("En el cas que l'input sigui una Peça, la connexio d'aquesta ha de coincidir amb el que s'ha posat aquí.")]
    [BoxGroup("INPUTS", centerLabel: true), Title("Estat connexio"), EnumToggleButtons, HideLabel, SerializeField, PropertySpace(0, spaceAfter: 20)] 
    Peça.ConnexioEnum connexioInput;

    [BoxGroup("OUTPUTS", centerLabel: true), SerializeField] 
    ScriptableObject[] output;

    [BoxGroup("OUTPUTS", centerLabel: true), Title("Accio connectar"), EnumToggleButtons, HideLabel, SerializeField, PropertySpace(0, spaceAfter: 20)] 
    Peça.ConnexioEnum connexio;

    [BoxGroup("OUTPUTS", centerLabel: true), SerializeField]
    int experiencia;

    [SerializeField, ReadOnly] Produccio produccio;

    //INTERN
    bool confirmat = true;
    bool trobat = false;
    List<Peça> peces;
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
    //public Peça.ConnexioEnum Connexio => connexio;




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
            return ConfirmarRecepta(inputs);

        //agafar els estats que tinguin les connexions com les demano.
        peces = new List<Peça>();
        for (int i = 0; i < inputs.Count; i++)
        {
            if (ConnexioNoImporta(inputs[i]))
            {
                /*
                 * clar, aquest es el primer filtre.
                 * s'han de confirmar tots els filtres abans de continuar.
                 */
                Debug.Log("Added perque la connexio no importa");
                peces.Add((Peça)inputs[i]);
                continue;
            }

            if (ConnectatAmbMi(ref peces, peça, inputs[i]))
                continue;

            if (ConnexionsIguals(inputs[i]))
            {
                Debug.Log($"Added perque et les connexions com demano. Busco ({connexioInput}) i he trobat ({((Peça)inputs[i]).GetEstatConnexio})");
                peces.Add((Peça)inputs[i]);
            }
        }
        return ConfirmarRecepta(peça, peces);
    }

    bool HiHaInputs(List<object> inputsPassats) => inputsPassats.Count != 0;
    bool ConnexioPropia(Peça peça)
    {
        if (connexioPropia == Peça.ConnexioEnum.NoImporta)
            return true;

        return peça.GetEstatConnexio == connexioPropia;
    }
    bool InputPeça(object input) 
    {
        Debug.Log($"{input} is Peça? = {input is Peça}");
        return input is Peça;
    }
    bool ConnexioNoImporta(object input) => connexioInput == Peça.ConnexioEnum.NoImporta;
    bool ConnectatAmbMi(ref List<Peça> passats, Peça peça, object input)
    {
        if (connexioInput == Peça.ConnexioEnum.ConnectatAmbMi)
        {
            if (!((Peça)input).EstaConnectat)
                return true; //Ha d'estar connectat pero no ho està a res, per tant no el puc deixar contiuar

            if (((Peça)input).Connexio != peça)
                return true; //Ha d'estar connectat amb mi, pero no ho està amb mi. per tant no el puc deixar continuar

            Debug.Log("Added perque està conncetada amb mi.");
            passats.Add((Peça)input);
            return true; //Està connectat amb mi, per tant no cal que continui.
        }
        return false; //No està marcat com connectat per tant haig de continuar mirant.
    }
    bool ConnexionsIguals(object input) => connexioInput == ((Peça)input).GetEstatConnexio;


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
    bool ConfirmarRecepta(Peça peça, List<Peça> peces)
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

                    if (connexio == Peça.ConnexioEnum.Connectat)
                        peça.Connectar(peces[p]);
                    else if (connexio == Peça.ConnexioEnum.Desconnectat)
                        peça.Desconnectar();

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
