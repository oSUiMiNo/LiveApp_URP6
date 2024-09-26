using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create AssetBundles")]
    private static void CreateAllAssetBundles()
    {
        string assetBundleDirectory = $"AssetBundles/{EditorUserBuildSettings.activeBuildTarget}";

        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            EditorUserBuildSettings.activeBuildTarget);
    }
#endif
}
