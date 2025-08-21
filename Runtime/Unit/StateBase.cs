using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HFSM
{
    public enum StateType
    {
        State,
        StateMachine
    }

    public class StateBase
    {
#if UNITY_EDITOR
        public static int stateWidth => 480;
        public static int stateHeight => 90;
#endif
        public string id
        {
            get => m_id;
            internal set => m_id = value;
        }

        protected string m_id;

        public StateType stateType => m_stateType;
        protected StateType m_stateType;

        public StateMachine parent
        {
            get;
            internal set;
        }
        
        public StateMachineScriptController ctrl { get; internal set; }

        public StateBundle[] executeStackSnapshot => m_executeStackSnapshot;
        protected StateBundle[] m_executeStackSnapshot = null;

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual bool isCanExit()
        {
            return true;
        }

        /// <summary>
        /// 在父StateMachine中获取到以当前state或StateMachine为起点的Transition
        /// </summary>
        public List<Transition> GetParentTransitionsStartWith()
        {
            if (parent is null) return null;
            return parent.transitions
                .Where(t => t.transitionType != TransitionType.Global && t.from.id == id).ToList();
        }

        /// <summary>
        /// 设置当前状态执行时的执行栈快照；便于随时将执行栈恢复到当前状态执行时的样子
        /// </summary>
        public void SetExecuteStackSnapshot(StateBundle[] stateBundles)
        {
            if (m_executeStackSnapshot != null) return;
            m_executeStackSnapshot = stateBundles;
        }
    }

    [Serializable]
    public class StateBaseData
    {
#if UNITY_EDITOR

        [HideInInspector] public Rect position;

        [HideInInspector] public bool isExecuting;

#endif
        public string id;
        public StateType stateType;
        public bool isDefault;

        [Multiline] public string description;
    }
}