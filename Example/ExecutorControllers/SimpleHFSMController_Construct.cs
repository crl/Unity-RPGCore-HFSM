//Automatically generated code
using RPGCore.AI.HFSM;
[StateMachineController(ControllerName = "SimpleHFSMController")]
public partial class SimpleHFSMController : StateMachineScriptController
{
	public override StateMachine ConstructStateMachine()
	{
		StateMachineHandler.BeginStateMachine(this, "Root")
            			.AddState<IdleState>("idle", true)
			.AddState<WalkState>("walk", false)
			.AddState<RunState>("run", false)
			.AddTemporaryState<GetHitState>("GetHit")
			.AddStateMachine("SMTest", false)
				.AddService<SMServiceService>("SMService",ServiceType.CustomInterval,1)
				.AddState<AttackState>("Attack", true)
				.AddState<RollState>("Roll", false)
				.AddState<SkillState>("Skill", false)
				.SwitchHandle("Attack").ToState("Roll",false)
					.BoolCondition("IsRoll",true)
				.SwitchHandle("Roll").ToState("Skill",false)
					.BoolCondition("IsSkill",true)
				.SwitchHandle("Skill").ToState("Attack",false)
					.BoolCondition("IsAttack",true)
				.FinishHandle()
			.AddTemporaryState<PauseState>("Pause")
			.SwitchHandle("idle").ToState("walk",false)
				.BoolCondition("IsWalk",true)
			.SwitchHandle("walk").ToState("run",false)
				.BoolCondition("IsRun",true)
			.SwitchHandle("run").ToState("idle",false)
				.BoolCondition("IsIdle",true)
			.SwitchHandle("Any").ToState("GetHit",true)
				.TriggerCondition("IsGetHit")
			.SwitchHandle("Any").ToStateMachine("SMTest")
				.TriggerCondition("ToSM")
			.SwitchHandle("SMTest").ToState("idle",false)
				.TriggerCondition("ExitSM")
			.SwitchHandle("Any").ToState("Pause",true)
				.BoolCondition("Pause",true)
			.FinishHandle()

			.EndHandle();
		return StateMachineHandler.EndStateMachine();
	}
}