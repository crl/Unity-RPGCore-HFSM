
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Look_to_target_idleState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            animPlayer.RequestTransition("BattleIdle");
        }
    }
}