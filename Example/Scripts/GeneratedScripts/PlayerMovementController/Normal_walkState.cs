
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Normal_walkState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            animPlayer.RequestTransition("NormalWalk");
        }
    }
}