using Gameplay;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySO", menuName = "Scriptable Objects/GameplaySO")]
public class GameplaySO : ScriptableObject
{
    [SerializeField] Color[] colors;

    private static GameplaySO _instance;
    public static GameplaySO instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<GameplaySO>("Assets/_Game/ScriptableObjects/GameplaySO.asset");
            }
            return _instance;
        }
    }

    public Color GetColor(SheepColor sheepColor)
    {
        if (this == null) return Color.white;
        int idx = (int)sheepColor;
        if (colors != null && idx >= 0 && idx < colors.Length)
            return colors[idx];
        return Color.white;
    }
}
