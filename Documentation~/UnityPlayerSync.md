# UnityPlayerSync

UnityPlayerSync is a package that'll add a function that can sync with UnityEditor and an app built with UnityEditor(UnityPlayer).
This package allows the following workflow possible in the UnityEditor.

1. Transfer the application's Scene to UnityEditor
2. Edit Scene in the UnityEditor
3. Apply that edit to the application in real time 

<https://user-images.githubusercontent.com/29646672/177088255-24accd44-5e35-4e97-85bc-5c154905566e.mp4>

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

*Note that you could also create/import a new GameObject in UntyEditor and reflect it over to UnityPlayer with the same procedure.

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

### Obtaining Package

In order to use UnityPlayerSync, we also need the [RemoteConnect](https://github.com/katsumasa/RemoteConnect.git) as well. Proceed with the following if you're going to obtain it through git:

```:console
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/UnityPlayerSync.git
```

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

## Other

- Please report any requests, comments, or problems through Issue. We cannot promise, but will do our best to respond.
- When reporting an Issue, please attach a project that could reproduce the problem and describe the steps on how it can be replicated. If the problem cannot be reproduced, we may not be able to help.
