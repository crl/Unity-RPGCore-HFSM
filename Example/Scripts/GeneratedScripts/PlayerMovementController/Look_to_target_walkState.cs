
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class Look_to_target_walkState : AbsState
    {
        private PlayerManager playerManager;
        public override void OnEnter()
        {
            base.OnEnter();
            playerManager = ctrl.gameObject.GetComponent<PlayerManager>();
        }

        public override void OnUpdate()
        {
            ref var moveVec = ref playerManager.moveVec;
            if (Mathf.Abs(moveVec.x) == 1)
            {
                animPlayer.RequestTransition(moveVec.x == 1 ? "LookToTargetRight" : "LookToTargetLeft");
            }
            else if (Mathf.Abs(moveVec.y) == 1)
            {
                animPlayer.RequestTransition(moveVec.y == 1 ? "LookToTargetForward" : "LookToTargetBackward");
            }
        }
    }
}