using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class AttackState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            animationPlayer.RequestTransition("Attack");
            Debug.Log("Attack Execute.");
        }

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