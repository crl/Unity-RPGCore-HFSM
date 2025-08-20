using RPGCore.AI.HFSM;
using RPGCore.Animation;

public partial class SimpleHFSMController : StateMachineScriptController
{
    public class AbsState:State
    {
        protected SimpleHFSMController ctrl;
        protected AnimationPlayerManager animationPlayer;
        public override void OnEnter()
        {
            base.OnEnter();

            ctrl=this._parentStateMachine.ctrl as SimpleHFSMController;

            animationPlayer=ctrl.animationPlayer;
        }
    }
}