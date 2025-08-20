using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class GetHitState : AbsState
    {
        //Don't delete or modify the #region & #endregion

        public override void OnEnter()
        {
            base.OnEnter();

            animationPlayer.RequestTransition("GetHit");
        }

        public override bool OnExitRequset()
        {
            return animationPlayer.CurrentFinishPlaying;
        }
    }
}