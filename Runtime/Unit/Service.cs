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
        public bool isPaused
        {
            get;
            protected internal set;
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
            isPaused = false;
        }


        internal void OnUpdate()
        {
            if (isPaused) return;
            doUpdate();
        }

        protected virtual void doUpdate()
        {
        }

        public virtual void OnExit()
        {
            isPaused = false;
        }

        public void Pause() => isPaused = true;

        public void Resume() => isPaused = false;
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