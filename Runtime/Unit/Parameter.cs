using UnityEngine;
using UnityEngine.Serialization;

namespace HFSM
{
    public enum ParameterType
    {
        Int,
        Float,
        Bool,
        Trigger
    }

    [System.Serializable]
    public class Parameter
    {
        [FormerlySerializedAs("m_name")] [SerializeField]
        protected string _name;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        [FormerlySerializedAs("m_type")] [SerializeField] protected ParameterType _type;

        public ParameterType type => _type;

        public float baseValue;

        public Parameter(string name, ParameterType type, float value)
        {
            _name = name;
            _type = type;
            baseValue = value;
        }
    }
}