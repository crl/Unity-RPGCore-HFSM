using HFSM.EditorExtension;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

namespace HFSM
{
	[CustomEditor(typeof(StateMachineInspectorHelper))]
	public class StateMachineInspector : Editor
	{
		public string stateName;
		private ReorderableList reorderableList;
		private Rect left_container;
		private Rect right_container;
		private bool isRenaming;
		private string tempname;

		private void OnEnable()
		{
			StateMachineInspectorHelper helper = target as StateMachineInspectorHelper;
			if (helper == null) return;
			reorderableList = new ReorderableList(helper.stateMachineData.services, typeof(string), true, false, true, true);
			reorderableList.onAddCallback += AddService;
			reorderableList.onRemoveCallback += RemoveService;
			reorderableList.drawElementCallback += DrawElement;
		}

		public override void OnInspectorGUI()
		{
			StateMachineInspectorHelper helper = target as StateMachineInspectorHelper;
			if (helper == null) return;
			bool disable = EditorApplication.isPlaying;
			EditorGUI.BeginDisabledGroup(disable);
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("state name", GUILayout.Width(80));
			string newName = helper.stateMachineData.id;
			disable = newName == "Root";
			EditorGUI.BeginDisabledGroup(disable);
			EditorGUI.BeginChangeCheck();
			newName = EditorGUILayout.DelayedTextField(newName);
			if (EditorGUI.EndChangeCheck() && newName != stateName)
			{
				helper.controller.RenameState(helper.stateMachineData, newName);
				stateName = newName;
				EditorUtility.SetDirty(helper.controller);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("services", GUILayout.Width(80));
			if (reorderableList.list != helper.stateMachineData.services)
				reorderableList.list = helper.stateMachineData.services;
			reorderableList.DoLayoutList();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("description", GUILayout.Width(80));
			string description = helper.stateMachineData.description;
			EditorGUI.BeginChangeCheck();
			description = EditorGUILayout.DelayedTextField(description);
			if (EditorGUI.EndChangeCheck())
			{
				helper.stateMachineData.description = description;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
			EditorGUI.EndDisabledGroup();
		}

		protected override void OnHeaderGUI()
		{
			StateMachineInspectorHelper helper = target as StateMachineInspectorHelper;
			if (helper == null) return;
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label(EditorGUIUtility.IconContent("icons/processed/unityeditor/animations/animatorstate icon.asset"), GUILayout.Width(30), GUILayout.Height(30));
			EditorGUILayout.LabelField("Name", style: "HeaderLabel", GUILayout.Width(50));

			EditorGUILayout.LabelField(helper.stateMachineData.id);

			EditorGUILayout.EndHorizontal();

			var rect = EditorGUILayout.BeginHorizontal();

			EditorGUILayout.Space();
			Handles.color = Color.black;
			Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
			EditorGUILayout.Space();

			EditorGUILayout.EndHorizontal();
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var helper = target as StateMachineInspectorHelper;
			if (helper == null) return;
			left_container = rect.SplitVertical(1, 2)[0];
			right_container = rect.SplitVertical(1, 2)[1];
			var service = helper.stateMachineData.services[index];
			if (isFocused && EventExtension.IsMouseDown(0))
			{
				isRenaming = true;
			}
			if (isRenaming && reorderableList.index == index)
			{
				EditorGUI.BeginChangeCheck();
				tempname = EditorGUI.DelayedTextField(left_container, service.id);
				if (EditorGUI.EndChangeCheck())
				{
					helper.controller.RenameService(service, tempname);
					isRenaming = false;
				}
			}
			else
			{
				EditorGUI.LabelField(left_container, service.id);
			}
			if (isRenaming && EventExtension.IsMouseDown(0))
			{
				isRenaming = false;
			}
			if (service.serviceType != ServiceType.CustomInterval)
			{
				//GUI.Box(right_container.Resize(2), "");
				EditorGUI.LabelField(right_container, service.serviceType.ToString());
			}
			else
			{
				Rect[] rects = right_container.SplitVertical(3, 1);
				//GUI.Box(rects[0].Resize(2), "");
				EditorGUI.LabelField(rects[0], service.serviceType.ToString());
				service.customInterval = EditorGUI.FloatField(rects[1], service.customInterval);
			}
			
			var e = Event.current;
			if (e.type == EventType.ContextClick && rect.Contains(e.mousePosition))
			{
				// 阻止默认事件并显示右键菜单
				e.Use();
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Edit Script"), false, () =>
				{
					//TODO:打开文件并跳转到对应行
					helper.controller.JumpToScript(service.id,"Service");
				});
				menu.ShowAsContext();
			}
		}

		private void RemoveService(ReorderableList list)
		{
			StateMachineInspectorHelper helper = target as StateMachineInspectorHelper;
			if (helper == null || list.index < 0) return;
			ServiceData service = helper.stateMachineData.services[list.index];
			helper.controller.DeleteService(helper.stateMachineData, service);
		}

		private void AddService(ReorderableList list)
		{
			var helper = target as StateMachineInspectorHelper;
			if (helper == null) return;
			GenericMenu genericMenu = new GenericMenu();

			for (int i = 0; i < Enum.GetNames(typeof(ServiceType)).Length; i++)
			{
				ServiceType serviceType = (ServiceType)Enum.GetValues(typeof(ServiceType)).GetValue(i);
				genericMenu.AddItem(new GUIContent(Enum.GetNames(typeof(ServiceType))[i]), false, () =>
				{
					helper.controller.CreateService(helper.stateMachineData, serviceType);
				});
			}
			genericMenu.ShowAsContext();
		}
	}

	public class StateMachineInspectorHelper : ScriptableObjectSingleton<StateMachineInspectorHelper>
	{
		public StateMachineExecutorController controller;
		public StateMachineData stateMachineData;

		public void Inspector(StateMachineExecutorController HFSMController, StateMachineData stateMachineData)
		{
			this.controller = HFSMController;
			this.stateMachineData = stateMachineData;
			Undo.IncrementCurrentGroup();
			Selection.activeObject = this;
		}
	}
}