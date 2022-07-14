# UnityPlayerSync

UnityPlayerSyncはUnityEditorでビルドしたアプリケーション(UnityPlayer)とUnityEditorを同期する機能を追加するパッケージです。
このパッケージによって、UnityEditor上で下記のワークフローを実現することが可能です。

1. アプリケーションのSceneをUnityEditorに転送する
2. UnityEditor上でSceneの内容を編集する
3. 編集した内容をアプリケーションへリアルタイムに反映させる

<https://user-images.githubusercontent.com/29646672/177088255-24accd44-5e35-4e97-85bc-5c154905566e.mp4>

Note:同期と謳っていますが、正確には復元である為、復元出来ないケースも多々あります。

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

1. Inspecter Window上でフィードバックするComponentのコンテキストメニューを表示
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

ビルドの対象となるSceneにUnityPLayerSyncのPrefabを配置してください。

### Scripting Backend

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177101210-aa86af64-015e-440d-9ccb-9e121d572105.png">

Scripting BackendにはMonoを指定する必要があります。
Editor -> Project Settings ->PlayerScripting BackendにMonoを設定してください。

### Build

<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100904-b3b66b86-2d46-4230-aa17-b3cd0d0ecf2f.png">

Developmemt Build及びAutoconnect Profilerにチェックを入れてBuld( And Run)を実行してください。

## Tips

### 同期出来るobject

同期出来るobjectは下記の通りです。

- Hierarchy内のGameObject
- GameObjectにAddされているComponent
- Component内でpublicなプロパティ及びSerializeアトリビュートを持つフィールドの値(Inspecter上で表示されているものと同義) 

Componentのプロパティやフィールドの値に関しては下記の制限があります。

- System.ValuType(int,float,etc..)型と列挙型
- Componentへの参照
- メジャーなUnity Build-in struct(Color,Vector3,etc...)
- Asset(Texture,Mesh,Material,Shader,etc...)に関しては、Editor側で復元する場合は、Assetフォルダに存在する、Player側で復元する場合は[Resources.FindObjectsOfTypeAll](https://docs.unity3d.com/ja/current/ScriptReference/Resources.FindObjectsOfTypeAll.html)で検索可能なAssetに限定されます。（RenderTextureなど、Runtime上で生成されたAssetは同期出来ません。）

### 同期出来ないobject

同期出来ないオブジェクトの例は以下の通りです。

- LightmapやSkyboxのようなHierarchy上に存在しないデータ-
- Componentを継承しないClassや構造体(Unity Build-inのオブジェクトに関しては可能な限り対応しますが、packageで提供されているオブジェクトに関しては対応する予定はありません。)
- Runtime及びEditorどちらか一方にしか存在しないAsset
- ScriptableObject
- delegateやUnityEvent
- 多次元配列・ジャグ配列

## Q&A

Q. Sceneの同期が出来ません  
A. Profilerでプロファイルが出来ているか確認して下さい。プロファイルが出来ている場合は、セットアップの章で記載されている通りであるか見直して下さい。プロファイルが出来ていない場合、UnityEditorとデバイスの接続が正しく行われていない可能性があります。[こちら](https://docs.unity3d.com/ja/current/Manual/profiler-profiling-applications.html)を参考に、UnityEditorとデバイスの接続を行って下さい。
  
Q. UnityEditor上で編集したGameObjectがPlayerに反映されません  
A. Sceneの同期後、UnityEditor上でScriptのコンパイルが走ると同期情報が失われます。コンパイルが発生した場合は、Sceneの同期からやり直して下さい。

## その他

- 要望・ご意見・不具合に関してはIssueから報告をお願いします。約束は出来ませんが可能な限り対応します。
- 不具合報告に関してはそれを再現する為のプロジェクトの添付及び再現手順などの記述をお願いします。こちらで再現が取れない場合、対応出来ない場合があります。
