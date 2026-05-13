using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utils
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static object m_Lock = new object();
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                lock (m_Lock)
                {
                    if ((Object)s_Instance == (Object)null)
                    {
                        CreateAndLoad();
                    }

                    return s_Instance;
                }
            }
        }

        protected ScriptableSingleton()
        {
            if ((Object)s_Instance != (Object)null)
            {
                Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
            }
            else
            {
                s_Instance = this as T;
            }
        }

        private static void CreateAndLoad()
        {

            string filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                InternalEditorUtility.LoadSerializedFileAndForget(filePath);
            }

            if ((Object)s_Instance == (Object)null)
            {
                ScriptableObject.CreateInstance<T>().hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            }
        }

        protected virtual void Save(bool saveAsText)
        {
            if ((Object)s_Instance == (Object)null)
            {
                Debug.LogError("Cannot save ScriptableSingleton: no instance!");
                return;
            }

            string filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                Object[] obj = new T[1] { s_Instance };
                InternalEditorUtility.SaveToSerializedFileAndForget(obj, filePath, saveAsText);
            }
            else
            {
                Debug.LogWarning($"Saving has no effect. Your class '{GetType()}' is missing the FilePathAttribute. Use this attribute to specify where to save your ScriptableSingleton.\nOnly call Save() and use this attribute if you want your state to survive between sessions of Unity.");
            }
        }

        protected static string GetFilePath()
        {
            object[] customAttributes = typeof(T).GetCustomAttributes(inherit: true);
            foreach (object obj in customAttributes)
            {
                //if (obj is FilePathAttribute)
                //{
                //    return (obj as FilePathAttribute).Get;
                //}
            }

            return string.Empty;
        }
    }
}

