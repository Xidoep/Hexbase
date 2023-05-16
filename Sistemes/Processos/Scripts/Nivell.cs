using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Nivell")]
public class Nivell : ScriptableObject
{
    [SerializeField] int nivell = 1;
    [SerializeField] int experiencia = 0;
    [Space(20)]

    System.Action<int> enGuanyarExperiencia;
    System.Action<int> enPujarNivell;
    public System.Action<int> EnGuanyarExperiencia { get => enGuanyarExperiencia; set => enGuanyarExperiencia = value; }
    public System.Action<int> EnPujarNivell { get => enPujarNivell; set => enPujarNivell = value; }


    WaitForSeconds wfsExperiencia;
    WaitForSeconds wfsNivell;

    bool haPujatDeNivell;

    
    
    public bool HaPujatDeNivell { get => haPujatDeNivell; set => haPujatDeNivell = value; }

    public int ExperienciaNecessariaProximNivell => ProximNivell(nivell + 1);
    int ProximNivell(int nivell) => nivell * nivell * 10;

    public float FactorExperienciaNivellActual => (experiencia - (ProximNivell(nivell - 1))) / (float)((ProximNivell(nivell) - ProximNivell(nivell - 1)));



    //FUNCIONS PUBLIQUES
    public void GuanyarExperiencia(int experiencia, float delay) 
    {
        Debug.Log("Guanyar experiencia");
        this.experiencia += experiencia;
        XS_Coroutine.StartCoroutine(GuanyarExperiencia_Corrutina(delay));

        if(this.experiencia >= ProximNivell(nivell))
        {
            nivell++;
            haPujatDeNivell = true;
            XS_Coroutine.StartCoroutine(PujarNivell_Corrutina(delay + 1));
        }
    }
    IEnumerator GuanyarExperiencia_Corrutina(float delay)
    {
        wfsExperiencia = new WaitForSeconds(delay);
        yield return wfsExperiencia;
        enGuanyarExperiencia?.Invoke(experiencia);
    }
    IEnumerator PujarNivell_Corrutina(float delay)
    {
        wfsNivell = new WaitForSeconds(delay);
        yield return wfsNivell;
        enPujarNivell?.Invoke(nivell);
    }

    public void Reset()
    {
        nivell = 1;
        experiencia = 0;
        //enGuanyarExperiencia?.Invoke(nivell, this.experiencia);

        enGuanyarExperiencia = null;
        enPujarNivell = null;
    }









    //DEBUG
    [ContextMenu("mes 2")] void Prova2() => GuanyarExperiencia(2,1);
    [ContextMenu("mes 20")] void Prova20() => GuanyarExperiencia(20,1);
    [ContextMenu("mes 200")] void Prova200() => GuanyarExperiencia(200,1);

}
