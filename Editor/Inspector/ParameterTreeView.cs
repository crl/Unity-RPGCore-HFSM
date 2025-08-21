using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HFSM
{
	public class ParameterTreeView : TreeView
	{
		private StateMachineExecutorController controller;
		private ParameterConditionData conditionData;

		public ParameterTreeView(TreeViewState state, StateMachineExecutorController controller, ParameterConditionData conditionData) : base(state)
		{
			this.controller = controller;
			this.conditionData = conditionData;

			showBorder = true;//边框
			showAlternatingRowBackgrounds = true;//交替显示
		}

		protected override TreeViewItem BuildRoot()
		{
			var root = new TreeViewItem(-1, -1);

			if (controller != null)
			{
				for (int i = 0; i < controller.parameters.Count; i++)
				{
					root.AddChild(new TreeViewItem(i, 0, controller.parameters[i].name));
				}
			}

			return root;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			base.RowGUI(args);
			if (args.label == conditionData.parameterName)
			{
				GUI.Label(args.rowRect, "√");
			}
		}

		protected override void SingleClickedItem(int id)
		{
			base.SingleClickedItem(id);
			string paramterName = FindItem(id, rootItem).displayName;

			var parameterData = controller.parameters.FirstOrDefault(x => x.name == paramterName);
			if (parameterData != null)
			{
				conditionData.parameterName = parameterData.name;
				switch (parameterData.type)
				{
					case ParameterType.Float:
					case ParameterType.Int:
						conditionData.compareType = CompareType.Greater;
						break;

					case ParameterType.Bool:
					case ParameterType.Trigger:
						conditionData.compareType = CompareType.Equal;
						break;
				}
				controller.Save();
			}
		}
	}
}