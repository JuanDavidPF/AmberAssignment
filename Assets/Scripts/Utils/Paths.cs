using System.IO;
using UnityEngine;

namespace Amber.Utils
{
    public static class Paths
    {
        public static string AssetBundles => Path.Combine(Application.streamingAssetsPath, "AssetBundles");
    }
}