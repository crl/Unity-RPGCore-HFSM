using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class RollState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            animationPlayer.RequestTransition("Roll");
            Debug.Log("Roll Execute.");
        }

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