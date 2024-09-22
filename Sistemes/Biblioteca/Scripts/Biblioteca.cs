using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Biblioteca")]
public class Biblioteca : ScriptableObject
{
    [System.Serializable]
    public class Convertible
    {
        public Convertible(Estat estat)
        {
            this.estat = estat;
            receptes = new List<Recepta>();
        }

        public Estat estat;
        public List<Recepta> receptes;
    }

    [SerializeField] Referencies referencies;

    [SerializeField] List<Estat> inicials;
    [SerializeField] List<Convertible> convertibles;
    private void OnValidate()
    {
        inicials = new List<Estat>();
        for (int i = 0; i < referencies.Colocables.Length; i++)
        {
            inicials.Add(referencies.Colocables[i].Estat);
        }

        convertibles = new List<Convertible>();
        for (int i = 0; i < referencies.Estats.Length; i++)
        {
            if (inicials.Contains(referencies.Estats[i]))
                continue;

            convertibles.Add(new Convertible(referencies.Estats[i]));
        }

        for (int c = 0; c < convertibles.Count; c++) 
        {
            for (int r = 0; r < referencies.Receptes.Length; r++)
            {
                if (referencies.Receptes[r].Outputs.Length == 0)
                    continue;

                if (referencies.Receptes[r].Outputs[0] is not Estat)
                    continue;

                if (referencies.Receptes[r].Outputs[0].Equals(convertibles[c].estat))
                {
                    convertibles[c].receptes.Add(referencies.Receptes[r]);
                }
            }
        }
    }
}
