using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class WalkState : AbsState
    {
        public override void OnUpdate()
        {
            if (animationPlayer.CurrentFinishPlaying)
            {
                ctrl.SetBool("IsIdle", false);
                ctrl.SetBool("IsWalk", false);
                ctrl.SetBool("IsRun", true);
            }
        }
    }
}