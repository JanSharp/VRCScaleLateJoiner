using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UdonSharpEditor;

namespace JanSharp
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemScaleManager))]
    public class ItemScaleManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targets))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Find all Items", $"Populates {nameof(ItemScaleManager.items)} and"
                + $" {nameof(ItemScaleManager.initialScales)} with all objects in all the Pools of all the"
                + $" {nameof(VRCObjectPool)}s it finds in all {nameof(ItemScaleManager.parentsForObjectPools)}"
                + $" objects and their children.")))
            {
                foreach (var manager in targets.Cast<ItemScaleManager>())
                {
                    List<GameObject> itemsList = new List<GameObject>();
                    foreach (GameObject obj in manager.parentsForObjectPools)
                    {
                        foreach (VRCObjectPool op in obj.GetComponentsInChildren<VRCObjectPool>())
                        {
                            itemsList.AddRange(op.Pool);
                        }
                    }
                    SerializedObject managerProxy = new SerializedObject(manager);
                    EditorUtil.SetArrayProperty(
                        managerProxy.FindProperty(nameof(ItemScaleManager.items)),
                        itemsList.ToArray(),
                        (p, v) => p.objectReferenceValue = v
                    );
                    EditorUtil.SetArrayProperty(
                        managerProxy.FindProperty(nameof(ItemScaleManager.initialScales)),
                        itemsList.Select(go => go.transform.localScale).ToArray(),
                        (p, v) => p.vector3Value = v
                    );
                    managerProxy.ApplyModifiedProperties();
                }
            }
        }
    }
}
