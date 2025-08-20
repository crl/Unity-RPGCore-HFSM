using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class SkillState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            animationPlayer.RequestTransition("Skill");
            Debug.Log("Skill Execute.");
        }

        public override void OnUpdate()
        {
            if (animationPlayer.CurrentFinishPlaying)
            {
                ctrl.SetBool("IsAttack", true);
                ctrl.SetBool("IsRoll", false);
                ctrl.SetBool("IsSkill", false);
            }
        }
    }
}