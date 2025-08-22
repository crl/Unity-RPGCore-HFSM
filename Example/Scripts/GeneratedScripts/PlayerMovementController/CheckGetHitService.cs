
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class CheckGetHitService : Service
    {
        private PlayerManager playerManager;
        public override void OnEnter()
        {
            base.OnEnter();

            playerManager = ctrl.gameObject.GetComponent<PlayerManager>();
        }

        protected override void doUpdate()
        {
            base.doUpdate();
            
            if (playerManager.beAttack)
            {
                if (ctrl.executeState != "roll" && ctrl.executeState != "dodge") ctrl.SetTrigger("GetHit");
                playerManager.beAttack = false;
            }
        }
    }
}