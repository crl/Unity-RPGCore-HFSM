using System;

namespace HFSM
{
	public class StateMachineHandlerResult
	{
		/// <summary>
		/// 上一次操作的结果；
		/// </summary>
		public StateMachineHandlerResult previousHandleResult => m_previousHandleResult;

		protected StateMachineHandlerResult m_previousHandleResult;

		/// <summary>
		/// 当前操作的State或StateMachine的父StateMachine
		/// </summary>
		public StateMachine parentStateMachine => m_parentStateMachine;

		protected StateMachine m_parentStateMachine;

		/// <summary>
		/// 当前操作过的State或StateMachine
		/// </summary>
		public StateBase handledState => m_handledState;

		protected StateBase m_handledState;

		/// <summary>
		/// 由上一次操作创建的一条Transition
		/// </summary>
		public Transition createdTransition => m_createdTransition;

		protected Transition m_createdTransition;

		/// <summary>
		/// 由上一次操作创建的一个Service
		/// </summary>
		public Service createdService => m_createdService;

		private Service m_createdService;

		/// <summary>
		/// 返回当前是否开启了新的一层操作；创建StateMachine后第一次操作此字段为真
		/// </summary>
		private bool IsBeginNewLevel => parentStateMachine.id.Equals(handledState.id);

		public StateMachineHandlerResult(StateMachineHandlerResult previousHandleResult,
			StateMachine parentStateMachine,
			StateBase handledState,
			Transition createdTransition = null,
			Service createdService = null)
		{
			m_previousHandleResult = previousHandleResult;
			m_parentStateMachine = parentStateMachine;
			m_handledState = handledState;
			m_createdTransition = createdTransition;
			m_createdService = createdService;
		}

		/// <summary>
		/// 为当前操作的StateMachine添加一个state
		/// </summary>
		public StateMachineHandlerResult AddState<T>(string stateID,bool isDefault = false) where T:State,new()
		{
			//Debug.Log($"add [{stateId}] state to [{parentStateMachine.id}].");
			var state = parentStateMachine.AddState<T>(stateID,isDefault);
			return new StateMachineHandlerResult(IsBeginNewLevel ? this : previousHandleResult, parentStateMachine, state);
		}

		public StateMachineHandlerResult AddTemporaryState<T>(string stateID) where T:State,new()
		{
			//Debug.Log($"add [{stateId}] state to [{parentStateMachine.id}].");
			var state = parentStateMachine.AddState<T>(stateID);
			state.SetIsTemporary(true);
			return new StateMachineHandlerResult(IsBeginNewLevel ? this : previousHandleResult, parentStateMachine, state);
		}

		/// <summary>
		/// 为当前操作的StateMachine添加一个StateMachine
		/// </summary>
		public StateMachineHandlerResult AddStateMachine(string stateMachineId, bool isDefault = false)
		{
			//Debug.Log($"add [{stateMachineId}] state machine to [{parentStateMachine.id}].");
			var stateMachine = parentStateMachine.GetStateMachine(stateMachineId, isDefault);
			return new StateMachineHandlerResult(IsBeginNewLevel ? this : previousHandleResult, stateMachine, stateMachine);
		}

		/// <summary>
		/// 为当前操作的state或StateMachine添加一条到目标state的Transition
		/// 如果目标state不存在，则创建一个state
		/// </summary>
		public StateMachineHandlerResult ToState(string stateID,bool isTemporary = false) 
		{
			if (IsBeginNewLevel)
			{
				throw new Exception($"have not any handled state in {parentStateMachine.id} state machine." +
					$"you should first add a state or state machine and then call 'ToState'.");
			}
			else
			{
				var state = parentStateMachine.GetState(stateID);
				state.SetIsTemporary(isTemporary);
				//Debug.Log($"add transition from [{handledState.id}] to [{state.id}]");
				return new StateMachineHandlerResult(previousHandleResult, parentStateMachine, state,
					parentStateMachine.AddTransition(handledState, state));
			}
		}

		/// <summary>
		/// 为当前操作的state或StateMachine添加一条到目标StateMachine的Transition
		/// 如果目标StateMachine不存在，则创建一个如果目标StateMachine不存在
		/// </summary>
		public StateMachineHandlerResult ToStateMachine(string stateMachineId)
		{
			if (IsBeginNewLevel)
			{
				throw new Exception($"have any handled state in {parentStateMachine.id} state machine." +
					$"you should first add a state or state machine and then call 'ToStateMachine'.");
			}
			else
			{
				bool smIsExist = parentStateMachine.Contains(stateMachineId);
				var stateMachine = parentStateMachine.GetStateMachine(stateMachineId);
				m_createdTransition = parentStateMachine.AddTransition(handledState, stateMachine);
				//Debug.Log($"add transition from [{handledState.id}] to [{stateMachine.id}]");
				return new StateMachineHandlerResult(previousHandleResult, smIsExist ? parentStateMachine : stateMachine, smIsExist ? handledState : stateMachine,
					createdTransition);
			}
		}

		/// <summary>
		/// 为当前操作后创建的Transition添加 baseCondition
		/// </summary>
		public StateMachineHandlerResult Condition(Func<Transition, bool> condition)
		{
			if (m_createdTransition != null)
			{
				m_createdTransition.AddBaseCondition(condition);
				//Debug.Log($"add condition from [{createdTransition.from.id}] to [{createdTransition.to.id}]");
				return new StateMachineHandlerResult(previousHandleResult, parentStateMachine, handledState, createdTransition);
			}
			throw new Exception($"no transition have been created to add conditions." +
				$"you should call this after 'ToState' or 'ToStateMachine'.");
		}

		/// <summary>
		/// 为当前操作后创建的Transition添加 paramterCondition
		/// </summary>
		public StateMachineHandlerResult Condition(string paramterName, ParameterType parameterType, CompareType compareType, object value)
		{
			if (m_createdTransition != null)
			{
				Parameter parameter = StateMachineHandler.currentHandledController.parameters[paramterName];
				if (parameter == null)
				{
					throw new Exception($"paramter named {paramterName} is not exist.");
				}
				m_createdTransition.AddParamterCondition(parameter, compareType, value);
				//Debug.Log($"add condition from [{createdTransition.from.id}] to [{createdTransition.to.id}]");
				return new StateMachineHandlerResult(previousHandleResult, parentStateMachine, handledState, createdTransition);
			}
			throw new Exception($"no transition have been created to add conditions." +
				$"you should call this after 'ToState' or 'ToStateMachine'.");
		}

		public StateMachineHandlerResult BoolCondition(string paramterName, bool value)
		{
			return Condition(paramterName, ParameterType.Bool, CompareType.Equal, value);
		}

		public StateMachineHandlerResult IntCondition(string paramterName, CompareType compareType, int value)
		{
			return Condition(paramterName, ParameterType.Int, compareType, value);
		}

		public StateMachineHandlerResult FloatCondition(string paramterName, CompareType compareType, float value)
		{
			return Condition(paramterName, ParameterType.Float, compareType, value);
		}

		public StateMachineHandlerResult TriggerCondition(string paramterName)
		{
			return Condition(paramterName, ParameterType.Trigger, CompareType.Equal, true);
		}

		/// <summary>
		/// 创建一条与当前操作后创建的Transition方向相反的Transition
		/// </summary>
		public StateMachineHandlerResult Reverse(bool reverseCondition = false)
		{
			if (m_createdTransition != null)
			{
				//Debug.Log($"reverse transition from [{createdTransition.from.id}] to [{createdTransition.to.id}]");
				m_createdTransition = m_createdTransition.Reverse(reverseCondition);
				return new StateMachineHandlerResult(previousHandleResult, parentStateMachine, handledState, createdTransition);
			}
			throw new Exception($"no transition have been created to reverse." +
				$"you should call this after 'ToState','ToStateMachine' or 'Condition'.");
		}

		public StateMachineHandlerResult SwitchHandle<T>() where T:State
		{
			var stateId = typeof(T).Name;
			return SwitchHandle(stateId);
		}

		/// <summary>
		/// 改变当前操作的State或StateMachine
		/// </summary>
		public StateMachineHandlerResult SwitchHandle(string stateId)
		{
			if (parentStateMachine.states.TryGetValue(stateId,out var state))
			{
				return new StateMachineHandlerResult(previousHandleResult, parentStateMachine, state);
			}
			throw new Exception($"[{stateId}] in state machine [{parentStateMachine.id}] is not exist.");
		}

		/// <summary>
		/// 为StateMachine添加Service
		/// </summary>
		public StateMachineHandlerResult AddService<T>(ServiceType serviceType = ServiceType.Update, float customInterval = 0f) where T:Service,new()
		{
			if (handledState.stateType == StateType.StateMachine)
			{
				var service = (handledState as StateMachine).AddService<T>(type: serviceType,
					customInterval: customInterval);
				//Debug.Log($"add service [{serviceId}] to [{handledState.id}] state machine.");
				return new StateMachineHandlerResult(previousHandleResult, parentStateMachine,
					handledState, createdTransition,service);
			}
			throw new Exception("only state machine can add Service");
		}

		/// <summary>
		/// 结束当前操作的StateMachine，即返回上一层StateMachine继续操作
		/// </summary>
		public StateMachineHandlerResult FinishHandle()
		{
			//Debug.Log($"finish handle [{previousHandleResult.handledState.id}] state machine");
			if (previousHandleResult == null || previousHandleResult.previousHandleResult == null) return this;
			return new StateMachineHandlerResult(previousHandleResult.previousHandleResult,
				previousHandleResult.previousHandleResult.parentStateMachine,
				previousHandleResult.handledState,
				previousHandleResult.createdTransition);
		}

		/// <summary>
		/// 结束操作
		/// </summary>
		public void EndHandle()
		{
			//Debug.Log($"end handle [{parentStateMachine.id}] state machine");
		}

		public StateMachineHandlerResult OnExecute(Action<State, StateExecuteType> onIdleExecute)
		{
			return new StateMachineHandlerResult(IsBeginNewLevel ? this : previousHandleResult, parentStateMachine, null);
		}
	}
}