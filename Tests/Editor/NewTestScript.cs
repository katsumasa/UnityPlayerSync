using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UTJ.UnityPlayerSyncEngine;

public class NewTestScript
{
    [Test]
    public void SandBox()
    {

        var t1 = new SyncType(typeof(Camera));
        var t2 = SyncType.GetType(t1);
        Debug.Log(t2.Assembly.FullName);
    }


    [Test]
    public void SyncSceneManagerTest()
    {
        var syncSceneManager = new SyncSceneManager();

        var memory = new MemoryStream();
        var writer = new BinaryWriter(memory);
        byte[] bytes;

        syncSceneManager.Serialize(writer);
        bytes = memory.ToArray();
        writer.Close();
        memory.Close();

        memory = new MemoryStream(bytes);
        var reader = new BinaryReader(memory);


        var syncSceneManager2 = new SyncSceneManager();
        syncSceneManager2.Deserialize(reader);
        syncSceneManager2.WriteBack();
    }



    [Test]
    public void SyncAllocTest()
    {
        object[] objs = { true,(byte)1,(char)1,(short)1,(int)1,(long)1,(sbyte)1,(float)1.0f,(string)"Test",(ushort)1,(uint)1,(long)1};

        foreach(var obj in objs)
        {
            SyncValueType.Allocater(obj.GetType().Name,obj);
            //Debug.Log(obj.GetType().Name);
        }
    }

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
            
            var syncValue = new SyncValueType<T>(value);

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
            var syncValue = new SyncValueType<T>(value);
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
