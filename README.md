# UnityPlayerSync

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/UnityPlayerSync?style=plastic)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/katsumasa/UnityPlayerSync?style=plastic)

[English Version README](https://github.com/katsumasa/UnityPlayerSync/blob/develop/Documentation~/UnityPlayerSync.md)

UnityPlayerSyncはUnityEditorでビルドしたアプリケーション(UnityPlayer)とUnityEditorを同期する機能を追加するパッケージです。
このパッケージによって、UnityEditor上で下記のワークフローを実現することが可能です。

1. アプリケーション上のSceneをUnityEditorに転送する
2. UnityEditor上で上記のSceneの内容を編集する
3. 編集した内容をリアルタイムでアプリケーションに反映させる


https://user-images.githubusercontent.com/29646672/181483550-17334b57-63a5-4e8e-b257-fc58af7f1249.mp4


既存のプロジェクトによる同期の例(撮影の都合上、Androidエミュレーターを使用しています。）

GameKit2D  
<img width="800" alt="GameKit2D" src="https://user-images.githubusercontent.com/29646672/181436438-3a3e868d-7e98-4a9b-bc3a-97de5bbf62d4.gif">

Candy Rock Star  
<img width="800" alt="CRS" src="https://user-images.githubusercontent.com/29646672/181443227-5dfe513d-bac5-4c03-8d97-60101e0b72a7.gif">


Note:正確には同期ではなく、限られた情報からの復元である為、復元出来ないケースも多々あります。

## 使い方

### Sceneの同期

<img width="800" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177090991-41c6d907-5c85-4230-919a-529357d9f1d6.gif">

UnityPlayerからUnityEditor上にSceneを取り込む手順は下記の通りです。

1. File > New Sceneを選択して新しいSceneを作成する
2. Hierarchy Window上でScene(New Sceneを作成した直後はUntitled)を選択
3. 右クリック
4. コンテキストメニューからSyncを選択

これによってアプリケーションで実行されているScene(Hierarchy)をUnity Editor上に展開します。

*Note*:

- Sceneの規模に応じて取り込み完了迄に時間が掛かる場合があります。
- 必ずしも新しいSceneを作る必要はありませんが、既存のSceneの変更内容は保存されず閉じられることに注意して下さい。

### GameObjectの同期

<img width="400" alt="GameObject" src="https://user-images.githubusercontent.com/29646672/177096715-b2b47b6d-9200-4ff8-83cd-8f8138793ea7.png">

UnityEditor上で編集したGameObjectをUnityPlayer側にフィードバックする手順は下記の通りです。

1. Hierarchy Window上でフィードバックするGameObjectを選択
2. 右クリック
3. コンテキストメニューからSyncを選択

*Note*：UnityEditor上で新規に作成したGameObjectも上記の手順でUnityPlayerへ反映することが可能です。

### Componentの同期

<img width="800" alt="Component" src="https://user-images.githubusercontent.com/29646672/177094157-dcfced8b-dd36-4f4a-ac09-47da3c3b12c0.png">

Component単体で同期を行うことで、GameObjectの同期と比較して素早くUnityPlayerと同期を行うことが可能です。

1. Inspector Window上でフィードバックするComponentのコンテキストメニューを表示
2. コンテキストメニューからSyncを選択

### GameObjectの削除

下記の手順でPlayer上のGameObjectを削除することが可能です。

1. Hierarchy Window上でフィードバックするGameObjectを選択
2. 右クリック
3. コンテキストメニューからSync Deleteを選択

### Componentの削除

GameObjectからComponentを削除後、GameObjectの同期を行うことでPlayer上のGameObjectのComponentも削除されます。

## セットアップ

### Packageの取得

UnityPlayerSyncは別途[RemoteConnect](https://github.com/katsumasa/RemoteConnect.git)パッケージを使用します。UnityPlayerSyncと合わせて取得して下さい。
パッケージの取得にgitを取得する場合、下記の通りです。

```:console
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/UnityPlayerSync.git
```

### Prefabの配置

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100609-e0b11bac-d8b6-4cf6-a167-4053937f7545.gif">

ビルドの対象となるSceneにUnityPlayerSyncのPrefabを配置してください。

### Scripting Backend

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177101210-aa86af64-015e-440d-9ccb-9e121d572105.png">

Scripting BackendにはMonoを指定する必要があります。
Editor -> Project Settings ->PlayerScripting BackendにMonoを設定してください。

### Build

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100904-b3b66b86-2d46-4230-aa17-b3cd0d0ecf2f.png">

Development Build及びAutoconnect Profilerにチェックを入れてBuld( And Run)を実行してください。

## Tips

### 同期出来るobject

同期出来るobjectは下記の通りです。

- Hierarchy内のGameObject
- GameObjectにAddされているComponent
- Component内でpublicなプロパティ及びSerializeアトリビュートを持つフィールドの値(Inspector上で表示されているものと同義)

Componentのプロパティやフィールドの値に関しては下記の制限があります。

- System.ValueType(int,float,etc..)型と列挙型
- Componentへの参照
- メジャーなUnity Build-in struct(Color,Vector3,etc...)
- Asset(Texture,Mesh,Material,Shader,etc...)に関しては、Editor側で復元する場合は、プロジェクトの存在するAsset、Player側で復元する場合はHierarchy上に存在しているAssetに限定されます。

### 同期出来ないobject

同期出来ないオブジェクトの例は以下の通りです。

- LightmapやSkyboxのようなHierarchy上に存在しないデータ-
- Componentを継承しないClassや構造体(Unity Build-inのオブジェクトに関しては可能な限り対応しますが、packageで提供されているオブジェクトに関しては対応する予定はありません。)
- Runtime及びEditorどちらか一方にしか存在しないAsset
- ScriptableObject
- delegateやUnityEvent
- 多次元配列・ジャグ配列
- Particleのパラメーター

などなど・・・

## Q&A

Q. 同期が出来ません  
A. Profilerでプロファイルが出来ているか確認して下さい。プロファイルが出来ている場合は、セットアップの章で記載されている通りであるか見直して下さい。プロファイルが出来ていない場合、UnityEditorとデバイスの接続が正しく行われていない可能性があります。[こちら](https://docs.unity3d.com/ja/current/Manual/profiler-profiling-applications.html)を参考に、UnityEditorとデバイスの接続を行って下さい。

Q. 同期出来ません。  
A. 同期済みのGameObjectを削除する場合は必ず、`Sync Delete`から削除を行って下さい。同期の整合性が取れなくなり想定出来ないエラーが発生する場合があります。

Q. 同期が出来ません
A. Sceneの同期後、UnityEditor上でScriptのコンパイルが走ると同期情報が失われます。コンパイルが発生した場合は、Sceneの同期からやり直して下さい。

Q. Editor上でUIを変更するとPlayer側の表示がおかしくなります。  
A. デバイスのスクリーンサイズとEditor上のGameViewのサイズが一致していないのが原因だと思われます。Editor上のGameViewのサイズをCanvas ScalerのReference Resolutionの値に合わせてみて下さい。

Q. 同期中にOut of Memoryが発生します。  
A. Sceneを構成するObjectが多くなる程マ、ネージドメモリから確保するメモリ量が多くなって行きます。UnityPlayerSyncPlayerの`Use Stream Buffer Capacity`を有効にし、`Capacity Size[MB]`で適切な値を設定することで複雑なシーンでも同期が成功する場合があります。（あまり大きな値を設定すると同期前にOut of Memoryが発生するので注意して下さい。)

![b223d923249ba487d60ff4c016f86624](https://user-images.githubusercontent.com/29646672/181192810-38ab173c-8f97-497c-b8b9-f8c6eab1b90e.png)


Q. 同期が終わりません。  
A. Sceneを構成するObjectと比例する形で、同期にかかる時間が伸びて行きます。Unityが提供しているサンプルプロジェクトGameKit3Dを例にとると、Start Sceneは10秒程度で同期が完了しますが、インゲーム中のSceneでは同期に5分以上かかります。

## その他

- 要望・ご意見・不具合に関しては[Issue](https://github.com/katsumasa/UnityPlayerSync/issues)から報告をお願いします。約束は出来ませんが可能な限り対応します。
- 不具合報告に関してはそれを再現する為のプロジェクトの添付及び再現手順などの記述をお願いします。こちらで再現が取れない場合、対応出来ない場合があります。
