using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySO", menuName = "Scriptable Objects/GameplaySO")]
public class GameplaySO : ScriptableObject
{
    [SerializeField] Color[] colors;
}
