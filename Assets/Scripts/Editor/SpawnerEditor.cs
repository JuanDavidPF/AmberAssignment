using UnityEditor;
using UnityEngine;
using Amber.Runtime;

namespace Amber.Editor
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : UnityEditor.Editor
    {
        private string _bundleName;

        private void OnEnable()
        {
            _bundleName = ((Spawner)target).LinkedAsset.AssetBundleName;
        }

        public override void OnInspectorGUI()
        {
            Spawner spawner = (Spawner)target;
            Object linkedAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(spawner.LinkedAsset.GUID));

            ShowLinkedAssetInformation(spawner, linkedAsset);

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            if (linkedAsset) {
                GUILayout.Space(10);
                if (GUILayout.Button("Apply changes to AssetBundle")) {
                    LinkAsset(spawner, linkedAsset, true);
                }

                GUILayout.Space(10);
                if (GUILayout.Button("Unlink")) {
                    spawner.UnlinkPrefab();
                }

                return;
            }

            _bundleName = EditorGUILayout.TextField("Asset Bundle", _bundleName);

            EditorGUI.BeginChangeCheck();

            Object newAssetToLink = (GameObject)EditorGUILayout.ObjectField("Drop Asset To Link", null, typeof(Object), false);

            if (EditorGUI.EndChangeCheck() && newAssetToLink) {
                LinkAsset(spawner, newAssetToLink);
            }
        }

        private void LinkAsset(Spawner spawner, Object prefab, bool force = false)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            string prefabGUID = AssetDatabase.AssetPathToGUID(path);

            if (force || spawner.LinkedAsset.GUID != prefabGUID) {
                spawner.AddAssetToBundle(path, new AssetMetadata { GUID = prefabGUID, Name = prefab.name, AssetBundleName = _bundleName });
            }
        }

        private void ShowLinkedAssetInformation(Spawner spawner, Object asset)
        {
            string path = AssetDatabase.GUIDToAssetPath(spawner.LinkedAsset.GUID);

            if (string.IsNullOrEmpty(spawner.LinkedAsset.GUID)) {
                EditorGUILayout.HelpBox("No asset linked", MessageType.Info);
                return;
            }

            if (!asset) {
                EditorGUILayout.HelpBox("The linked asset was not found in the project", MessageType.Warning);
                return;
            }

            EditorGUILayout.HelpBox($"Linked asset at {path}", MessageType.None);
        }
    }
}