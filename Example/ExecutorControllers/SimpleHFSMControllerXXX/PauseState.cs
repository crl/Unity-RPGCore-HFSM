using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class PauseState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Debug.Log("Pause");
            animationPlayer.Stop();
        }

        public override void OnExit()
        {
            animationPlayer.Play();
        }

        public override bool OnExitRequset()
        {
            return !ctrl.GetBool("Pause");
        }
    }
}