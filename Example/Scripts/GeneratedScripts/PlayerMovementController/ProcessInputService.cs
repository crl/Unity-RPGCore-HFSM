
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class ProcessInputService : Service
    {
        
        private PlayerManager playerManager;
        public override void OnEnter()
        {
            base.OnEnter();

            playerManager = ctrl.gameObject.GetComponent<PlayerManager>();
        }
        protected override void doUpdate()
        {
            ref var moveVec =ref playerManager.moveVec;
            moveVec.x = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            moveVec.y = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
            moveVec = moveVec.normalized;
            ctrl.SetBool("IsIdle", moveVec.sqrMagnitude == 0);
            ctrl.SetBool("IsWalk", moveVec.sqrMagnitude != 0);
            ctrl.SetBool("IsRun", Input.GetKey(KeyCode.LeftShift));
            if (moveVec.sqrMagnitude != 0)
            {
                if (!ctrl.GetBool("IsLookToTarget"))
                {
                    ctrl.gameObject.transform.forward = Vector3.Lerp(ctrl.gameObject.transform.forward, new Vector3(moveVec.x, 0, moveVec.y), Time.deltaTime * 10);
                }
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ctrl.SetBool("IsOnBattle", !ctrl.GetBool("IsOnBattle"));
            }
        }
    }
}