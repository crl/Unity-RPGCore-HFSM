using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    public class AbsState:State
    {
        protected AnimationPlayer animationPlayer;

        protected virtual string animName
        {
            get
            {
                return StringUtil.ToTitleCase(id);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            animationPlayer=ctrl.gameObject.GetComponent<AnimationPlayer>();

            if (string.IsNullOrEmpty(animName)==false)
            {
                animationPlayer.RequestTransition(animName);
            }
            
            Debug.Log($"{id} Execute.");
        }
        
        
    }
}