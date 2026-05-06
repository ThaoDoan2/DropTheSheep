using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qutility.CustomEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionPropertyName { get; }

        public ShowIfAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }
    }
}