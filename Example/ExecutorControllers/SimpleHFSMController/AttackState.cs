using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class AttackState : AbsState
    {

        public override void OnUpdate()
        {
            if (animationPlayer.CurrentFinishPlaying)
            {
                ctrl.SetBool("IsAttack", false);
                ctrl.SetBool("IsRoll", true);
                ctrl.SetBool("IsSkill", false);
            }
        }
    }
}