using UnityEngine;

namespace HFSM.EditorExtension
{
	public static class EventExtension
	{
		/// <summary>
		/// 鼠标按下
		/// </summary>
		public static bool IsMouseDown(int button = -1)
		{
			if (button == -1)
			{
				return Event.current.type == EventType.MouseDown;
			}
			else if (button == 0 || button == 1 || button == 2)
			{
				return Event.current.button == button && Event.current.type == EventType.MouseDown;
			}
			return false;
		}

		/// <summary>
		/// 鼠标抬起
		/// </summary>
		public static bool IsMouseUp(int button = -1)
		{
			if (button == -1)
			{
				return Event.current.type == EventType.MouseUp;
			}
			else if (button == 0 || button == 1 || button == 2)
			{
				return Event.current.button == button && Event.current.type == EventType.MouseUp;
			}
			return false;
		}
		
		/// <summary>
		/// 检测当前GUI元素是否被双击
		/// </summary>
		/// <param name="rect">GUI元素的区域（通常通过GUILayoutUtility.GetLastRect()获取）</param>
		/// <returns>是否发生双击</returns>
		public static bool IsDoubleClicked(Rect rect)
		{
			Event currentEvent = Event.current;
			return currentEvent.type == EventType.MouseDown 
			       && currentEvent.clickCount == 2 
			       && currentEvent.button == 0 // 0 = 左键，1 = 右键，2 = 中键
			       && rect.Contains(currentEvent.mousePosition);
		}
	}
}