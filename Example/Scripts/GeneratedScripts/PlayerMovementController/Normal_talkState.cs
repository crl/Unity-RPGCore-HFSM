
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Normal_talkState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animPlayer.RequestTransition("NormalTalk");
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