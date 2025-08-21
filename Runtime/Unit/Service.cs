using System;
using UnityEngine;

namespace HFSM
{
    public class Service
    {
        public string id { get; private set; }

        public ServiceType serviceType
        {
            get;
            private set;
        }
        public bool pauseService
        {
            get;
            private set;
        }

        public StateMachine parent
        {
            get;
            internal set;
        }
        
        public StateMachineScriptController ctrl { get; internal set; }

        public float customInterval
        {
            get;
            private set;
        }
        public Timer timer;

        public Service()
        {
            timer = new Timer();
        }

        internal void Init(string serviceId,
            ServiceType type = ServiceType.Update, float interval = 0f)
        {
            id = serviceId;
            serviceType = type;
            customInterval = interval;
        }

        public virtual void OnEnter()
        {
            timer.Reset();
            pauseService = false;
        }


        internal void OnUpdate()
        {
            if (pauseService) return;
            doUpdate();
        }

        protected virtual void doUpdate()
        {
        }

        public virtual void OnExit()
        {
            pauseService = false;
        }

        public void Pause() => pauseService = true;

        public void Resume() => pauseService = false;
    }

    [Serializable]
    public class ServiceData
    {
        public string id;

        public ServiceType serviceType = ServiceType.Update;

        public float customInterval = 0;

        [Multiline] public string description;
    }
}