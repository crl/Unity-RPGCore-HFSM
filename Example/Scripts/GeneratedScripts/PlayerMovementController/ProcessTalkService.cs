
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class ProcessTalkService : Service
    {
        protected override void doUpdate()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ctrl.SetTrigger("Talk");
            }
        }
    }
}