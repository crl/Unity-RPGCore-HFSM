
using HFSM;
using UnityEngine;

public partial class PlayerMovementController : StateMachineScriptController
{
    internal class FaceToTargetService : Service
    {
        private PlayerManager playerManager;
        public override void OnEnter()
        {
            base.OnEnter();
            
            playerManager= ctrl.gameObject.GetComponent<PlayerManager>();
        }

        protected override void doUpdate()
        {
            var lookTarget=playerManager.lookTarget;
            
            var tForward = (lookTarget.position - ctrl.gameObject.transform.position).normalized;
            tForward.y = 0;
            ctrl.gameObject.transform.forward = Vector3.Lerp(ctrl.gameObject.transform.forward, tForward, Time.deltaTime * 20);
        }
    }
}