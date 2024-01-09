using UnityEditor;
using System.IO;

public class AssetBundleBuilder
{
    [MenuItem("XHWTest/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/XHWTEST/MaterialTest";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            BuildTarget.StandaloneWindows);

        AssetBundleBuild builder = new AssetBundleBuild();

    }
}