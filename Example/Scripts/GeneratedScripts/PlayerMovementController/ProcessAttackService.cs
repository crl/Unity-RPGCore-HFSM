
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class ProcessAttackService : Service
    {
        protected override void doUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ctrl.SetTrigger("Attack");
            }
        }
    }
}