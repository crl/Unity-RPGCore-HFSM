
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Normal_idleState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animPlayer.RequestTransition("NormalIdle");
        }
    }
}