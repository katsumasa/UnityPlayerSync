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

UnityPlayerSyncは別途[RemoteConnect](https://github.com/katsumasa/RemoteConnect.git)パッケージを使用します。UnityPlayerSyncと合わせて取得して下さい。  　
取得したパッケージを任意のUnityプロジェクトへ追加することでセットアップは完了です。

### パッケージの取得方法

パッケージはGitHubで管理されており、取得方法の例を下記へ記載します。


#### コマンドラインのgitを使用してパッケージ取得する場合

```:console
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/UnityPlayerSync.git
```

#### Webページから取得する場合

1. [UnityPlayerSync](https://github.com/katsumasa/UnityPlayerSync)及び[RemoteConnect](https://github.com/katsumasa/RemoteConnect)のWebページへアクセスし、それぞれ画面右上のCode > Download Zipを選択し、ZIPファイルを取得する
<img width="800" alt="Code" src="https://user-images.githubusercontent.com/29646672/183364441-1dd3da87-be04-419c-b060-448817ec0cec.gif">
2. ZIPファイルを解凍する

#### PackageManagerで取得する[お勧め]

1. Window > Package ManagerでPackage Managerを開く
2. Package Manager左上の+のプルダウンメニューからAdd package form git URL...を選択する
3. ダイアログへ`https://github.com/katsumasa/RemoteConnect.git`を設定し、Addボタンを押す
4. Package Manager左上の+のプルダウンメニューからAdd package form git URL...を選択する
5. ダイアログへ `https://github.com/katsumasa/UnityPlayerSync.git`を設定し、Addボタンを押す

<img width="813" alt="image" src="https://user-images.githubusercontent.com/29646672/183819326-f43d7fde-5cf9-469f-bd8b-02d0c52f4fa4.png">

※RemoteConnectを先に追加する必要があることに注意して下さい。



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

### 動作確認

ビルドが完了し、実機上でアプリケーションが実行されたらいよいよ動作確認です。　　
1. UnityEditor上で`File > New Scene`を選択します。
2. `Hierarchy View`上でGameObjectを何も選択していない状態で右クリック > Syncを実行します。しばらく待つとHierarchy Viewに実機上のアプリにScene情報が構築されます。
3. 適当なGameObjectを選択し、InspectorからTransformの値を変更してみましょう。
4. Hierarchy ViewでGameObjectを選択したまま、右クリック > Syncを実行します。実機上でも該当するGameObjectのTransformの値が更新されることが確認出来る筈です。

![402f527086e746b601a17ddcc6f9d09a](https://user-images.githubusercontent.com/29646672/183376337-ad6b1073-44fe-4330-a649-6801dba3e421.gif)


### UnityPlayerSyncEditorWindow

<img width="400" alt="image" src="https://user-images.githubusercontent.com/29646672/183376759-3a074cbc-8da8-4959-8977-42d1bbcda4b4.png">

Window > UTJ > UnityPlayerSyncを選択する、もしくは実機とUnityEditorが接続すると自動的に開くWindowです。

#### ConnectTo

UnityEditorに複数のデバイスが接続されている場合、接続先を切り替えることが可能です。この値はUnityProfilerと連動しています。

#### SYNC

デバイス上のSceneをUnityEditorへ展開します。
Hierarchy ViewからGameObjectを選択せずに、右クリック > Syncを実行した時と同じです。

#### GC

実機上でSystem.GC.Collect()を実行します。

#### UnloadUnusedAssets

実機上でResources.UnloadUnusedAssets()を実行します。

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

![image](https://user-images.githubusercontent.com/29646672/183591982-a5b3c71f-370f-4855-aedc-da448a74e614.png)

Q. 同期が終わりません。  
A. Sceneを構成するObjectと比例する形で、同期にかかる時間が伸びて行きます。Unityが提供しているサンプルプロジェクトGameKit3Dを例にとると、Start Sceneは10秒程度で同期が完了しますが、インゲーム中のSceneでは同期に5分以上かかります。


Q.[UnityPlayerSync](https://github.com/katsumasa/UnityPlayerSync)と[UnityChoseKun](https://github.com/katsumasa/UnityChoseKun)の違いを教えて下さい。
A.UnityChoseKunはアプリケーション上のHierarchyの情報及び必要最低限のComponentの情報を取得し、その情報をアプリケーションへダイレクトに反映し、UnityPlayerSyncはアプリケーションのHierarchyをUnityEditor上にそのまま再現し、変更された内容をアプリケーションに反映します。 その為、UnityPlayerSyncはUnityChoseKunよりも得られる情報量が多い一方、アプリケーションとUnityEditorの同期にかかる時間はUnityChoseKunの方が短くなっています。

例えば、アプリケーションのパラメーターを調整してパフォーマンスチューニングや見た目の調整を行うような用途であればUnityChoseKunが適しています。 一方、UnityPlayerSyncは通常のUnityEditorのワークフローと殆ど変わらないGUIで操作することが出来る為、エンジニア以外のクリエーターでも直観的に操作が出来るというメリットがあります。

## その他

- 要望・ご意見・不具合に関しては[Issue](https://github.com/katsumasa/UnityPlayerSync/issues)から報告をお願いします。約束は出来ませんが可能な限り対応します。
- 不具合報告に関してはそれを再現する為のプロジェクトの添付及び再現手順などの記述をお願いします。こちらで再現が取れない場合、対応出来ない場合があります。
