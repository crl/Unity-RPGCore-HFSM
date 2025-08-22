
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class RollState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            animPlayer.RequestTransition("Roll");
            ctrl.PauseService<ProcessInputService>();
        }

        public override void OnExit()
        {
            ctrl.ResumeService<ProcessInputService>();
        }

        public override bool isCanExit()
        {
            return animPlayer.CurrentFinishPlaying;
        }
    }
}