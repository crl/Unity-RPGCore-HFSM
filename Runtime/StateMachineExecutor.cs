using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HFSM
{
	public class StateMachineExecutor : MonoBehaviour
	{
		/// <summary>
		/// 当前执行的Controller
		/// </summary>
		public StateMachineExecutorController executorController;

		private StateMachineScriptController _scriptController;
		public StateMachineScriptController scriptController => _scriptController;

		/// <summary>
		/// 当前执行的Controller的根状态机
		/// </summary>
		public StateMachine rootStateMachine
		{
			get;
			private set;
		}


		//运行时状态机执行栈
		protected internal Stack<StateBundle> executeStateStack
		{
			get;
		} = new Stack<StateBundle>();

		//记录当前执行的状态
		protected internal State currentExecuteState
		{
			get;
			private set;
		}

		//记录State执行历史 最大记录8个
		private RingStack<StateBundle> _executeStateHistory = new(8);

		private void Awake()
		{
			rootStateMachine = executorController.GetExecuteStateMachine(this, out _scriptController);
			//if (m_rootStateMachine != null)
			//{
			//	ShowStateMachineTree(0, m_rootStateMachine);
			//}
		}

		private void Start()
		{
			InitStateMachineExecute();
			UpdateStackMachine();
		}

		private void FixedUpdate()
		{
			ExecuteStateMachineService(ServiceType.FixedUpdate);
		}

		private void Update()
		{
			ExecuteStateMachineService(ServiceType.Update);
			ExecuteStateMachineService(ServiceType.CustomInterval);
			currentExecuteState.OnUpdate();
		}

		private void LateUpdate()
		{
			UpdateStackMachine();
		}

		/// <summary>
		/// 状态机执行初始化
		/// </summary>
		public void InitStateMachineExecute()
		{
			executeStateStack.Clear();
			currentExecuteState = null;
			if (rootStateMachine != null)
			{
				FillExecuteStateStack(rootStateMachine);
			}
		}

		/// <summary>
		/// 执行状态机服务
		/// </summary>
		public void ExecuteStateMachineService(ServiceType type)
		{
			if (executeStateStack.Count == 0) return;
			foreach (var bundle in executeStateStack.Reverse())
			{
				if (bundle.services != null)
				{
					foreach (Service service in bundle.services.Where(s => s.serviceType == type))
					{
						if (type != ServiceType.CustomInterval) service.OnUpdate();
						else
						{
							if (service.timer.Elapsed >= service.customInterval)
							{
								service.OnUpdate();
								service.timer.Reset();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 尝试执行转换 由执行栈底到栈顶 先Global再Normal
		/// </summary>
		public Transition TryTransition()
		{
			var stateBundles = executeStateStack.ToArray();
			//当前通过的转换
			Transition passedTransition = null;
			for (int i = stateBundles.Length - 1; i >= 0; i--)
			{
				var bundle = stateBundles[i];
				//尝试以当前状态为起点的Transition
				if (bundle.transitions is not null)
				{
					foreach (Transition transition in bundle.transitions)
					{
						if (transition.ShouldTransition() && (executeStateStack.Peek().state as State).isCanExit())
						{
							passedTransition = transition;
							break;
						}
					}
				}
				//尝试当前状态中的GlobalTransition
				var gTrans = bundle.GetGlobalTransitions();
				if (gTrans is not null)
				{
					foreach (var transition in gTrans)
					{
						if (transition.ShouldTransition() && executeStateStack.Peek().state.isCanExit())
						{
							passedTransition = transition;
							break;
						}
					}
				}
				//尝试临时状态是否执行转换
				if (bundle.state.stateType == StateType.State && (bundle.state is State { isTemporary: true } state))
				{
					if (state.isCanExit())
					{
						passedTransition = new Transition(null, state, _executeStateHistory.Peek().state.parent);
						break;
					}
				}
			}
			return passedTransition;
		}

		/// <summary>
		/// 更新状态机执行
		/// </summary>
		public void UpdateStackMachine()
		{
			Transition passedTransition = TryTransition();
			if (passedTransition is not null)
			{
				StateBase transState = passedTransition.transitionType ==
					TransitionType.Global ? passedTransition.parentStateMachine : passedTransition.from;
				StateBase toState = passedTransition.to;
				//当前转换的状态是不是一个临时状态
				if (transState.stateType == StateType.State && (transState as State).isTemporary)
				{
					executeStateStack.Clear();
					if (toState.parent != null)
					{
						foreach (var state in toState.parent.executeStackSnapshot.Reverse())
						{
							executeStateStack.Push(state);
						}
					}
					transState.OnExit();
				}
				else
				{
					//先把转换前的状态出栈
					while (true)
					{
						StateBundle popState = executeStateStack.Pop();
						if (popState.state.stateType != StateType.StateMachine) popState.state.OnExit();
						else popState.services.ForEach(s => s.OnExit());
						if (popState.state.id == transState.id)
						{
							if (passedTransition.transitionType == TransitionType.Global) executeStateStack.Push(popState);
							break;
						}
					}
				}
				//Debug.Log("toState : " + toState.id);
				//再将转换后的状态入栈
				FillExecuteStateStack(toState);
				//重置当前执行成功的Transition中的所使用的Trigger
				passedTransition.ResetTriggers();
			}
		}

		/// <summary>
		/// 根据传入的State填充执行状态栈
		/// </summary>
		private void FillExecuteStateStack(StateBase state)
		{
			while (state.stateType == StateType.StateMachine)
			{
				executeStateStack.Push(new StateBundle(state));
				if (state.executeStackSnapshot == null)
				{
					state.SetExecuteStackSnapshot(executeStateStack.ToArray());
				}

				var sm = state as StateMachine;
				sm.services.ForEach(service => { service.OnEnter(); });
				state = sm.defaultState;
				if (state is null)
				{
					Debug.LogError($"Can not find the default state/state machine in [{state.id}] state machine.");
					return;
				}
			}
			state.OnEnter();
			executeStateStack.Push(new StateBundle(state));
			if (state.executeStackSnapshot == null)
			{
				state.SetExecuteStackSnapshot(executeStateStack.ToArray());
			}
			currentExecuteState = state as State;
			//仅当当前状态不是临时状态才记录进执行历史中
			if (!(state as State).isTemporary) _executeStateHistory.Push(executeStateStack.Peek());
			//Debug.Log("current:"+m_executeStateHistory.Peek().state.id+" count:"+m_executeStateHistory.Length);
		}

		//测试用，打印状态机的树状结构
		private void ShowStateMachineTree(int level, StateMachine stateMachine)
		{
			string sj = "";
			for (int i = 0; i < level; i++) { sj += "\t"; }
			Debug.Log(sj + "【" + stateMachine.id + "】" + $"t:{stateMachine.transitions.Count} "
				+ $"global t:{stateMachine.transitions.FindAll(t => t.transitionType == TransitionType.Global).Count}");
			foreach (var key in stateMachine.states.Keys)
			{
				var state = stateMachine.states[key];
				if (state.stateType == StateType.StateMachine)
				{
					ShowStateMachineTree(level + 1, state as StateMachine);
				}
				else
				{
					if (state.id != "Any")
					{
						Debug.Log(sj + "\t" + state.id);
					}
				}
			}
		}
	}

	public struct StateBundle
	{
		public StateBase state;
		public List<Transition> transitions;
		public List<Service> services;

		public StateBundle(StateBase s)
		{
			state = s;
			transitions = s.GetParentTransitionsStartWith();
			services = s.stateType == StateType.StateMachine ? (s as StateMachine).services : null;
		}

		/// <summary>
		/// 获取到当前StateMachine（如果是的话）下的GlobalTransition
		/// </summary>
		public List<Transition> GetGlobalTransitions()
		{
			if (state.stateType == StateType.StateMachine)
			{
				return (state as StateMachine).transitions.FindAll(t => t.transitionType == TransitionType.Global);
			}
			return null;
		}
	}
}