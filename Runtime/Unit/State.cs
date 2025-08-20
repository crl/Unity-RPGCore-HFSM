using System;
using UnityEngine;

namespace HFSM
{
	public enum StateExecuteType
	{
		OnEnter,
		OnLogic,
		OnExit,
	}
	

	public class State : StateBase
	{
		public bool isTemporary => m_isTemporary;
		protected bool m_isTemporary;
		

		public Timer timer => m_timer;
		private Timer m_timer;

		public State()
		{
			m_stateType = StateType.State;
			m_timer = new Timer();
		}

		internal State(string id)
		{
			this.id = id;
		}

		public override void OnEnter()
		{
			m_timer.Reset();
		}

		/// <summary>
		/// 设置状态是否为临时状态
		/// </summary>
		public void SetIsTemporary(bool isTemporary) => m_isTemporary = isTemporary;
	}

	[Serializable]
	public class StateData : StateBaseData
	{
		public bool isTemporary = false;
		public bool canExitHandle = false;
		public string canExitDescription;
		public StateData()
		{
			stateType = StateType.State;
		}
	}
}