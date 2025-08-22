
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Free_view_sprintState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            animPlayer.RequestTransition("FreeViewSprint");
        }
    }
}