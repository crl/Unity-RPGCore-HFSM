using HFSM.EditorExtension;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HFSM
{
    [CustomEditor(typeof(StateMachineExecutorController))]
    public class StateMachineControllerInspector : Editor
    {
        bool showHiddenData = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = serializedObject.FindProperty("controllerConfig");
            //================================
            GUILayout.Label("Controller Configs", "WhiteLargeLabel");
            GUILayout.Space(10);
            //================================

            var isCustom = config.FindPropertyRelative("isCustom");
            EditorGUILayout.PropertyField(isCustom);
            if (isCustom.boolValue)
            {
                EditorGUI.BeginDisabledGroup(true);
                var filePath = config.FindPropertyRelative("FilePath");
                EditorGUILayout.PropertyField(filePath);
                EditorGUI.EndDisabledGroup();

                if (Event.current.type == EventType.DragUpdated)
                {
                    if (GUILayoutUtility.GetLastRect().MouseOn())
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                        Event.current.Use();
                    }
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    if (GUILayoutUtility.GetLastRect().MouseOn())
                    {
                        string path = DragAndDrop.paths[0];
                        if (!path.Contains("."))
                        {
                            filePath.stringValue = path;
                        }

                        Event.current.Use();
                    }
                }
            }


            GUILayout.Space(5);


            showHiddenData = GUILayout.Toggle(showHiddenData, "Show Hidden Data");
            if (showHiddenData)
            {
                DrawDefaultInspector();
            }

            serializedObject.ApplyModifiedProperties();
        }


        [OnOpenAsset(0)]
        static bool OnOpenAsset(int instatnceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instatnceID) as StateMachineExecutorController;
            if (obj == null) return false;

            HFSMEditorWindow.ShowEditorWindow();
            return true;
        }
    }
}