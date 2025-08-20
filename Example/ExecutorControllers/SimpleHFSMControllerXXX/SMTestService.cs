using RPGCore.AI.HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class SMTestService : Service
    {
        //Don't delete or modify the #region & #endregion

        #region Method

        [Service("SMTest/SMService")]
        protected override void doUpdate()
        {
            Debug.Log("SMService Execute");
        }

        #endregion Method
    }
}