using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UTJ.UnityPlayerSync.Runtime;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public class UnityPlayerSyncTestScript
{
    [Test]
    public void SandBox()
    {

        var t = typeof(Transform);
        

        var typeName = $"{t.FullName},{t.Assembly.GetName().Name}";
        var t1 = System.Type.GetType(typeName);
        Debug.Log(t1);
    }
    

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
