using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using XS_Utils;

public class TesterSimulacio
{
    [UnityTest]
    public IEnumerator TesterSimulacioWithEnumeratorPasses()
    {
        FasesControlador controlador = AssetDatabase.LoadAssetAtPath<FasesControlador>("Assets/XidoStudio/Hexbase/Sistemes/Fases/_Controlador.asset");
        Fase_Iniciar iniciar = AssetDatabase.LoadAssetAtPath<Fase_Iniciar>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Iniciar.asset");
        Fase_Colocar colocar = AssetDatabase.LoadAssetAtPath<Fase_Colocar>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Colocar.asset");
        Grid grid = GameObject.FindObjectOfType<Grid>();
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return new WaitForSeconds(3);
        XS_Coroutine.StartCoroutine(FindButton(controlador, iniciar, colocar));
    }

    IEnumerator FindButton(FasesControlador controlador, Fase_Iniciar iniciar, Fase_Colocar colocar)
    {
        iniciar.Iniciar();
        yield return new WaitForSeconds(3);

        Assert.AreEqual(true, controlador.Es(colocar));
    }
}
