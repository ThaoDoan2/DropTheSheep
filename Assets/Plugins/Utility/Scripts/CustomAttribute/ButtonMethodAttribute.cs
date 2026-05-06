using System;

namespace Qutility.CustomEditor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonMethodAttribute : Attribute
    {
        public string Text { get; private set; }
        public ButtonMethodAttribute(string text = null)
        {
            Text = text;
        }
    }
}
