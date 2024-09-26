using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System;
using System.IO;
using System.Net;

public class CatalogNamer : MonoBehaviour
{
    MyButton button => transform.Find("Decide").gameObject.AddComponent<MyButton>();
    MyInputField inputField;

    public string BufferDir;
    public string ContentsDir;
    public string ContentsName;


    private async void Awake()
    {
        inputField = transform.Find("InputField").gameObject.AddComponent<MyInputField>();

        button.On_Click.Subscribe( async _ => 
        {
            ContentsName = inputField.text_Input.text;
            if (string.IsNullOrEmpty(ContentsName)) return;

            

            await new ContentsDivider(BufferDir, ContentsDir, ContentsName).Execute();
            Debug.Log("カタログの取り込み完了");

            Finish();
        });
    }


    void Finish()
    {
        if (Directory.Exists(BufferDir)) Directory.Delete(BufferDir, true);
        inputField._Text.Value = string.Empty;
        gameObject.SetActive(false);
    }

    //// 該当コンテンツのアドレス一覧を取得
    //public async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator)
    //{
    //    Addressables.AddResourceLocator(resourceLocator);

    //    List<string> addresses = new List<string>();
    //    List<string> loadedAddresses = new List<string>();

    //    // カタログ(メモリ上ではIResourceLocatorとして扱われる)からアドレス一覧を抽出
    //    // IResourceLocator は key(通常はAdress)と、それに紐づくアセットの対応関係の情報を持っている
    //    foreach (var a in resourceLocator.Keys)
    //    {
    //        addresses.Add(a.ToString());
    //    }

    //    // ロードできるものを判別して、今回ロードしたいタイプのアセットか判別して、全部ロード
    //    foreach (var a in addresses)
    //    {
    //        Debug.Log($"------ アドレス : {a}");
    //        IList<IResourceLocation> resourceLocations;
    //        // IResourceLocation はアセットをロードするために必要な情報を持っている
    //        // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
    //        // つまりカタログが IResourceLocation を内包している
    //        if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


    //        foreach (var b in resourceLocations)
    //        {
    //            Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
    //            // IResourceLocation の PrimaryKey がアドレスらしい
    //            AsyncOperationHandle<GameObject> opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
    //            await opHandle_LoadedGObj;

    //            // ロードできなかったら次のループへ
    //            if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
    //            Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

    //            // シーン内の物体としてのゲームオブジェクト(つまりRendererコンポーネントがついているはず)かどうかを判別して、
    //            // 違ったら次のループへ
    //            Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
    //            if (renderers.Length == 0) continue;
    //            Debug.Log($"------ レンダラを持つゲームオブジェクト : {opHandle_LoadedGObj.Result}");

    //            // 今回のアドレスを、ロード済みのアドレス一覧loadedAddressesに記録
    //            if (loadedAddresses.Contains(b.PrimaryKey)) continue;
    //            loadedAddresses.Add(b.PrimaryKey);
    //        }
    //    }

    //    Addressables.RemoveResourceLocator(resourceLocator);
    //    return loadedAddresses;
    //}
}



class ContentsDivider
{
    public string BufferDir;
    public string DLContentsDir;
    public string ContentsDir;
    public string ContentsName;

    public string AddressablesDir;
    public string CatalogName;
    public string HashName;
    public string Hash;


    // アセットバンドルグループのパス一を全取得
    public List<string> GroupPaths;
    // フォルダ振り分け後のCatalogのパス
    public string CatalogPath;

    public List<string> labels = new List<string>
    {
        "部屋",
        "アバター",
        "モーション",
        "空",
        "家具"
    };

    public ContentsDivider(string bufferDir, string contentsDir, string contentsName)
    {
        // バンドルの保存先
        AddressablesDir = $@"{Addressables.RuntimePath}/StandaloneWindows64";
        // コンテンツの名前
        ContentsName = contentsName;
        // バッファのパス
        BufferDir = bufferDir;
        // カタログとハッシュの保存先
        DLContentsDir = contentsDir;
        // コンテントのディレクトリ
        ContentsDir = @$"{contentsDir}/{contentsName}";
        // カタログのファイル名（拡張子を含む）
        CatalogName = Directory.GetFiles(bufferDir, $"*.json")[0].Replace($"{bufferDir}\\", "");
        // ハッシュのファイル名（拡張子を含む）
        HashName = Directory.GetFiles(bufferDir, $"*.hash")[0].Replace($"{bufferDir}\\", "");
        // ハッシュの中身
        Hash = File.ReadAllText(@$"{BufferDir}/{HashName}");
    }


    public async UniTask Execute()
    {
        //// 既に同じハッシュのデータが存在したら取り込み中止
        //DLContentsHandler.Data.Load();
        //foreach (var contentsCatalog in DLContentsHandler.Data.ContentsCatalogs.Values)
        //{
        //    if (contentsCatalog.Hash == Hash)
        //    {
        //        Debug.LogAssertion($"既に同じカタログがあります");
        //        return;
        //    }
        //}

        DivideFolder(ContentsName);

        CatalogPath = @$"{DLContentsDir}/{ContentsName}/{CatalogName}";

        // リソースロケータ（取得したカタログがデシリアライズされたもの）を取得
        IResourceLocator resourceLocator = await GetLocator(@$"{CatalogPath}");
        // アセットバンドルグループのパス一を全取得
        GroupPaths = await ExtractGroupPaths(resourceLocator);
        //// 対象となるプレハブコンテンツのアドレスの一覧を取得
        //Addresses_Room = await ExtractContentAdresses(resourceLocator);

        Catalog catalog = new Catalog()
        {
            Hash = Hash,
            Path_Catalog = CatalogPath,
            Paths_Group = GroupPaths,
        };

        foreach (var label in labels)
        {
            List<string> addressList = new List<string>();
            List<string> list = new List<string>();
            if (label == "部屋" || label == "家具")
            {
                addressList = await AddressExtracter.Go(resourceLocator, label);
            }
            if (label == "アバター")
            {
                addressList = await AddressExtracter.Txt(resourceLocator, label);

                Addressables.AddResourceLocator(resourceLocator);
                for (int a = 0; a < addressList.Count; a++)
                {
                    string path = @$"{ContentsDir}\Avatar{a}.bytes";
                    TextAsset vRMAsBytes = await Addressables.LoadAssetAsync<TextAsset>(addressList[a]);
                    // TextAssetのバイトデータを取得
                    byte[] byteArray = vRMAsBytes.bytes;

                    // 指定されたパスに.bytesファイルとして保存
                    File.WriteAllBytes(path, byteArray);
                    addressList[a] = path;
                    //await LoadVRMFromBytes.Execute(path);
                    //File.Delete(path);
                }
                Addressables.RemoveResourceLocator(resourceLocator);
            }
            if (label == "空")
            {
                addressList = await AddressExtracter.Sky(resourceLocator, label);
            }

            Addresses addresses = new Addresses()
            {
                BeforeDL = addressList,
                AfterDL = new List<string>(),
            };
            catalog.Labels.Add(label, addresses);
        }


        DLContentsHandler.Data.ContentsCatalogs.Add(ContentsName, catalog);
        DLContentsHandler.Data.Save();
        DLContentsHandler.Data.Load();
    }


    void DivideFolder(string contentsName)
    {
        // コンテントディレクトリ作成
        Directory.CreateDirectory(ContentsDir);

        // Catalogをコンテントディレクトリに移す
        File.Copy(@$"{BufferDir}/{CatalogName}", @$"{ContentsDir}/{CatalogName}", true);
        File.Delete($@"{BufferDir}/{CatalogName}");

        File.Copy(@$"{BufferDir}/{HashName}", @$"{ContentsDir}/{HashName}", true);
        File.Delete($@"{BufferDir}/{HashName}");

        Debug.Log($"アドレッサブルフォルダ {AddressablesDir}");
        DirectoryUtil.CopyParentFolder(BufferDir, AddressablesDir);
        Directory.Delete(BufferDir, true);
    }



    // 使いたいカタログをリソースロケータに登録
    public async UniTask<IResourceLocator> GetLocator(string catalogPath)
    {
        // 新しいカタログを取得。ファイルパスかURL
        AsyncOperationHandle<IResourceLocator> requestCatalog
            = Addressables.LoadContentCatalogAsync(catalogPath);

        // ロード完了を待つ
        await requestCatalog;

        // 何のエラー処理だろ
        Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);

        // リソースロケータ（取得したカタログがデシリアライズされたもの）を返す
        return requestCatalog.Result;
    }


    //// 該当コンテンツのアドレス一覧を取得
    //public async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator, string label)
    //{
    //    Addressables.AddResourceLocator(resourceLocator);

    //    List<string> addresses = new List<string>();
    //    List<string> loadedAddresses = new List<string>();

    //    //Addressables.LoadAssetsAsync<GameObject>(label, null,)
    //    AsyncOperationHandle<IList<GameObject>> handle_LabelGobj = new AsyncOperationHandle<IList<GameObject>>();
    //    try
    //    {
    //        handle_LabelGobj = Addressables.LoadAssetsAsync<GameObject>(label, null);
    //        await handle_LabelGobj;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log($"このカタログにラベル {label} は無い");
    //        return new List<string>();
    //    }


    //    // カタログ(メモリ上ではIResourceLocatorとして扱われる)からアドレス一覧を抽出
    //    // IResourceLocator は key(通常はAdress)と、それに紐づくアセットの対応関係の情報を持っている
    //    foreach (var a in resourceLocator.Keys)
    //    {
    //        addresses.Add(a.ToString());
    //    }

    //    // ロードできるものを判別して、今回ロードしたいタイプのアセットか判別して、全部ロード
    //    foreach (var a in addresses)
    //    {
    //        Debug.Log($"------ アドレス : {a}");
    //        IList<IResourceLocation> resourceLocations;
    //        // IResourceLocation はアセットをロードするために必要な情報を持っている
    //        // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
    //        // つまりカタログが IResourceLocation を内包している
    //        if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


    //        foreach (var b in resourceLocations)
    //        {
    //            Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
    //            // IResourceLocation の PrimaryKey がアドレスらしい

    //            AsyncOperationHandle<GameObject> opHandle_LoadedGObj = new AsyncOperationHandle<GameObject>();
    //            try
    //            {
    //                opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
    //                await opHandle_LoadedGObj;
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.Log($"{b.PrimaryKey} はロードできなかった");
    //                continue;
    //            }

    //            // 今回のラベルに含まれているゲームオブジェクトかどうか
    //            bool containedInLabel = false;
    //            foreach (GameObject labelGobj in handle_LabelGobj.Result)
    //            {
    //                if (opHandle_LoadedGObj.Result == labelGobj)
    //                {
    //                    Debug.Log($"ラベルに含まれていた {opHandle_LoadedGObj.Result}");
    //                    containedInLabel = true;
    //                    continue;
    //                }
    //            }
    //            if (containedInLabel == false) continue;

    //            // ロードできなかったら次のループへ
    //            if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
    //            //Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

    //            // シーン内の物体としてのゲームオブジェクト(つまりRendererコンポーネントがついているはず)かどうかを判別して、
    //            // 違ったら次のループへ
    //            Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
    //            if (renderers.Length == 0) continue;
    //            //Debug.Log($"------ レンダラを持つゲームオブジェクト : {opHandle_LoadedGObj.Result}");

    //            // 既にロード済みのアドレスに含まれていたら次のループへ
    //            if (loadedAddresses.Contains(b.PrimaryKey)) continue;
    //            // 今回のアドレスを、ロード済みのアドレス一覧loadedAddressesに記録
    //            loadedAddresses.Add(b.PrimaryKey);
    //        }
    //    }

    //    Addressables.RemoveResourceLocator(resourceLocator);
    //    return loadedAddresses;
    //}



    public async UniTask<List<string>> ExtractGroupPaths(IResourceLocator resourceLocator)
    {
        List<string> groupPaths = new List<string>();
        Debug.Log($"===============================================");

        foreach (var a in resourceLocator.Keys)
        {
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation はアセットをロードするために必要な情報を持っている
            // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
            // つまりカタログが IResourceLocation を内包している

            // typeof()の中身は、アプリでDLアセットとして許す型
            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;
            Debug.Log($"アドレス {a}");

        }

        // カタログに含まれる全アセットバンドルグループのパスを切り出し
        foreach (var a in resourceLocator.Keys)
        {
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation はアセットをロードするために必要な情報を持っている
            // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
            // つまりカタログが IResourceLocation を内包している

            // typeof()の中身は、アプリでDLアセットとして許す型
            if (resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) break;
            if (resourceLocator.Locate(a, typeof(Animator), out resourceLocations)) break;
            if (resourceLocator.Locate(a, typeof(Skybox), out resourceLocations)) break;

            // グループのパスはKeysの序盤に集まっていて、その後に address達が来るっぽい
            // なので、Locate(Keys) が true になるまで（つまりaddressの前まで）の Keys を切り出せばグループのパスのリストを作れる
            groupPaths.Add($"{AddressablesDir}/{a.ToString()}");
            Debug.Log($"------- グループパス {AddressablesDir}/{a.ToString()}");
        }

        return groupPaths;
    }
}



public class AddressExtracter
{
    // 該当コンテンツのアドレス一覧を取得
    public static async UniTask<List<string>> Go(IResourceLocator resourceLocator, string label)
    {
        Addressables.AddResourceLocator(resourceLocator);

        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();

        AsyncOperationHandle<IList<GameObject>> handle_LabelGobj = new AsyncOperationHandle<IList<GameObject>>();
        try
        {
            handle_LabelGobj = Addressables.LoadAssetsAsync<GameObject>(label, null);
            await handle_LabelGobj;
        }
        catch (Exception e)
        {
            Debug.Log($"このカタログにラベル {label} は無い");
            return new List<string>();
        }


        // カタログ(メモリ上ではIResourceLocatorとして扱われる)からアドレス一覧を抽出
        // IResourceLocator は key(通常はAdress)と、それに紐づくアセットの対応関係の情報を持っている
        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        // ロードできるものを判別して、今回ロードしたいタイプのアセットか判別して、全部ロード
        foreach (var a in addresses)
        {
            Debug.Log($"------ アドレス : {a}");
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation はアセットをロードするために必要な情報を持っている
            // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
            // つまりカタログが IResourceLocation を内包している
            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
                // IResourceLocation の PrimaryKey がアドレスらしい

                AsyncOperationHandle<GameObject> opHandle_LoadedGObj = new AsyncOperationHandle<GameObject>();
                try
                {
                    opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
                    await opHandle_LoadedGObj;
                }
                catch (Exception e)
                {
                    //Debug.Log($"{b.PrimaryKey} はロードできなかった");
                    continue;
                }

                // 今回のラベルに含まれているゲームオブジェクトかどうか
                bool containedInLabel = false;
                foreach (GameObject labelGobj in handle_LabelGobj.Result)
                {
                    if (opHandle_LoadedGObj.Result == labelGobj)
                    {
                        Debug.Log($"ラベルに含まれていた {opHandle_LoadedGObj.Result}");
                        containedInLabel = true;
                        continue;
                    }
                }
                if (containedInLabel == false) continue;

                // ロードできなかったら次のループへ
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
                //Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

                // シーン内の物体としてのゲームオブジェクト(つまりRendererコンポーネントがついているはず)かどうかを判別して、
                // 違ったら次のループへ
                Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) continue;
                //Debug.Log($"------ レンダラを持つゲームオブジェクト : {opHandle_LoadedGObj.Result}");

                // 既にロード済みのアドレスに含まれていたら次のループへ
                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                // 今回のアドレスを、ロード済みのアドレス一覧loadedAddressesに記録
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        Addressables.RemoveResourceLocator(resourceLocator);
        return loadedAddresses;
    }

    // 該当コンテンツのアドレス一覧を取得
    public static async UniTask<List<string>> Txt(IResourceLocator resourceLocator, string label)
    {
        Addressables.AddResourceLocator(resourceLocator);

        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();

        AsyncOperationHandle<IList<TextAsset>> handle_LabelGobj = new AsyncOperationHandle<IList<TextAsset>>();
        try
        {
            handle_LabelGobj = Addressables.LoadAssetsAsync<TextAsset>(label, null);
            await handle_LabelGobj;
        }
        catch (Exception e)
        {
            Debug.Log($"このカタログにラベル {label} は無い");
            return new List<string>();
        }


        // カタログ(メモリ上ではIResourceLocatorとして扱われる)からアドレス一覧を抽出
        // IResourceLocator は key(通常はAdress)と、それに紐づくアセットの対応関係の情報を持っている
        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        // ロードできるものを判別して、今回ロードしたいタイプのアセットか判別して、全部ロード
        foreach (var a in addresses)
        {
            Debug.Log($"------ アドレス : {a}");
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation はアセットをロードするために必要な情報を持っている
            // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
            // つまりカタログが IResourceLocation を内包している
            if (!resourceLocator.Locate(a, typeof(TextAsset), out resourceLocations)) continue;


            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
                // IResourceLocation の PrimaryKey がアドレスらしい

                AsyncOperationHandle<TextAsset> opHandle_LoadedGObj = new AsyncOperationHandle<TextAsset>();
                try
                {
                    opHandle_LoadedGObj = Addressables.LoadAssetAsync<TextAsset>(b.PrimaryKey);
                    await opHandle_LoadedGObj;
                }
                catch (Exception e)
                {
                    Debug.Log($"{b.PrimaryKey} はロードできなかった");
                    continue;
                }

                // 今回のラベルに含まれているゲームオブジェクトかどうか
                bool containedInLabel = false;
                foreach (TextAsset labelGobj in handle_LabelGobj.Result)
                {
                    if (opHandle_LoadedGObj.Result == labelGobj)
                    {
                        Debug.Log($"ラベルに含まれていた {opHandle_LoadedGObj.Result}");
                        containedInLabel = true;
                        continue;
                    }
                }
                if (containedInLabel == false) continue;

                // ロードできなかったら次のループへ
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
                //Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

                // 既にロード済みのアドレスに含まれていたら次のループへ
                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                // 今回のアドレスを、ロード済みのアドレス一覧loadedAddressesに記録
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        Addressables.RemoveResourceLocator(resourceLocator);
        return loadedAddresses;
    }

    // 該当コンテンツのアドレス一覧を取得
    public static async UniTask<List<string>> Sky(IResourceLocator resourceLocator, string label)
    {
        Addressables.AddResourceLocator(resourceLocator);

        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();

        AsyncOperationHandle<IList<Material>> handle_LabelGobj = new AsyncOperationHandle<IList<Material>>();
        try
        {
            handle_LabelGobj = Addressables.LoadAssetsAsync<Material>(label, null);
            await handle_LabelGobj;
        }
        catch (Exception e)
        {
            Debug.Log($"このカタログにラベル {label} は無い");
            return new List<string>();
        }


        // カタログ(メモリ上ではIResourceLocatorとして扱われる)からアドレス一覧を抽出
        // IResourceLocator は key(通常はAdress)と、それに紐づくアセットの対応関係の情報を持っている
        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        // ロードできるものを判別して、今回ロードしたいタイプのアセットか判別して、全部ロード
        foreach (var a in addresses)
        {
            Debug.Log($"------ アドレス : {a}");
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation はアセットをロードするために必要な情報を持っている
            // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
            // つまりカタログが IResourceLocation を内包している
            if (!resourceLocator.Locate(a, typeof(Material), out resourceLocations)) continue;


            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
                // IResourceLocation の PrimaryKey がアドレスらしい

                AsyncOperationHandle<Material> opHandle_LoadedGObj = new AsyncOperationHandle<Material>();
                try
                {
                    opHandle_LoadedGObj = Addressables.LoadAssetAsync<Material>(b.PrimaryKey);
                    await opHandle_LoadedGObj;
                }
                catch (Exception e)
                {
                    Debug.Log($"{b.PrimaryKey} はロードできなかった");
                    continue;
                }

                // 今回のラベルに含まれているゲームオブジェクトかどうか
                bool containedInLabel = false;
                foreach (Material labelGobj in handle_LabelGobj.Result)
                {
                    if (opHandle_LoadedGObj.Result == labelGobj)
                    {
                        Debug.Log($"ラベルに含まれていた {opHandle_LoadedGObj.Result}");
                        containedInLabel = true;
                        continue;
                    }
                }
                if (containedInLabel == false) continue;

                // ロードできなかったら次のループへ
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
                //Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

                // 既にロード済みのアドレスに含まれていたら次のループへ
                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                // 今回のアドレスを、ロード済みのアドレス一覧loadedAddressesに記録
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        Addressables.RemoveResourceLocator(resourceLocator);
        return loadedAddresses;
    }
}