
using HFSM;
public partial class PlayerMovementController : StateMachineScriptController
{
    internal class AttackState : AbsState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animPlayer.RequestTransition("BattleAttack");
            ctrl.PauseService<ProcessInputService>();
        }

        public override void OnExit()
        {
            ctrl.ResumeService<ProcessInputService>();
        }
    }
}