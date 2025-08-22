using HFSM;
using UnityEngine;

public partial class SimpleHFSMController : StateMachineScriptController
{
    internal class SMServiceService : Service
    {
        //Don't delete or modify the #region & #endregion

        #region Method

        protected override void doUpdate()
        {
            Debug.Log("SMService Execute");
        }

        #endregion Method
    }
}