#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HFSM
{
    /// <summary>
    /// 状态机Controller;不用做运行时
    /// </summary>
    public partial class StateMachineExecutorController:ScriptableObject
    {
        private SerializedObject serializedObject;
        public void Update()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }
            serializedObject.Update();
        }
        
        /// <summary>
        /// 根据此Controller存储的信息生成实际的运行时Controller
        /// </summary>
        public void GenerateScriptController()
        {
            if (controllerConfig.isCustom && controllerConfig.FilePath != "")
            {
                generateFilePath = Application.dataPath.Replace("Assets", "") + controllerConfig.FilePath + "/";
            }
            else
            {
                scriptableObjectAssetPath = AssetDatabase.GetAssetPath(this);
                if (scriptableObjectAssetPath != "")
                {
                    scriptableObjectAssetPath =
                        scriptableObjectAssetPath.Remove(scriptableObjectAssetPath.LastIndexOf("/") + 1);
                }

                generateFilePath = Application.dataPath.Replace("Assets", "") + scriptableObjectAssetPath;
            }

            realScriptControllerName = name.Replace(" ", "");
            GenerateInitScript(generateFilePath);
            GenerateConstructScript(generateFilePath);
            GenerateAbsStateScript(generateFilePath);

            var tpl = @"
using HFSM;
public partial class {0} : StateMachineScriptController
{
    internal class {1} : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
    }
}";
            foreach (var state in states)
            {
                var cls = GetClassNameBy(state.id, "State");

                var str = tpl.Replace("{0}", realScriptControllerName);
                str = str.Replace("{1}", cls);
                var path = generateFilePath + realScriptControllerName + $"/{cls}.cs";
                if (File.Exists(path) == false)
                {
                    File.WriteAllText(path, str, Encoding.UTF8);
                }
            }

            foreach (var stateMachine in stateMachines)
            {
                foreach (var service in stateMachine.services)
                {
                    var cls = GetClassNameBy(service.id, "Service");

                    var str = tpl.Replace("{0}", realScriptControllerName);
                    str = str.Replace("{1}", cls);
                    str = str.Replace("AbsState", "Service");
                    var path = generateFilePath + realScriptControllerName + $"/{cls}.cs";
                    if (File.Exists(path) == false)
                    {
                        File.WriteAllText(path, str, Encoding.UTF8);
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        private void GenerateInitScript(string filePath)
        {
            var tpm = @"using HFSM;
public partial class {0} : StateMachineScriptController
{
	public override void Init()
	{
	}
}";
            var dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            var str = tpm.Replace("{0}", realScriptControllerName);
            var path = filePath + realScriptControllerName + ".cs";
            if (File.Exists(path) == false)
            {
                File.WriteAllText(path, str, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成构造层状态机的代码脚本
        /// </summary>
        private void GenerateConstructScript(string filePath)
        {
            var tpm = @"//Automatically generated code
using HFSM;
[StateMachineController(ControllerName = ""{0}"")]
public partial class {0} : StateMachineScriptController
{
	public override StateMachine ConstructStateMachine()
	{
		StateMachineHandler.BeginStateMachine(this, ""{1}"")
            {2}
			.EndHandle();
		return StateMachineHandler.EndStateMachine();
	}
}";
            var rootData = stateMachines.Find(sm => sm.isRoot);
            var sb = new List<string>();
            GenerateConstructCode(rootData, sb);
            var code = "";
            foreach (var item in sb)
            {
                code += $"\t\t\t{item}\n";
            }

            code.TrimStart('\t');
            code.TrimEnd('\n');
            var str = tpm.Replace("{0}", realScriptControllerName);
            str = str.Replace("{1}", rootData.id);
            str = str.Replace("{2}", code);

            var path = filePath + realScriptControllerName + "_Construct.cs";
            File.WriteAllText(path, str, Encoding.UTF8);
        }

        private void GenerateAbsStateScript(string filePath)
        {
            //状态基类
            var tpm = @"//Automatically generated code
using HFSM;
public partial class {0} : StateMachineScriptController
{
    public class AbsState:State
    {
        protected {0} ctrlT;
        public override void OnEnter()
        {
            base.OnEnter();
            ctrlT=this.ctrl as {0};
        }
    }
}";
            var str = tpm.Replace("{0}", realScriptControllerName);

            var dir = filePath + realScriptControllerName;
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            var path = dir + "/AbsState.cs";
            if (File.Exists(path) == false)
            {
                File.WriteAllText(path, str, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 自顶向下生成构造状态机代码；
        /// </summary>
        private void GenerateConstructCode(StateMachineData stateMachineData, List<string> sb)
        {
            var allStates = new List<StateBaseData>();
            allStates.AddRange(states);
            allStates.AddRange(stateMachines);

            //先生成service
            for (int i = 0, len = stateMachineData.services.Count; i < len; i++)
            {
                var service = stateMachineData.services[i];
                var cls = GetClassNameBy(service.id, "Service");
                sb.Add(
                    $".AddService<{cls}>(ServiceType.{service.serviceType.ToString()},{service.customInterval})");
            }

            //再添加所有的state与StateMachine
            for (int i = 0; i < stateMachineData.childStates.Count; i++)
            {
                StateBaseData state = allStates.Find(s => s.id == stateMachineData.childStates[i]);
                if (state.stateType == StateType.StateMachine)
                {
                    sb.Add(
                        $".AddStateMachine(\"{state.id}\", {(state.id == stateMachineData.defaultState).ToString().ToLower()})");
                    GenerateConstructCode(state as StateMachineData, sb);
                }
                else
                {
                    var cls = GetClassNameBy(state.id, "State");
                    if ((state as StateData).isTemporary)
                    {
                        sb.Add($".AddTemporaryState<{cls}>(\"{state.id}\")");
                    }
                    else
                    {
                        sb.Add(
                            $".AddState<{cls}>(\"{state.id}\", {(state.id == stateMachineData.defaultState).ToString().ToLower()})");
                    }

                    if ((state as StateData).canExitHandle)
                    {
                    }
                }
            }

            //最后处理所有的Transition
            for (int i = 0, len = stateMachineData.transitions.Count; i < len; i++)
            {
                var transition = transitions.Find(t => t.id == stateMachineData.transitions[i]);
                //switch to
                var state = allStates.Find(s => s.id == transition.to);
                if (state.stateType == StateType.State)
                {
                    bool isTemporary = states.Find(s => s.id == transition.to).isTemporary;
                    sb.Add($".SwitchHandle(\"{transition.from}\")" +
                           $".ToState(\"{transition.to}\",{isTemporary.ToString().ToLower()})");
                }
                else
                {
                    sb.Add($".SwitchHandle(\"{transition.from}\")" +
                           $".ToStateMachine(\"{transition.to}\")");
                }

                //conditions
                foreach (var conditionName in transition.baseConditionsName)
                {
                    sb.Add($".Condition(()=>{conditionName})");
                }

                foreach (var parameterCondition in transition.parameterConditionDatas)
                {
                    Parameter param = parameters.Find(p => p.name == parameterCondition.parameterName);
                    if (param.type == ParameterType.Bool)
                    {
                        string value = parameterCondition.compareValue >= 1.0f ? "true" : "false";
                        sb.Add($".BoolCondition(\"{param.name}\",{value})");
                    }
                    else if (param.type == ParameterType.Trigger)
                    {
                        sb.Add($".TriggerCondition(\"{param.name}\")");
                    }
                    else if (param.type == ParameterType.Int)
                    {
                        sb.Add(
                            $".IntCondition(\"{param.name}\",CompareType.{parameterCondition.compareType},{parameterCondition.compareValue})");
                    }
                    else if (param.type == ParameterType.Float)
                    {
                        sb.Add(
                            $".FloatCondition(\"{param.name}\",CompareType.{parameterCondition.compareType},{parameterCondition.compareValue})");
                    }
                }
            }

            sb.Add(".FinishHandle()");
        }

        private string GetClassNameBy(string str, string postfix)
        {
            var name = str.Replace(" ", "");
            return StringUtil.ToTitleCase(name) + postfix;
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        public void Save()
        {
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///创建一个State
        /// </summary>
        public StateData CreateState(Rect rect, StateMachineData currentStateMachine)
        {
            int count = states.Count(s => s.id.Contains("New State"));
            StateData state = new StateData()
            {
                id = "New State " + count,
                stateType = StateType.State,
                position = rect,
                isDefault = currentStateMachine.childStates.Count == 0,
            };
            Undo.IncrementCurrentGroup();
            states.Add(state);
            currentStateMachine.childStates.Add(state.id);
            if (state.isDefault)
            {
                currentStateMachine.defaultState = state.id;
            }

            Save();
            return state;
        }

        /// <summary>
        ///创建一个StateMachine
        /// </summary>
        public StateMachineData CreateStateMachine(Rect rect, StateMachineData currentStateMachine)
        {
            int count = stateMachines.Count(s => s.id.Contains("New StateMachine"));
            StateMachineData stateMachine = new StateMachineData()
            {
                id = "New StateMachine " + count,
                stateType = StateType.StateMachine,
                position = rect,
                isDefault = currentStateMachine.childStates.Count == 0,
            };
            stateMachines.Add(stateMachine);
            currentStateMachine.childStates.Add(stateMachine.id);
            CreateState(new Rect(600, 400, StateBase.stateWidth, StateBase.stateHeight), stateMachine);
            Save();
            return stateMachine;
        }

        /// <summary>
        ///创建一个Transition
        /// </summary>
        public void CreateTransition(StateMachineData stateMachine, StateBaseData from, StateBaseData to)
        {
            if (to.id == StateMachine.anyState ||
                to.id == StateMachine.entryState ||
                from == to)
            {
                return;
            }
            TransitionData transition = new TransitionData()
            {
                id = GUID.Generate().ToString(),
                from = from.id,
                to = to.id
            };
            transitions.Add(transition);
            stateMachine.transitions.Add(transition.id);
            Save();
        }

        /// <summary>
        ///创建一个Parameter
        /// </summary>
        public void CreateParamter(ParameterType parameterType)
        {
            int count = parameters.Count(p => p.name.Contains("New Param"));
            string name = "New Param " + count;
            Parameter parameter = new Parameter(name, parameterType, 0.0f);
            
            parameters.Add(parameter);
            Save();
        }

        /// <summary>
        ///创建一个Transition的ParamterCondition
        /// </summary>
        public void CreateParamterCondition(TransitionData transition)
        {
            ParameterConditionData parameterCondition = new ParameterConditionData();
            Parameter defaultParam = parameters.First();
            if (defaultParam != null)
            {
                parameterCondition.parameterName = defaultParam.name;
                parameterCondition.compareType = CompareType.Equal;
                parameterCondition.compareValue = 0.0f;
            }
            transition.parameterConditionDatas.Add(parameterCondition);
            Save();
        }

        /// <summary>
        ///创建一个StateMachine的Service
        /// </summary>
        public void CreateService(StateMachineData stateMachine, ServiceType serviceType)
        {
            ServiceData service = new ServiceData();
            service.serviceType = serviceType;
            int count = stateMachine.services.Count(s => s.id.Contains("NewService"));
            service.id = "NewService" + count;
            
            stateMachine.services.Add(service);
            Save();
        }

        /// <summary>
        /// 删除State或StateMachine
        /// </summary>
        public void DeleteState(StateMachineData stateMachine, StateBaseData state)
        {
            if (state != null)
            {
                if (state.stateType == StateType.State)
                {
                    states.Remove(states.Find(s => s.id == state.id));
                }
                else
                {
                    StateMachineData machineData = (state as StateMachineData);
                    List<StateBaseData> allStates = new List<StateBaseData>();
                    allStates.AddRange(states.FindAll(s => machineData.childStates.Contains(s.id)));
                    allStates.AddRange(stateMachines.FindAll(s => machineData.childStates.Contains(s.id)));
                    foreach (var s in allStates)
                    {
                        DeleteState((state as StateMachineData), s);
                    }

                    stateMachines.Remove(stateMachines.Find(s => s.id == state.id));
                    foreach (var transition in machineData.transitions)
                    {
                        transitions.Remove(transitions.Find(t => t.id == transition));
                    }
                }
                stateMachine.childStates.Remove(state.id);
            }

            Save();
        }

        /// <summary>
        /// 删除Transition
        /// </summary>
        public void DeleteTransition(StateMachineData stateMachine, TransitionData transition)
        {
            stateMachine.transitions.Remove(transition.id);
            transitions.Remove(transition);
            Save();
        }

        /// <summary>
        /// 删除State或StateMachine时同步删除与此链接的Transition
        /// </summary>
        public void DeleteTransition(StateMachineData stateMachine, StateBaseData state)
        {
            List<TransitionData> datas = transitions.FindAll(t => t.from == state.id || t.to == state.id);
            stateMachine.transitions.RemoveAll(t => datas.Select(d => d.id).Contains(t));
            transitions.RemoveAll(t => datas.Contains(t));
            Save();
        }

        /// <summary>
        /// 删除一个Parameter
        /// </summary>
        public void DeleteParameter(int index)
        {
            Parameter parameter = parameters[index];
            foreach (var t in transitions)
            {
                var condition = t.parameterConditionDatas.Find(pc => pc.parameterName == parameter.name);
                t.parameterConditionDatas.Remove(condition);
            }

            parameters.Remove(parameter);
            Save();
        }

        /// <summary>
        /// 删除一个Transition中的ParameterCondition
        /// </summary>
        public void DeleteParameterCondition(TransitionData transition, int index)
        {
            transition.parameterConditionDatas.RemoveAt(index);
            Save();
        }

        /// <summary>
        /// 删除一个StateMachine中的Service
        /// </summary>
        public void DeleteService(StateMachineData stateMachine, ServiceData service)
        {
            stateMachine.services.Remove(service);
            Save();
        }

        /// <summary>
        /// 重命名一个Parameter
        /// </summary>
        public void RenameParameter(Parameter parameter, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                return;
            if (parameters.Select(p => p.name).Contains(newName))
                return;
            
            foreach (var t in transitions)
            {
                var condition = t.parameterConditionDatas.Find(pc => pc.parameterName == parameter.name);
                if (condition != null)
                {
                    condition.parameterName = newName;
                }
            }

            parameter.name = newName;
            Save();
        }

        /// <summary>
        /// 重命名一个State或StateMachine
        /// </summary>
        public void RenameState(StateBaseData state, string newName, bool description = false, bool canExit = false)
        {
            if (state.stateType == StateType.State)
            {
                if (canExit)
                {
                    (state as StateData).canExitDescription = newName;
                    Save();
                    return;
                }
                else if (description)
                {
                    state.description = newName;
                    Save();
                    return;
                }
                else
                {
                    if (string.IsNullOrEmpty(newName)) return;
                    //todo
                }
            }
            
            foreach (var sm in stateMachines)
            {
                int index = sm.childStates.FindIndex(s => s == state.id);
                if (index != -1)
                {
                    sm.childStates[index] = newName;
                    if (state.isDefault)
                    {
                        sm.defaultState = newName;
                    }

                    break;
                }
            }

            foreach (var t in transitions)
            {
                if (t.to == state.id)
                {
                    t.to = newName;
                }
                else if (t.from == state.id)
                {
                    t.from = newName;
                }
            }

            state.id = newName;
            Save();
        }

        /// <summary>
        /// 重命名一个Service
        /// </summary>
        public void RenameService(ServiceData serviceData, string newName)
        {
            if (string.IsNullOrEmpty(newName)) return;

            serviceData.id = newName;
            Save();
        }

        /// <summary>
        /// 跳转到对应的脚本文件
        /// </summary>
        public void JumpToScript(string id,string postfix)
        {
            var cls = GetClassNameBy(id, postfix);
            var filePath = controllerConfig.isCustom
                ? controllerConfig.FilePath + "/"
                : scriptableObjectAssetPath;

            filePath += realScriptControllerName + $"/{cls}.cs";


            Debug.Log(filePath);
            if (!AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(filePath), 10))
            {
                Debug.Log("Jump failure.");
            }
        }
    }
}
#endif