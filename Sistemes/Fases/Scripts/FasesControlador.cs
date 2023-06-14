using UnityEngine;
using XS_Utils;
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Controller")]
public class FasesControlador : ScriptableObject
{
    [Nota("Només per debug")]
    [SerializeField] Fase actual;
    public Fase Actual
    {
        set {
            if(actual != null) Debugar.LogError($"{actual.name.ToUpper()}>>>");
            actual?.Finalitzar();
            actual = value;
            Debug.Log($">>>{actual.name.ToUpper()}");
            actual.FaseStart();
        }
    }

    public bool EstaEnFase(Fase fase) => actual == fase;

    private void OnEnable() => actual = null;
    private void OnDisable() => actual = null;
}

[System.Serializable]
public abstract class Fase : ScriptableObject
{
    [SerializeField] FasesControlador controlador;
    System.Action onFinish;
    System.Action onStart;

    //PROPIETATS
    public System.Action OnFinish { get => onFinish; set => onFinish = value; }
    public System.Action OnStart { get => onStart; set => onStart = value; }

    //INTERN
    protected object arg;
    
    //ABSTRACT
    public abstract void FaseStart();


    public void Iniciar() => Iniciar(null);
    public void Iniciar(object arg)
    {
        this.arg = arg;
        controlador.Actual = this;
        onStart?.Invoke();
    }
    public void Finalitzar() => onFinish?.Invoke();



    protected void OnDisable() => onFinish = null;
}
