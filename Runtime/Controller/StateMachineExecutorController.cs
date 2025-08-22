using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HFSM
{
    /// <summary>
    /// 状态机Controller;不用做运行时
    /// </summary>
    [CreateAssetMenu(fileName = "FSM Executor Controller", menuName = "State Machine/Executor Controller", order = 4)]
    public partial class StateMachineExecutorController : ScriptableObject
    {
        [HideInInspector] public string realScriptControllerName;

        //controller中的parameter
        public List<Parameter> parameters = new();

        //controller中的state
        public List<StateData> states = new();

        //controller中的StateMachine
        public List<StateMachineData> stateMachines = new()
        {
            new StateMachineData()
            {
                id = "Root",
                isRoot = true,
                stateType = StateType.StateMachine,
            }
        };

        //controller中的Transition
        public List<TransitionData> transitions = new();

        private string scriptableObjectAssetPath;


        //配置文件SO
        [SerializeField]
        internal StateMachineControllerConfig controllerConfig = new();


        /// <summary>
        /// 根据此获取运行时Controller
        /// </summary>
        public StateMachineScriptController GetController()
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains("UnityEngine") || assembly.FullName.Contains("UnityEditor") ||
                    assembly.FullName.Contains("Unity") || assembly.FullName.Contains("System") ||
                    assembly.FullName.Contains("Microsoft")) continue;
                List<Type> result = assembly.GetTypes().Where(type =>
                {
                    return type.IsClass && !type.IsAbstract &&
                           type.IsSubclassOf(typeof(StateMachineScriptController)) &&
                           type.GetCustomAttribute<StateMachineControllerAttribute>() != null;
                }).ToList();
                types.AddRange(result);
            }

            Type target = types.Find(type =>
                type.GetCustomAttribute<StateMachineControllerAttribute>().ControllerName == realScriptControllerName);
            if (target != null)
            {
                return (StateMachineScriptController)Activator.CreateInstance(target);
            }

            return null;
        }

        /// <summary>
        /// 根据运行时Controller生成并初始化状态机
        /// </summary>
        public StateMachine GetExecuteStateMachine(StateMachineExecutor executor,
            out StateMachineScriptController scriptController)
        {
            scriptController = GetController();
            if (scriptController != null)
            {
                scriptController.executor = executor;
                scriptController.gameObject = executor.gameObject;
                scriptController.PrepareParameters(parameters);
                scriptController.Init();
                return scriptController.ConstructStateMachine();
            }

            return null;
        }
    }

    public enum MethodType
    {
        State,
        Service,
        CanExit
    }
}