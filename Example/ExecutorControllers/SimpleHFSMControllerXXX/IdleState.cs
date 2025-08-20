using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class IdleState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            animationPlayer.RequestTransition("Idle");
            Debug.Log("Idle Execute.");
        }

        public override void OnUpdate()
        {
            if (animationPlayer.CurrentFinishPlaying)
            {
                ctrl.SetBool("IsIdle", false);
                ctrl.SetBool("IsRun", false);
                ctrl.SetBool("IsWalk", true);
            }
        }
    }
}