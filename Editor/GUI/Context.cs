using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HFSM
{
    public class Context
    {
        private StateMachineExecutorController _controller;

        public StateMachineExecutorController controller
        {
            get
            {
                var ctrl = GetController();
                if (ctrl != null && ctrl != _controller)
                {
                    _controller = ctrl;
                    Reset();
                }

                return _controller;
            }
        }

        private StateMachineExecutor m_executor;

        public StateMachineExecutor executor
        {
            get
            {
                if (Application.isPlaying)
                {
                    if ((Selection.activeObject as GameObject) != null &&
                        (Selection.activeObject as GameObject).GetComponent<StateMachineExecutor>() != null)
                    {
                        m_executor = (Selection.activeObject as GameObject).GetComponent<StateMachineExecutor>();
                    }
                }

                return m_executor;
            }
        }

        public List<StateBaseData> selectedStates = new();

        public TransitionData selectedTransition = null;

        private StateMachineData m_currentStateMachine;

        public StateMachineData currentStateMachine
        {
            get => m_currentStateMachine;
            set
            {
                if (m_currentStateMachine != value)
                {
                    m_currentStateMachine = value;
                    UpdateCurrentChildStatesData();
                    UpdateCurrentTransitionData();
                }
            }
        }

        public StateMachineData nextStateMachine = null;
        public List<StateMachineData> stateMachinePath = new();

        public List<StateBaseData> currentChildStatesData = new();

        public List<TransitionData> currentTransitionData = new();

        public bool isPreviewTransition;
        public StateBaseData preFrom;
        public StateBaseData preTo;

        public float zoomFactor { get; set; } = 0.3f;

        public Vector2 dragOffset { get; set; } = Vector2.zero;

        private StateMachineExecutorController GetController()
        {
            if ((Selection.activeObject as StateMachineExecutorController) != null)
            {
                return (StateMachineExecutorController)Selection.activeObject;
            }

            if ((Selection.activeObject as GameObject) != null &&
                (Selection.activeObject as GameObject).GetComponent<StateMachineExecutor>() != null)
            {
                if ((Selection.activeObject as GameObject).GetComponent<StateMachineExecutor>().executorController !=
                    null)
                {
                    return (Selection.activeObject as GameObject).GetComponent<StateMachineExecutor>()
                        .executorController;
                }
            }

            return null;
        }

        public void StartPriviewTransition(StateBaseData fromState)
        {
            isPreviewTransition = true;
            this.preFrom = fromState;
        }

        public void StopPriviewTransition()
        {
            isPreviewTransition = false;
            this.preFrom = null;
        }

        public void Reset()
        {
            currentStateMachine = controller.stateMachines.Find(sm => sm.isRoot);
            stateMachinePath.Clear();
            stateMachinePath.Add(currentStateMachine);
            this.zoomFactor = 0.3f;
            this.dragOffset = Vector2.zero;
        }

        public void ClearAllSelectNode()
        {
            selectedStates.Clear();
            selectedTransition = null;
        }

        public void UpdateCurrentChildStatesData()
        {
            currentChildStatesData.Clear();
            currentChildStatesData.AddRange(_controller.states.FindAll(s =>
                m_currentStateMachine.childStates.Contains(s.id)));
            currentChildStatesData.AddRange(
                _controller.stateMachines.FindAll(s => m_currentStateMachine.childStates.Contains(s.id)));
            currentChildStatesData.Add(m_currentStateMachine.any);
            currentChildStatesData.Add(m_currentStateMachine.entry);
        }

        public void UpdateCurrentTransitionData()
        {
            currentTransitionData.Clear();
            currentTransitionData.AddRange(
                _controller.transitions.FindAll(t => m_currentStateMachine.transitions.Contains(t.id)));
        }

        /// <summary>
        /// 重置界面
        /// </summary>
        public void ResetByUndo()
        {
            var paths = this.stateMachinePath;
            var len = paths.Count;
            if (len > 0)
            {
                for (int i = len - 1; i >= 0; i--)
                {
                    var item = controller.stateMachines.Find(sm => sm.id == paths[i].id);

                    if (item == null)
                    {
                        paths.RemoveAt(i);
                    }
                    else
                    {
                        paths[i] = item;
                    }
                }

                len = paths.Count;
                this.currentStateMachine = paths[len - 1];
            }
            else
            {
                this.Reset();
            }

            var selectedStates = this.selectedStates;
            len = selectedStates.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                var item = this.selectedStates.Find(sm => sm.id == selectedStates[i].id);
                if (item == null)
                {
                    selectedStates.RemoveAt(i);
                }
                else
                {
                    selectedStates[i] = item;
                }
            }
        }
    }
}