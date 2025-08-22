
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class ProcessRollOrDodgeService : Service
    {
        protected override void doUpdate()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ctrl.SetTrigger("RollOrDodge");
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                ctrl.SetBool("IsLookToTarget", !ctrl.GetBool("IsLookToTarget"));
            }
        }
    }
}