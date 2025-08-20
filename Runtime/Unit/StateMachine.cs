using System;
using System.Collections.Generic;
using UnityEngine;

namespace HFSM
{
    public enum ServiceType
    {
        Update,
        FixedUpdate,
        CustomInterval
    }

    public class StateMachine : StateBase
    {
        public static string anyState => "Any";
        public static string entryState => "Entry";

        public StateMachineScriptController ctrl { get; internal set; }

        public Dictionary<string, StateBase> states => m_states;
        private Dictionary<string, StateBase> m_states;
        public bool isRoot => m_isRoot;
        private bool m_isRoot;
        public List<Transition> transitions => m_transitions;
        private List<Transition> m_transitions;
        public StateBase activeState => m_activeState;
        private StateBase m_activeState;
        public StateBase defaultState => m_defaultState;
        private StateBase m_defaultState = null;
        public List<Service> services => m_services;
        private List<Service> m_services;

        public State Any { get; } = new State(anyState);
        public State Entry { get; } = new State(entryState);

        public StateMachine(string id)
        {
            m_id = id;
            m_stateType = StateType.StateMachine;
            m_states = new();
            m_transitions = new List<Transition>();
            m_services = new List<Service>();
            m_defaultState = null;
            m_states.Add(Any.id, Any);
            m_states.Add(Entry.id, Entry);
        }

        private State AddState(State state, bool defaultState = false)
        {
            m_states.Add(state.id, state);
            state.SetParentStateMachine(this);
            if (defaultState) m_defaultState = state;
            return state;
        }

        public State GetState(string stateID)
        {
            if (m_states.TryGetValue(stateID, out var state) == false)
            {
                Debug.LogWarning($"state {stateID} in {this.id} has not exist.");
            }

            return state as State;
        }

        public T AddState<T>(string stateID,bool defaultState = false) where T : State, new()
        {

            if (m_states.TryGetValue(stateID, out var stateBase) == false)
            {
                var state = new T();
                state.id = stateID;
                AddState(state, defaultState);
                return state;
            }

            Debug.LogWarning($"state {stateID} in {this.id} has already exist.");
            return stateBase as T;
        }

        public StateMachine AddStateMachine(StateMachine stateMachine, bool defaultState = false)
        {
            m_states.Add(stateMachine.id, stateMachine);
            stateMachine.SetParentStateMachine(this);
            if (defaultState) m_defaultState = stateMachine;
            return stateMachine;
        }

        public StateMachine GetStateMachine(string stateMachineId, bool defaultState = false)
        {
            if (m_states.TryGetValue(stateMachineId, out var state) == false || state is StateMachine == false)
            {
                var stateMachine = new StateMachine(stateMachineId);
                stateMachine.ctrl = ctrl;
                return AddStateMachine(stateMachine, defaultState);
            }
            return (StateMachine)state;
        }

        public Transition AddTransition(string fromId, string toId,
            Func<Transition, bool> condition = null)
        {
            m_states.TryGetValue(fromId, out var from);
            m_states.TryGetValue(fromId, out var to);
            return AddTransition(from, to, condition);
        }

        public Transition AddTransition(StateBase from,
            StateBase to,
            Func<Transition, bool> condition = null)
        {
            Transition transition = null;
            transition = new Transition(this, from, to, condition,
                from.id.Equals(Any.id) ? TransitionType.Global : TransitionType.Normal);
            m_transitions.Add(transition);
            return transition;
        }

        public Service AddService<T>(ServiceType type = ServiceType.Update, float customInterval = 0f)
            where T : Service, new()
        {
            var serviceId = typeof(T).Name;
            Service _service = m_services.Find(s => s.id.Equals(serviceId));
            if (_service == null)
            {
                _service = new T();
                _service.Init(serviceId, type, customInterval);
                m_services.Add(_service);
            }

            return _service;
        }

        public bool Contains(string stateId)
        {
            return m_states.ContainsKey(stateId);
        }

        public void SetRoot() => m_isRoot = true;
    }

    [Serializable]
    public class StateMachineData : StateBaseData
    {
        public bool isRoot;
        public List<string> childStates = new();
        public string defaultState;
        public List<string> transitions = new();
        public List<ServiceData> services = new();

        [HideInInspector] public StateData any;

        [HideInInspector] public StateData entry;

        public StateMachineData()
        {
#if UNITY_EDITOR
			any = new StateData()
			{
				id = StateMachine.anyState,
				stateType = StateType.State,
				position = new Rect(0, 100, StateBase.stateWidth, StateBase.stateHeight)
			};
			entry = new StateData()
			{
				id = StateMachine.entryState,
				stateType = StateType.State,
				position = new Rect(0, 400, StateBase.stateWidth, StateBase.stateHeight)
			};
			stateType = StateType.StateMachine;
#endif
        }
    }
}