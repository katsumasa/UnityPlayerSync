# UnityPlayerSync

UnityPlayerSyncはUnityでビルドしたアプリケーションとUnityEditorを同期させるパッケージです。
アプリケーションのSceneをUnityEditorに転送し、UnityEditor上で編集した内容をアプリケーションにリアルタイムで反映することを可能にします。

https://user-images.githubusercontent.com/29646672/177088255-24accd44-5e35-4e97-85bc-5c154905566e.mp4

## 使い方

Hierarchy Window上で何も選択していない状態で右クリック -> Syncを選択します。これによってアプリケーションで現在実行されているHierarchyをUnity Editor上に取り込みます。


<img width="800" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177090991-41c6d907-5c85-4230-919a-529357d9f1d6.gif">


UnityEditor上で編集した情報はComponentもしくはGameObjectの単位でアプリケーションに反映することが可能です。</br>
InsectorからComponentを選択した状態でContextMenu -> Syncで選択したComponentをアプリケーション側に反映します。


<img width="800" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177094157-dcfced8b-dd36-4f4a-ac09-47da3c3b12c0.png">


GameObjectの単位で反映を行う場合、Hierarchy Window上でGameObjectを選択した状態で右クリック -> Syncで選択します。


<img width="400" alt="Scene" src="https://user-images.githubusercontent.com/29646672/177096715-b2b47b6d-9200-4ff8-83cd-8f8138793ea7.png">

## セットアップ


### Packageの取得

UnityPlayerSyncは別途[RemoteConnect](https://github.com/katsumasa/RemoteConnect.git)パッケージを使用します。UnityPlayerSyncと合わせて取得して下さい。
パッケージの取得にgitを取得する場合、下記の通りです。

```:console
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/UnityPlayerSync.git
```


### Prefabの配置

ビルドの対象となるSceneにUnityPLayerSyncのPrefabを配置してください。


<img width="800" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100609-e0b11bac-d8b6-4cf6-a167-4053937f7545.gif">

### Scripting Backend

Scripting BackendにはMonoを指定する必要があります。
Editor -> Project Settings ->PlayerScripting BackendにMonoを設定してください。
<img width="400" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177101210-aa86af64-015e-440d-9ccb-9e121d572105.png">

### Build
Developmemt Build及びAutoconnect Profilerにチェックを入れてBuldを実行してください。
<img width="400" alt="Prefab" src="https://user-images.githubusercontent.com/29646672/177100904-b3b66b86-2d46-4230-aa17-b3cd0d0ecf2f.png">


## Tips

### 同期出来るobject

同期出来るobjectは下記の通りです。

1. Hierarchy内のGameObject
2. GameObjectにAddされているComponent
3. Component内でpublicなプロパティ及びSerializeアトリビュートを持つフィールドの値(Inspecter上で表示されているものと同義)
  

*Note*:但し3に関しては制限があります。

1.ValuType(int,float,etc..)の変数と列挙型
2.Componentへの参照
3.メジャーなUnity Build-in struct(Color,Vector3,etc...)
4.Asset(Texture,Mesh,Material,Shader,etc...)に関しては、Assetフォルダに存在するものを名前引きで使用

