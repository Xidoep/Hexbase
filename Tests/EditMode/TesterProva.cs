using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class TesterProva
{
    // A Test behaves as an ordinary method
    [Test]
    public void TesterProvaSimplePasses()
    {
        Tester tester = AssetDatabase.LoadAssetAtPath<Tester>("Assets/XidoStudio/Hexbase/Sistemes/Tester/Tester.asset");
        Assert.AreEqual(true, tester.prova);
    }

}
