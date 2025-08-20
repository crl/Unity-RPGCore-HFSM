//Automatically generated code
using RPGCore.AI.HFSM;
public partial class SimpleHFSMController : StateMachineScriptController
{
    public class AbsState:State
    {
        protected SimpleHFSMController ctrl;
        public override void OnEnter()
        {
            base.OnEnter();
            ctrl=this._parentStateMachine.ctrl as SimpleHFSMController;
        }
    }
}