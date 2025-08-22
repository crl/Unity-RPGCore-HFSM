
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Get_hitState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animPlayer.RequestTransition("GetHit");
            ctrl.PauseService<ProcessInputService>();
        }

        public override void OnExit()
        {
            base.OnExit();
            ctrl.ResumeService<ProcessInputService>();
        }

        public override bool isCanExit()
        {
            return animPlayer.CurrentFinishPlaying;
        }
    }
}