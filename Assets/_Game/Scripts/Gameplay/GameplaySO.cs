using Gameplay;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySO", menuName = "Scriptable Objects/GameplaySO")]
public class GameplaySO : ScriptableSingleton<GameplaySO>
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
