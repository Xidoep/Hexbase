using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using XS_Utils;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class TesterSimulacio
{
    [UnityTest]
    public IEnumerator TesterSimulacioWithEnumeratorPasses()
    {
        SceneManager.LoadScene(0);

        //EditorSceneManager.LoadScene(0);
        //FasesControlador controlador = XS_Editor.LoadAssetAtPath<FasesControlador>("Assets/XidoStudio/Hexbase/Sistemes/Fases/_Controlador.asset");
        //Fase_Iniciar iniciar = XS_Editor.LoadAssetAtPath<Fase_Iniciar>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Iniciar.asset");
        //Fase_Colocar colocar = XS_Editor.LoadAssetAtPath<Fase_Colocar>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Colocar.asset");
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return new WaitForSeconds(3);

        Grid grid = GameObject.FindObjectOfType<Grid>();
        Debug.Log($"grid = {grid != null}");
        Assert.AreEqual(true, grid != null);
        //Assert.AreEqual(true, true);
        //XS_Coroutine.StartCoroutine(FindButton(grid, controlador, iniciar, colocar));
    }

    IEnumerator FindButton(Grid grid, FasesControlador controlador, Fase_Iniciar iniciar, Fase_Colocar colocar)
    {
        //iniciar.Iniciar();
        yield return new WaitForSeconds(3);
        Debug.LogError($"grid = {grid != null}");
        Assert.AreEqual(true, grid != null);
        //Assert.AreEqual(true, controlador.Es(colocar));
    }
}
