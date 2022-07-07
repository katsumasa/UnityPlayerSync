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


public class NewTestScript
{

    public CameraType[] cameraTypes;
    Camera camera;
    

    [Obsolete()]

    public List<CameraType> cameraTypesList;


    protected int m_protected;

    [Test]
    public void SandBox()
    {

        


        var t = this.GetType();
        var fis = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(var fi in fis)
        {
            
        }
    }


    [Test]
    public void SyncSceneManagerTest()
    {
        var syncSceneManager = new SyncSceneManager(true);

        var memory = new MemoryStream();
        var writer = new BinaryWriter(memory);
        byte[] bytes;

        syncSceneManager.Serialize(writer);
        bytes = memory.ToArray();
        writer.Close();
        memory.Close();



#if false
        memory = new MemoryStream(bytes);
        var reader = new BinaryReader(memory);
        var syncSceneManager2 = new SyncSceneManager(false);
        syncSceneManager2.Deserialize(reader);
        syncSceneManager2.WriteBack();
#endif
    }



    [Test]
    public void SyncAllocTest()
    {
        object[] objs = { true,(byte)1,(char)1,(short)1,(int)1,(long)1,(sbyte)1,(float)1.0f,(string)"Test",(ushort)1,(uint)1,(long)1};

        foreach(var obj in objs)
        {
            SyncValueObject.Allocater(obj.GetType(), obj.GetType(),obj);
            //Debug.Log(obj.GetType().Name);
        }

        var go = new GameObject();
        var camera = go.AddComponent(typeof(Camera));

        SyncValueObject.Allocater(typeof(Camera), typeof(Camera),camera);

    }

    [Test]   
    public void SyncGameObject()
    {        
        var gameObject = new GameObject();        
        var syncGameObject = new  SyncGameObject(gameObject);
        var memory = new MemoryStream();
        var writer = new BinaryWriter(memory);
        byte[] bytes;
        syncGameObject.Serialize(writer);

        bytes = memory.ToArray();
        
        writer.Close();
        memory.Close();

        memory = new MemoryStream(bytes);
        var reader = new BinaryReader(memory);
        var syncGameObject2 = new SyncGameObject(new GameObject());
        syncGameObject.Deserialize(reader);
        reader.Close();
        memory.Close();       
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

    
    [Test]
    public void SyncRectTest()
    {
        var r1 = new Rect(0.1f, 0.2f, 0.3f, 0.4f);

        var sync = new SyncRect(r1);
        var memory = new MemoryStream();
        var writer = new BinaryWriter(memory);
        byte[] bytes;

        sync.Serialize(writer);
        bytes = memory.ToArray();

        writer.Close();
        memory.Close();

        memory = new MemoryStream(bytes);
        var reader = new BinaryReader(memory);

        var r2 = new Rect();
        sync = new SyncRect(r2);
        sync.Deserialize(reader);
        reader.Close();
        memory.Close();

        var o = sync.GetValue();

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
            return (T)syncValue.GetValue();
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
