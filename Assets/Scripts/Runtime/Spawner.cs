using System;
using System.IO;
using Amber.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Amber.Runtime
{
    [Serializable]
    public class AssetMetadata
    {
        public string GUID;
        public string Name;
        public string AssetBundleName = "main";
    }

    public class Spawner : MonoBehaviour
    {
        [field: SerializeReference] public AssetMetadata LinkedAsset { get; private set; } = new();

        private async void Start()
        {
            if (string.IsNullOrEmpty(LinkedAsset.AssetBundleName)) return;
            if (string.IsNullOrEmpty(LinkedAsset.Name)) return;

            Object asset = await AssetBundleService.GetAssetAsync<Object>(LinkedAsset.AssetBundleName, LinkedAsset.Name);
            if (!asset) return;
            Instantiate(asset, transform.position, transform.rotation);
        }

#if UNITY_EDITOR
        public void AddAssetToBundle(string assetPath, AssetMetadata assetMetadata)
        {
            if (Directory.Exists(Paths.AssetBundles)) {
                Directory.Delete(Paths.AssetBundles, true);
            }

            Directory.CreateDirectory(Paths.AssetBundles);

            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = assetMetadata.AssetBundleName;
            importer.assetBundleVariant = string.Empty;
            importer.SaveAndReimport();

            BuildPipeline.BuildAssetBundles(
                Paths.AssetBundles,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
            LinkedAsset = assetMetadata;
        }

        public void UnlinkPrefab()
        {
            LinkedAsset = new AssetMetadata();
            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
        }
#endif
    }
}