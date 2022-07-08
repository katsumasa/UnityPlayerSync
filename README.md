# UnityPlayerSync

UnityPlayerSyncはUnityEditorでビルドしたアプリケーション(UnityPlayer)とUnityEditorを同期する機能を追加するパッケージです。
このパッケージによって、UnityEditor上で下記のワークフローを実現することが可能です。

1. アプリケーションのSceneをUnityEditorに転送する
2. UnityEditor上でSceneの内容を編集する
3. 編集した内容をアプリケーションにリアルタイムで反映する

https://user-images.githubusercontent.com/29646672/177088255-24accd44-5e35-4e97-85bc-5c154905566e.mp4

## 使い方

### Sceneの同期

<img width="800" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177090991-41c6d907-5c85-4230-919a-529357d9f1d6.gif">

UnityPlayerからUnityEditor上にSceneを取り込む手順は下記の通りです。

1. Hierarchy Window上でSceneを選択
2. 右クリック
3. コンテキストメニューからSyncを選択

これによってアプリケーションで実行されているScene(Hierarchy)をUnity Editor上に展開します。

*Note*:複雑なSceneは取り込み完了迄に時間が掛かる場合があります。

### GameObjectの同期

<img width="400" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177096715-b2b47b6d-9200-4ff8-83cd-8f8138793ea7.png">

UnityEditor上で編集したGameObjectをUnityPlayer側にフィードバックする手順は下記の通りです。

1. Hierarchy Window上でフィードバックするGameObjectを選択
2. 右クリック
3. コンテキストメニューからSyncを選択

*Note*：UnityEditor上で新規に作成したGameObjectも上記の手順でUnityPlayerへ反映することが可能です。

### Componentの同期

<img width="800" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177094157-dcfced8b-dd36-4f4a-ac09-47da3c3b12c0.png">

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
  
*Note*:Componentのプロパティやフィールドの値に関しては下記の制限があります。

- System.ValuType(int,float,etc..)型と列挙型
- Componentへの参照
- メジャーなUnity Build-in struct(Color,Vector3,etc...)
- Asset(Texture,Mesh,Material,Shader,etc...)に関しては、Editor側で復元する場合は、Assetフォルダに存在する、Player側で復元する場合は[Resources.FindObjectsOfTypeAll](https://docs.unity3d.com/ja/current/ScriptReference/Resources.FindObjectsOfTypeAll.html)で検索可能なAssetに限定されます。

## Q&A

Q. Sceneの同期が出来ません

A. Profilerでプロファイルが出来ているか確認して下さい。プロファイルが出来ている場合は、セットアップの章で記載されている通りであるか見直して下さい。プロファイルが出来ていない場合、[こちら](https://docs.unity3d.com/ja/current/Manual/profiler-profiling-applications.html)をご確認下さい。

Q. UnityEditor上で編集したGameObjectがPlayerに反映されません。

A. Sceneの同期後、UnityEditor上でコンパイルが走ると同期情報が失われます。コンパイルが発生した場合は、Sceneの同期からやり直して下さい。
