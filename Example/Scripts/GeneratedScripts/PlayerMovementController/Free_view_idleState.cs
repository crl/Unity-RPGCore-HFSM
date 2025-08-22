
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Free_view_idleState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            animPlayer.RequestTransition("BattleIdle");
        }
    }
}