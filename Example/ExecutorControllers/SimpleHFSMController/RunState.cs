using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
	internal class RunState : AbsState
	{

		public override void OnUpdate()
		{
			if (animationPlayer.CurrentFinishPlaying)
			{
				ctrl.SetBool("IsRun", false);
				ctrl.SetBool("IsWalk", false);
				ctrl.SetBool("IsIdle", true);
			}
		}
	}
}
