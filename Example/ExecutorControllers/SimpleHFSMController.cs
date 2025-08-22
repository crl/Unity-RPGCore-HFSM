using HFSM;

public partial class SimpleHFSMController : StateMachineScriptController
{
	private AnimationPlayer animationPlayer;
	public override void Init()
	{
		animationPlayer = gameObject.GetComponent<AnimationPlayer>();
	}
}