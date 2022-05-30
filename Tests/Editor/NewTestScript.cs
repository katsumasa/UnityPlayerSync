using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UTJ.UnityPlayerSyncEngine;

public class NewTestScript
{
    [Test]   
    public void SyncGameObject()
    {
        var gameObject = new GameObject();
        var syncGameObject = new  SyncGameObject(gameObject);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        var v2 = new Vector2();
        var t = v2.GetType();


        int a = 100;
        var b = TestSyncValue<int>(a);
        UnityEngine.Assertions.Assert.AreEqual(a, b);
    }


    T TestSyncValue<T>(T value)
    {
        var memory = new MemoryStream();
        var writer = new BinaryWriter(memory);
        byte[] bytes;
        try
        {
            
            var syncValue = new SyncValue<T>(value);

            syncValue.Serialize(writer);
            bytes = memory.ToArray();
        }
        finally
        {
            writer.Close();
            memory.Close();
        }
        
        memory = new MemoryStream(bytes);
        var reader = new BinaryReader(memory);
        try
        {
            var syncValue = new SyncValue<T>(value);
            syncValue.Deserialize(reader);
            return syncValue.value;
        }
        finally
        {
            reader.Close();
            memory.Close();
        }
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
