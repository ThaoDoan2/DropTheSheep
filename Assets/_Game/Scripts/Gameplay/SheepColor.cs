
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "SheepColorConst", menuName = "Scriptable Objects/SheepColorConst")]
    public class SheepColorConst: ScriptableObject
    {
        [SerializeField] Color[] colors;

        public Color GetColor(SheepColor sheepColor)
        {
            Debug.Log($"GetColor {sheepColor}");
            int idx = (int)sheepColor;
            if (idx < colors.Length)
                return colors[idx];
            return Color.white;
        }
    }
}
