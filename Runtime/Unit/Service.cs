using System;
using UnityEngine;

namespace RPGCore.AI.HFSM
{
    public class Service
    {
        public string id { get; private set; }

        public ServiceType serviceType => m_serviceType;
        private ServiceType m_serviceType;
        public bool pauseService => m_pauseService;
        private bool m_pauseService = false;
        public StateMachine serviceOwner => _serviceOwner;
        private StateMachine _serviceOwner;

        public float customInterval => m_customInterval;
        private float m_customInterval;
        public Timer timer;

        public Service()
        {
            timer = new Timer();
        }

        internal void Init(string serviceId,
            ServiceType type = ServiceType.Update, float customInterval = 0f)
        {
            id = serviceId;
            m_serviceType = type;
            m_customInterval = customInterval;
        }

        public virtual void OnEnter()
        {
            timer.Reset();
            m_pauseService = false;
        }


        internal void OnUpdate()
        {
            if (m_pauseService) return;
            doUpdate();
        }

        protected virtual void doUpdate()
        {
        }

        public virtual void OnExit()
        {
            m_pauseService = false;
        }

        public void Pause() => m_pauseService = true;

        public void Continue() => m_pauseService = false;
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