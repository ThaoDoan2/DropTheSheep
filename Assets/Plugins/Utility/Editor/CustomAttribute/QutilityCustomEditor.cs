using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Qutility.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
    public class QutilityCustomEditorScriptableObject : QutilityCustomEditor { }

    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class QutilityCustomEditor : Editor
    {
        // IEnumerable<MethodInfo> btnMethods;
        // IEnumerable<FieldInfo> fieldInfos;
        IEnumerable<MemberInfo> allMembers;

        protected virtual void OnEnable()
        {
            var btnMethods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttributes(typeof(ButtonMethodAttribute), true).Length > 0).OrderBy(f => f.MetadataToken);
            var fieldInfos = GetSerializableFields(target.GetType());

            List<MemberInfo> tmp = new();
            tmp.AddRange(fieldInfos);
            tmp.AddRange(btnMethods);

            allMembers = tmp;
        }

        public override void OnInspectorGUI()
        {
            DrawScriptHeader();
            serializedObject.Update();
            // Iterate over all fields and draw fields conditionally
            foreach (MemberInfo member in allMembers)
            {
                if (CanSerializeField(serializedObject, member))
                {
                    if (member is FieldInfo field)
                    {
                       
                        SerializedProperty property = serializedObject.FindProperty(field.Name);
                        if (property != null)
                        {
                            EditorGUILayout.PropertyField(property, true);
                        }
                    }
                    if (member is MethodInfo info)
                    {
                        Button(serializedObject.targetObject, info);
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        void DrawScriptHeader()
        {
            // Display the default script field, which acts as Unity's default clickable script header
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
            if (scriptProperty != null)
            {
                using (new EditorGUI.DisabledScope(true)) // Make the script field read-only
                {
                    EditorGUILayout.PropertyField(scriptProperty);
                }
            }
        }

        static bool CanSerializeField(SerializedObject serializedObject, MemberInfo field)
        {
            // Check if the field has the ShowIf attribute
            ShowIfAttribute showIfAttribute = field.GetCustomAttribute<ShowIfAttribute>();
            if (showIfAttribute != null)
            {
                // Evaluate the condition property specified in ShowIf
                return EvaluateCondition(serializedObject.targetObject, showIfAttribute.ConditionPropertyName);
            }
            return true;
        }
        static bool EvaluateCondition(object targetObject, string conditionPropertyName)
        {
            MemberInfo conditionProperty = GetPropertyInfo(targetObject.GetType(), conditionPropertyName);
            if (conditionProperty != null)
            {
                if (conditionProperty is FieldInfo field)
                    return (bool)field.GetValue(targetObject);
                if (conditionProperty is PropertyInfo property)
                    return (bool)property.GetValue(targetObject);
                if (conditionProperty is MethodInfo method)
                {
                    object[] defaultParams = method.GetParameters().Select(p => p.DefaultValue).ToArray();
                    return (bool)method.Invoke(targetObject, defaultParams);
                }

                return true;
            }
            Debug.LogWarning($"Condition property '{conditionPropertyName}' not found or not a boolean on '{targetObject.GetType()}'.");
            return true; // Default to true if condition not found or invalid
        }
        static MemberInfo GetPropertyInfo(System.Type type, string conditionPropertyName)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var stackType = type;
            while (stackType != null && stackType != typeof(object))
            {
                FieldInfo field = stackType.GetField(conditionPropertyName, bindingFlags);
                if (field != null) return field;

                PropertyInfo conditionProperty = stackType.GetProperty(conditionPropertyName, bindingFlags);
                if (conditionProperty != null) return conditionProperty;

                MethodInfo method = stackType.GetMethod(conditionPropertyName, bindingFlags);
                if (method != null) return method;

                stackType = stackType.BaseType;
            }
            return null;
        }
        static IEnumerable<FieldInfo> GetSerializableFields(System.Type type)
        {
            var stackType = type;
            Stack<System.Type> typeHierarchy = new();
            while (stackType != null && stackType != typeof(object))
            {
                typeHierarchy.Push(stackType); // Build the inheritance chain from parent to derived
                stackType = stackType.BaseType;
            }

            HashSet<int> seenFields = new();
            List<FieldInfo> fields = new();
            while (typeHierarchy.Count > 0) // Stop at the root `System.Object`
            {
                var currentType = typeHierarchy.Pop();
                var currentFields = currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(field =>
                            (field.IsPublic && field.GetCustomAttribute<HideInInspector>() == null &&
                             IsSerializableType(field.FieldType)) || // Public serializable fields
                            (!field.IsPublic && field.GetCustomAttribute<SerializeField>() != null &&
                             IsSerializableType(field.FieldType)) // Non-public with [SerializeField]
                    );
                foreach (var field in currentFields)
                {
                    if (seenFields.Add(field.MetadataToken)) // Add only if unique
                    {
                        fields.Add(field);
                    }
                }
            }

            return fields;
        }
        static bool IsSerializableType(System.Type type)
        {
            // return type.IsPrimitive ||
            //        type == typeof(string) ||
            //        type == typeof(AnimationCurve) ||
            //        type.IsEnum ||
            //        type.IsArray ||
            //        type.IsValueType ||
            //        type.IsSubclassOf(typeof(UnityEngine.Object)) ||
            //        type.GetCustomAttribute<SerializableAttribute>() != null;
            return true;
        }
        static void Button(UnityEngine.Object target, MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().All(p => p.IsOptional))
            {
                ButtonMethodAttribute buttonMethodAttribute =
                    (ButtonMethodAttribute)methodInfo.GetCustomAttributes(typeof(ButtonMethodAttribute), true)[0];
                string buttonText = string.IsNullOrEmpty(buttonMethodAttribute.Text)
                    ? ObjectNames.NicifyVariableName(methodInfo.Name)
                    : buttonMethodAttribute.Text;
                if (GUILayout.Button(buttonText))
                {
                    object[] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();
                    IEnumerator resultIEnumerator = methodInfo.Invoke(target, defaultParams) as IEnumerator;
                    if (Application.isPlaying && resultIEnumerator != null && target is MonoBehaviour behaviour)
                    {
                        behaviour.StartCoroutine(resultIEnumerator);
                    }
                }
            }
            else
            {
                string warning = nameof(ButtonMethodAttribute) + " works only on methods with no parameters";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
        }
    }
}