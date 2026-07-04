using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amber.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Amber.Runtime
{
    public static class AssetBundleService
    {
        private static readonly Dictionary<string, AssetBundleCreateRequest> _bundleLoadingOperations = new();
        private static readonly Dictionary<string, AssetBundleRequest> _loadingAssetOperations = new();

        public static async Task<T> GetAssetAsync<T>(string bundleName, string prefabName) where T : Object
        {
            AssetBundle bundle = await GetBundleAsync(bundleName);
            if (!bundle) return null;
            return await GetAssetAsync<T>(bundle, prefabName);
        }

        private static async Task<AssetBundle> GetBundleAsync(string bundleName)
        {
            if (_bundleLoadingOperations.TryGetValue(bundleName, out AssetBundleCreateRequest bundleOperation)) {
                return bundleOperation.assetBundle;
            }

            AssetBundleCreateRequest asyncOperation = AssetBundle.LoadFromFileAsync(Path.Combine(Paths.AssetBundles, bundleName));
            _bundleLoadingOperations.Add(bundleName, asyncOperation);
            await asyncOperation;

            if (asyncOperation.assetBundle) return asyncOperation.assetBundle;
            Debug.LogWarning($"AssetBundle '{bundleName}' not found.");
            return null;
        }

        private static async Task<T> GetAssetAsync<T>(AssetBundle bundle, string prefabName) where T : Object
        {
            if (_loadingAssetOperations.TryGetValue(prefabName, out AssetBundleRequest assetOperation)) {
                return assetOperation.asset as T;
            }

            AssetBundleRequest asyncOperation = bundle.LoadAssetAsync<T>(prefabName);
            _loadingAssetOperations.Add(prefabName, asyncOperation);
            await asyncOperation;

            if (asyncOperation.asset) return asyncOperation.asset as T;
            Debug.LogWarning($"Asset '{prefabName}' not found.");
            return null;
        }

        public static void UnloadAssetBundle(string bundleName, bool unloadAllLoadedAssets = true)
        {
            if (_bundleLoadingOperations.TryGetValue(bundleName, out AssetBundleCreateRequest bundleLoadingOperation)) {
                bundleLoadingOperation.assetBundle.Unload(unloadAllLoadedAssets);
                _bundleLoadingOperations.Remove(bundleName);
            }
        }

        public static void UnloadAllAssetBundles(bool unloadAllLoadedAssets = true)
        {
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedAssets);
            _bundleLoadingOperations.Clear();
            if (unloadAllLoadedAssets) {
                _loadingAssetOperations.Clear();
            }
        }
    }
}