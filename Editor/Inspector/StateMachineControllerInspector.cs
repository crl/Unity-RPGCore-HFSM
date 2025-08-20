using DogFramework.EditorExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPGCore.AI.HFSM
{
	[CustomEditor(typeof(StateMachineExecutorController))]
	public class StateMachineControllerInspector : Editor
	{
		bool foldout = false;
		bool showHiddenData = false;
		public override void OnInspectorGUI()
		{
			StateMachineExecutorController controller = (StateMachineExecutorController)target;
			StateMachineControllerConfig config = controller.controllerConfig;
			//================================
			GUILayout.BeginVertical();
			GUILayout.Label("Controller Configs", "WhiteLargeLabel");
			GUILayout.Space(10);
			//================================

			GUILayout.BeginHorizontal();
			GUILayout.Label(new GUIContent("Custom Path", "Customize the path of file generation"),GUILayout.Width(128));
			config.CustomFilePath=GUILayout.Toggle(config.CustomFilePath, "",GUILayout.Width(20));
			if (config.CustomFilePath)
			{
				EditorGUI.BeginDisabledGroup(true);
				config.FilePath = GUILayout.TextField(config.FilePath);
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
							config.FilePath = path;
						}

						Event.current.Use();
					}
				}
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label(new GUIContent("Show Hidden Data"), GUILayout.Width(128));
			showHiddenData = GUILayout.Toggle(showHiddenData, "", GUILayout.Width(20));
			GUILayout.EndHorizontal();
			if (showHiddenData) 
			{
				DrawDefaultInspector();
			}
			//================================
			GUILayout.EndVertical();
		}


		[OnOpenAsset(0)]
		static bool OnOpenAsset(int instatnceID, int line)
		{
			var obj=EditorUtility.InstanceIDToObject(instatnceID) as StateMachineExecutorController;
			if (obj == null) return false;

			HFSMEditorWindow.ShowEditorWindow();
			return true;
		}
	}
}