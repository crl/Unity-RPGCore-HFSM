using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class PauseState : AbsState
    {
        protected override string animName => "";

        public override void OnEnter()
        {
            base.OnEnter();

            animationPlayer.Stop();
        }

        public override void OnExit()
        {
            animationPlayer.Play();
        }

        public override bool isCanExit()
        {
            return !ctrl.GetBool("Pause");
        }
    }
}