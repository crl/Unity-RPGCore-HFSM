
using UnityEngine;
using RPGCore.AI.HFSM;
public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class PauseState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
    }
}