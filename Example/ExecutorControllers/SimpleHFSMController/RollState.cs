using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class RollState : AbsState
    {
        public override void OnUpdate()
        {
            if (animationPlayer.CurrentFinishPlaying)
            {
                ctrl.SetBool("IsAttack", false);
                ctrl.SetBool("IsRoll", false);
                ctrl.SetBool("IsSkill", true);
            }
        }
    }
}