
using UnityEngine;
using RPGCore.AI.HFSM;
public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class IdleState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
    }
}