//Automatically generated code
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    public class AbsState:State
    {
        protected AnimationPlayer animPlayer;
        public override void OnEnter()
        {
            base.OnEnter();
            animPlayer = ctrl.gameObject.GetComponent<AnimationPlayer>();
        }
    }
}