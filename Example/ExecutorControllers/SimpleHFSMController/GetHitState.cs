using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class GetHitState : AbsState
    {
        public override bool isCanExit()
        {
            return animationPlayer.CurrentFinishPlaying;
        }
    }
}