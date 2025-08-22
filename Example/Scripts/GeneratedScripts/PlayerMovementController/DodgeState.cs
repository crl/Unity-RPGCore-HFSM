
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class DodgeState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var playerManager = ctrl.gameObject.GetComponent<PlayerManager>();
            
            if (playerManager.moveVec.x == 1) { animPlayer.RequestTransition("DodgeRight"); }
            else if (playerManager.moveVec.x == -1) { animPlayer.RequestTransition("DodgeLeft"); }
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