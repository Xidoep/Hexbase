using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Controller")]
public class FasesControlador : ScriptableObject
{

    [SerializeField] Fase actual;
    public Fase Actual
    {
        set
        {
            if(actual != null) Debug.LogError($"{actual.name.ToUpper()}>>>");
            actual?.Finalitzar();
            actual = value;
            Debug.LogError($">>>{actual.name.ToUpper()}");
            actual.Actualitzar();
        }
    }




    private void OnDisable()
    {
        actual = null;
    }

    /*public void Fase(Fase fase)
    {
        actual?.Finalitzar();
        actual = fase;
        actual.Iniciar();
    }
    public void Fase(Fase fase, object arg)
    {
        actual?.Finalitzar();
        actual = fase;
        actual.Iniciar(arg);
    }*/
}

[System.Serializable]
public abstract class Fase : ScriptableObject
{
    [SerializeField] FasesControlador controlador;
    System.Action onFinish;

    protected object arg;



    public void OnFinish_Invocar() => onFinish?.Invoke();
    public System.Action OnFinish { get => onFinish; set => onFinish = value; }


    public void Iniciar() => Iniciar(null);
    public void Iniciar(object arg = null)
    {
        this.arg = arg;
        controlador.Actual = this;
    }

    public abstract void Actualitzar();

    public abstract void Finalitzar();
}
