# UnityPlayerSync

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/UnityPlayerSync?style=plastic)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/katsumasa/UnityPlayerSync?style=plastic)

[Japanese Version README](https://github.com/katsumasa/UnityPlayerSync/blob/develop/README.md)

UnityPlayerSync is a package that'll add a function that can sync with UnityEditor and an app built with UnityEditor(UnityPlayer).
This package allows the following workflow possible in the UnityEditor.

1. Transfer the application's Scene to UnityEditor
2. Edit Scene in the UnityEditor
3. Apply that edit to the application in real time 

<https://user-images.githubusercontent.com/29646672/181483550-17334b57-63a5-4e8e-b257-fc58af7f1249.mp4>

Sample of sync with existing project(used Android emulator since it's easier to visualize).

GameKit2D  
<img width="800" alt="GameKit2D" src="https://user-images.githubusercontent.com/29646672/181436438-3a3e868d-7e98-4a9b-bc3a-97de5bbf62d4.gif">

Candy Rock Star  
<img width="800" alt="CRS" src="https://user-images.githubusercontent.com/29646672/181443227-5dfe513d-bac5-4c03-8d97-60101e0b72a7.gif">


*Note that UntyPlayerSync functions more like restoring limited information rather than a complete sync. There might be a case where the result might differ from the original scene in the UnityEditor. 

## How to Use

### Syncing Scene

<img width="800" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177090991-41c6d907-5c85-4230-919a-529357d9f1d6.gif">

Here are the procedures for importing a Scene from UnityEditor to UnityPlayer:

1. Create a New Scene
2. Select that New Scene that you've created inside the Hierarchy tab
3. Right Click 
4. Select Sync from the Context Menu

With this, UnityEditor can reflect the deployed Scene(Hierarchy) inside the app.

*Note*:

- It may take some time to import depending on the size of the Scene.
- You don't necessarily have to create a New Scene, but please keep in mind that if you deploy changes to the existing Scene, the changes made can't be saved.

### Syncing GameObject

<img width="400" alt="GameObject" src="https://user-images.githubusercontent.com/29646672/177096715-b2b47b6d-9200-4ff8-83cd-8f8138793ea7.png">

Here are the procedures for updating UnityPlayer for modified GameOjects in UnityEditor.

1. Select the GameObject on the Hierarchy Window you wish to update
2. Right click
3. Select Sync from the Context Menu

*Note* You could also create/import a new GameObject in UntyEditor and reflect it over to UnityPlayer with the same procedure.

### Syncing Component

<img width="800" alt="Component" src="https://user-images.githubusercontent.com/29646672/177094157-dcfced8b-dd36-4f4a-ac09-47da3c3b12c0.png">

Since it can sync with Components, a fast syncing experience can be possible in UnityPlayer compared to GameObject Sync.

1. Show the Context Menu of the Component you wish to display to the Inspector Window
2. Select Sync from the Context Menu

### Deleting GameObject

You can also delete the GameObject in the Player by following steps:

1. Select the GameObject on the Hierarchy Window you wish to delete
2. Right click
3. Select Sync Delete from the Context Menu

### Deleting Component

After deleting the Component from the GameObject and syncing again, the Component on the Player side is also deleted as well.

## Setting UnityPlayerSync

In order to use UnityPlayerSync, we also need the [RemoteConnect](https://github.com/katsumasa/RemoteConnect.git) as well. The setup is complete once the package was added to the Unity project.

### Getting the Package

The package is managed by GitHub including any future updates.

### When using command line to get the package through git 

```:console
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/UnityPlayerSync.git
```
### When getting the package from a web

1. Go to [UnityPlayerSync](https://github.com/katsumasa/UnityPlayerSync) page and click on [RemoteConnect](https://github.com/katsumasa/RemoteConnect). Go to Code > Download ZIP on the top right of the page. 
<img width="800" alt="Code" src="https://user-images.githubusercontent.com/29646672/183364441-1dd3da87-be04-419c-b060-448817ec0cec.gif">
2. Extract the Zip file

### Placing Prefab

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100609-e0b11bac-d8b6-4cf6-a167-4053937f7545.gif">

Place UnityPlayerSync Prefab in the scene you wish to build.

### Scripting Backend

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177101210-aa86af64-015e-440d-9ccb-9e121d572105.png">

Set the Scripting Backend to Mono.
Go to Editor -> Project Settings ->PlayerScripting and assign the Backend to Mono.

### Build

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100904-b3b66b86-2d46-4230-aa17-b3cd0d0ecf2f.png">

Put a check in the Development Build and Autoconnect Profiler and execute Build(And Run).

### Operational Test

Once the build is complete and the app is running on the actual device, it's finally time to test.
1. Select `File > New Scene` in UnityEditor 
2. With no GameObject selected in the `Hierarchy View`, right-click and execute Sync. After waiting for a while, Scene information will appear in the Hierarchy View on the actual device's screen.
3. Select any GameObject and change the value of Transformation inside the Inspector tab.
4. With the GameObject selected in the Hierarchy View, right click and execute Sync. You should be able to confirm that the Transform value of the corresponding GameObject is updated on the actual device.

![402f527086e746b601a17ddcc6f9d09a](https://user-images.githubusercontent.com/29646672/183376337-ad6b1073-44fe-4330-a649-6801dba3e421.gif)

### UnityPlayerSyncEditorWindow

<img width="400" alt="image" src="https://user-images.githubusercontent.com/29646672/183376759-3a074cbc-8da8-4959-8977-42d1bbcda4b4.png">

Slect Window > UTJ > UnityPlayerSync. This window pops up once the UnityEditor is connected to the actual device.

#### ConnectTo

If multiple devices are connected to UnityEditor and this allows you to switch from one device to another. This value works syncs with UnityProfiler.

#### SYNC

Allows you to relect the Scene on the actual device to UnityEditor. This works the same as right-click and execute Sync without selecting any GameObject in Hierarchy.

#### GC

Executes System.GC.Collect() on the actual device.

#### UnloadUnusedAssets

Executes Resources.UnloadUnusedAssets() on the actual device.

## Tips

### Objects that can be synced

Here are the Objects that can be synced:

- Hierarchy within the GameObject
- Component that's added to the GameObject
- Values of public properties and serialized attributes inside the Component (The same value displayed by the Inspector).

There are restrictions to Component properties and field values:

- System.ValueType(int, float, etc..) and Enumerated type
- Reference to Component
- Major Unity Build-in struct(Color, Vector3, etc...)
- When restoring an Asset(Texture, Mesh, Material, Shader, etc...) on the Editor side, it is limited to what exists within the Project. When restoring an Asset on the Player side, it is limited to what exists within the Hierarchy.

### Objects that can't be synced

Here are the following Objects that can't be synced:

- Data that doesn't exist in Hierarchy (e.g. Lightmap, Skybox).
- Classes and structures that don't inherit from the Component (I will try to support Unity's Built-in objects as much as possible, but not for objects provided in the package).
- Asset that exists only in either Runtime or Editor
- ScriptableObject
- delegate, UnityEvent
- Multidimensional and Jagged arrays
- Multidimensional and Jagged arrays

  and etc ・・・

## Q&A

Q. Unable to sync 1
A. Check if the profile is created using the Profiler. If the Profile was properly created, please check the Setting UnityPlayerSync section on this page to see if there's any procedure that might get skipped. If the Profile wasn't created, there might be a possibility of UnityEditor and the device not being connected properly. Please refer [here](https://docs.unity3d.com/2021.3/Documentation/Manual/profiler-profiling-applications.html) to connect the device to UnityEditor.

Q. Unable to sync 2
A. If you wish to delete a synced GameObject, be sure to delete using `Sync Delete`. Inconsistent sync may cause an unexpected error.

Q. Unable to sync 3
A. If Compile runs on UnityEditor after syncing to a Scene, it could lose the synced information. Be sure to resync the Scene if a Compile occurs.

Q. The result of the UI looks different in the Player display when I applied sync after making changes to the UI in Editor. 
A. This is due to the difference between the screen size of the device and the size of the GameView in the Editor. Try to match the size of the GameView in the Editor to Canvas Scaler's Reference Resolution value. 

Q. Out of Memory occurs during the sync
A. The more Objects you put in a Scene, the more memory is secured from the managed memory. Such complex Scene can be synced by enabling UnityPlayerSyncPlayer's `Use Stream Buffer Capacity` and adjust the `Capacity Size[MB]`(Setting large value might lead to Out of Memory before the sync).

![b223d923249ba487d60ff4c016f86624](https://user-images.githubusercontent.com/29646672/181192810-38ab173c-8f97-497c-b8b9-f8c6eab1b90e.png)

Q. Unable to complete the Sync 
A. The more Object in the Scene, the time for Sync to complete increases. As a reference, GameKit3D's* Start Scene takes about 10 seconds to sync, but the In-game Scene takes about 5 minutes to sync. 

*GameKit3D is a sample Project provided by Unity.


## Other

- Please report any requests, comments, or problems through [Issue](https://github.com/katsumasa/UnityPlayerSync/issues). We cannot promise, but will do our best to respond.
- When reporting an [Issue](https://github.com/katsumasa/UnityPlayerSync/issues), please attach a project that could reproduce the problem and describe the steps on how it can be replicated. If the problem cannot be reproduced, we may not be able to help.
